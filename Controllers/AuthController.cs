using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetShop.DTO;
using PetShop.DTO.Auth;
using PetShop.DTO.ForgotPassword;
using PetShop.Services;

namespace PetShop.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthenticationService _authService;
        private readonly RegistrationService _registrationService;
        private readonly PasswordResetService _passwordResetService;

        public AuthController(AuthenticationService authService, RegistrationService registrationService, PasswordResetService passwordResetService)
        {
            _authService = authService;
            _registrationService = registrationService;
            _passwordResetService = passwordResetService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var token = await _authService.AuthenticateAsync(request);
            return Ok(new { token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var user = await _registrationService.RegisterAsync(request);
            return Ok(new { user.Id, user.Email, user.RoleId });
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            await _passwordResetService.RequestPasswordResetAsync(request.Email);
            return Ok("If the email exists, a reset link has been sent.");
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            var result = await _passwordResetService.ResetPasswordAsync(request.Token, request.NewPassword);
            if (!result) return BadRequest("Invalid or expired token.");
            return Ok("Password reset successfully.");
        }
    }
}
