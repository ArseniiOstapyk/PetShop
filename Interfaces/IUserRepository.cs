using PetShop.DTO.User;
using PetShop.Models;

namespace PetShop.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<(bool Success, string Message, string? NewRole)> UpdateRoleAsync(int id, string roleName);
    }
}
