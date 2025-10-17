using PetShop.DTO.Listing;
using PetShop.Models;

namespace PetShop.Interfaces
{
    public interface IListingRepository
    {
        Task<IEnumerable<ListingDto>> GetAllAsync();
        Task<ListingDto?> GetByIdAsync(int id);
        Task<ListingDto> AddAsync(CreateListingDto dto, int ownerId); // ✅ ownerId added
        Task<ListingDto?> UpdateAsync(int id, UpdateListingDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
