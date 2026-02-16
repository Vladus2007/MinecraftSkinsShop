// Infrastructure service: BTCConnection
// Responsibility:
// - Fetch current BTC/USD price from external APIs using HttpClient
// - Parse several common JSON response shapes (CoinGecko, coinapi, freecryptoapi, etc.)
// - Cache the price in IMemoryCache with a short TTL to reduce external calls
// - Support CancellationToken and log errors

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Application.Services.Interfaces;

namespace Infrastructure.BTCConnection
{
    public class BTCConnection : IGetRateService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BTCConnection> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly string? _fallbackUrl;
        private const string CacheKey = "CurrentBtcPrice";
        private readonly string? _apiKey;
        private readonly TimeSpan _cacheTtl = TimeSpan.FromSeconds(30);

        public BTCConnection(HttpClient httpClient, ILogger<BTCConnection> logger, IMemoryCache memoryCache, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _memoryCache = memoryCache;
            _fallbackUrl = configuration["BtcApi:Url"];
            _apiKey = configuration["BtcApi:Key"];

            // Add Authorization header only if api key present and header not already added
            if (!string.IsNullOrEmpty(_apiKey) && !_httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            }
        }

        /// <summary>
        /// Return current BTC price in USD. Uses cache and supports cancellation.
        /// </summary>
        public async Task<decimal> GetRateAsync(CancellationToken cancellationToken = default)
        {
            // Use GetOrCreateAsync to avoid cache stampede and ensure single fetch when empty
            try
            {
                return await _memoryCache.GetOrCreateAsync(CacheKey, async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = _cacheTtl;

                    var requestUri = ResolveRequestUri();
                    using var response = await _httpClient.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();

                    await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                    using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
                    var root = doc.RootElement;

                    // 1) coinapi.io style: { "rate": 12345.67 }
                    if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("rate", out var rateElement))
                    {
                        if (TryParseDecimal(rateElement, out var r1)) return r1;
                    }

                    // 2) freecryptoapi style: { "symbols": [ { "last": "70446.94", ... } ] }
                    if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("symbols", out var symbolsElement) && symbolsElement.ValueKind == JsonValueKind.Array && symbolsElement.GetArrayLength() > 0)
                    {
                        var first = symbolsElement[0];
                        if (first.TryGetProperty("last", out var lastElement) && TryParseDecimal(lastElement, out var last)) return last;
                    }

                    // 3) CoinGecko simple price: { "bitcoin": { "usd": 12345.67 } }
                    if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("bitcoin", out var bitcoin) && bitcoin.ValueKind == JsonValueKind.Object && bitcoin.TryGetProperty("usd", out var usdElement) && TryParseDecimal(usdElement, out var usd))
                    {
                        return usd;
                    }

                    // 4) fallback: try named properties
                    foreach (var prop in root.EnumerateObject())
                    {
                        if (prop.NameEquals("price") || prop.NameEquals("last_price") || prop.NameEquals("last") || prop.NameEquals("price_usd"))
                        {
                            if (TryParseDecimal(prop.Value, out var p)) return p;
                        }
                    }

                    _logger.LogWarning("Unexpected BTC API response JSON structure: no recognized rate field");
                    return -1m;

                }).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("BTC rate fetch canceled");
                throw;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Exception while fetching BTC rate");
                return -1m;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse BTC API JSON");
                return -1m;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected exception while fetching BTC rate");
                return -1m;
            }

            static bool TryParseDecimal(JsonElement el, out decimal value)
            {
                value = 0m;
                if (el.ValueKind == JsonValueKind.Number && el.TryGetDecimal(out value)) return true;
                if (el.ValueKind == JsonValueKind.String && decimal.TryParse(el.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out value)) return true;
                return false;
            }

            Uri ResolveRequestUri()
            {
                if (_httpClient.BaseAddress != null && _httpClient.BaseAddress.IsAbsoluteUri)
                {
                    return _httpClient.BaseAddress;
                }

                if (!string.IsNullOrEmpty(_fallbackUrl))
                {
                    if (Uri.TryCreate(_fallbackUrl, UriKind.Absolute, out var u)) return u;
                    // If fallback is relative, and BaseAddress exists (unlikely here), combine
                    if (_httpClient.BaseAddress != null && Uri.TryCreate(_httpClient.BaseAddress, _fallbackUrl, out var combined)) return combined;
                }

                throw new InvalidOperationException("No BTC API URL configured (BaseAddress or BtcApi:Url)");
            }
        }
    }
}
