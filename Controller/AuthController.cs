using Microsoft.AspNetCore.Mvc;
using PetShop.DTO;
using PetShop.Services;

namespace PetShop.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthenticationService _authService;
        private readonly RegistrationService _registrationService;

        public AuthController(AuthenticationService authService, RegistrationService registrationService)
        {
            _authService = authService;
            _registrationService = registrationService;
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
    }
}
