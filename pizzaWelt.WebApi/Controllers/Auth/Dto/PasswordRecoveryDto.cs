namespace PizzaWelt.DTOs
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string ClientUrl { get; set; }
    }

    public class PasswordRecoveryDto
    {
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public required string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Password and conformation password do not match")]
        public required string ConfirmNewPassword { get; set; }

        [Required]
        public required string Email { get; set; }

        [Required]
        public required string Token { get; set; }
    }

    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public required string OldPassword { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public required string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Password and conformation password do not match")]
        public required string ConfirmNewPassword { get; set; }

        [Required]
        public required string Token { get; set; }
    }
}
