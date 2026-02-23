using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Handlers
{
    /// <summary>
    /// Simple mock authentication handler for development/testing.
    /// Usage: Authorization: Bearer {userId} (e.g. Bearer 123)
    /// </summary>
    public class MockAuthoriseHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        
        public MockAuthoriseHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder,clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // 1. Check for header
            if (!Request.Headers.TryGetValue("Authorization", out var headerValues))
            {
                return Task.FromResult(AuthenticateResult.Fail("Missing Authorization header."));
            }

            var header = headerValues.ToString();
            if (string.IsNullOrWhiteSpace(header))
            {
                return Task.FromResult(AuthenticateResult.Fail("Empty Authorization header."));
            }

            // 2. Parse token (strip "Bearer " if present)
            var token = header;
            const string bearerPrefix = "Bearer ";
            
            if (token.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
            {
                token = token.Substring(bearerPrefix.Length).Trim();
            }

            // 3. Validate Format (expecting integer ID)
            if (!int.TryParse(token, out var userId))
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid user id. Expected integer."));
            }

            // 4. Create Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, $"Mock user {userId}"),
                new Claim(ClaimTypes.Role, "User") // Hardcoded role
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}