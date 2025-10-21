namespace PetShop.DTO.ForgotPassword
{
    public class ForgotPasswordRequestDto
    {
        public string Email { get; set; } = null!;
    }
    public class ResetPasswordRequestDto
    {
        public string Token { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
