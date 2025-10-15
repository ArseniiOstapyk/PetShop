using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using PetShop.DTO;
using PetShop.Models;

namespace PetShop.Services
{
    public class RegistrationService
    {
        private readonly PetShopContext _context;

        public RegistrationService(PetShopContext context)
        {
            _context = context;
        }

        public async Task<User> RegisterAsync(RegisterRequestDto registerRequest)
        {
            if (await _context.Users.AnyAsync(u => u.Email == registerRequest.Email))
                throw new Exception("Email is already registered.");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);

            var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            if (defaultRole == null)
                throw new Exception("Default role 'User' not found. Please seed roles first.");

            var user = new User
            {
                Email = registerRequest.Email,
                Password = hashedPassword,
                PhoneNumber = registerRequest.PhoneNumber,
                RoleId = defaultRole.Id,
                CreatedOn = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }
    }
}
