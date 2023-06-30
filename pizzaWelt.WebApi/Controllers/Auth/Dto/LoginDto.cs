namespace PizzaWelt.DTOs
{
    public class LoginDto
    {
        [Required]
        public required string Email { get; set; }

        [ReadOnly(true)]
        [Required]
        public required string Password { get; set; }

        public required string ClientUrl { get; set; }
    }

    public class AuthResponseDto
    {
        public bool AuthSuccessful { get; set; }
        public bool TwoFactorVerificationEnabled { get; set; }
        public string? Provider { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
