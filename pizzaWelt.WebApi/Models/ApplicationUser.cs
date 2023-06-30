namespace PizzaWelt.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? UserAccountsId { get; set; }
        public UserAccounts UserAccounts { get; set; } = null!;
        public string Street { get; set; } = string.Empty;
        public string HouseNumber { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int ZipCode { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTimeOffset RefreshTokenExpiryTime { get; set; }
    }
}
