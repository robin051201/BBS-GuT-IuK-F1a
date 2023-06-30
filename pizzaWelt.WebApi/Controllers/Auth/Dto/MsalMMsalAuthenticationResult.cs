namespace PizzaWelt.DTOs
{
    public class MsalMMsalAuthenticationResultDto
    {
        public string? AccesToken { get; set; }
        public MsalAccountInfoDto? Account { get; set; }
        public string? Authority { get; set; }
        public string? CorrelationId { get; set; }
        public DateTimeOffset ExpiresOn { get; set; }
        public DateTimeOffset ExtExpiresOn { get; set; }
        public string? IdToken { get; set; }
        public object? IdTokenClaims { get; set; }
        public string[]? Scopes { get; set; }
        public string? TenantId { get; set; }
        public string? TokenType { get; set; }
        public string? UniqueId { get; set; }
        public string? ClientUrl { get; set; }
    }

    public class MsalAccountInfoDto
    {
        public string? HomeAccountId { get; set; }
        public string? Environment { get; set; }
        public string? TenantId { get; set; }
        public string? Username { get; set; }
        public string? LocalAccountId { get; set; }
        public string? Name { get; set; }
        public object? IdTokenClaims { get; set; }
    }
}
