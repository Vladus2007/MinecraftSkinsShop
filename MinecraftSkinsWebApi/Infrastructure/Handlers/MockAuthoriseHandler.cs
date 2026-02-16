protected override Task<AuthenticateResult> HandleAuthenticateAsync()
{
    

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

    // !!! ВОТ ЭТО ОТСУТСТВУЕТ - парсинг userId из токена !!!
    if (!int.TryParse(token, out var userId))
    {
        return Task.FromResult(AuthenticateResult.Fail("Invalid user ID format. Expected numeric value."));
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