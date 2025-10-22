using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using PetShop.DTO.User;
using PetShop.Interfaces;
using PetShop.Models;

namespace PetShop.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly PetShopContext _context;

        public UserRepository(PetShopContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.Role)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Role = u.Role != null ? u.Role.Name : "Unknown",
                    CreatedOn = u.CreatedOn
                })
                .OrderByDescending(u => u.Id)
                .ToListAsync();
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Id == id)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Role = u.Role != null ? u.Role.Name : "Unknown",
                    CreatedOn = u.CreatedOn
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Success, string Message, string? NewRole)> UpdateRoleAsync(int id, string roleName)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return (false, "User not found.", null);

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
            if (role == null)
                return (false, "Invalid role name.", null);

            user.RoleId = role.Id;
            await _context.SaveChangesAsync();

            return (true, "Role updated successfully.", role.Name);
        }
    }
}
