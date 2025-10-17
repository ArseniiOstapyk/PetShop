using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using PetShop.DTO;
using PetShop.DTO.Auth;

namespace PetShop.Services
{
    public class AuthenticationService
    {
        private readonly PetShopContext _context;
        private readonly JwtService _jwtService;

        public AuthenticationService(PetShopContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<string> AuthenticateAsync(LoginRequestDto request)
        {
            var user = await _context.Users.Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            return _jwtService.GenerateToken(user);
        }
    }
}
