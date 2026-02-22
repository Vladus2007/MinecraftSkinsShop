namespace Infrastructure.Handlers
{
    /// <summary>
    /// Simple mock authentication handler for development/testing.
    /// Produces a principal with NameIdentifier (numeric string), Name and Role="User".
    /// </summary>
    public class MockAuthoriseHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public MockAuthoriseHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var headerValues))
            {
                return Task.FromResult(AuthenticateResult.Fail("Missing Authorization header."));
            }

            var header = headerValues.ToString();
            if (string.IsNullOrWhiteSpace(header))
            {
                return Task.FromResult(AuthenticateResult.Fail("Empty Authorization header."));
            }

            // Support formats: "Bearer {userId}" or just "{userId}"
            var token = header;
            const string bearerPrefix = "Bearer ";
            if (header.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
            {
                token = header.Substring(bearerPrefix.Length).Trim();
            }

            if (!int.TryParse(token, out var userId))
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid user id in Authorization header. Expected integer."));
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, $"Mock user {userId}"),
                new Claim(ClaimTypes.Role, "User")
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}