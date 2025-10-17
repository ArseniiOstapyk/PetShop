using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using PetShop.DTO.Listing;
using PetShop.Interfaces;
using PetShop.Models;

namespace PetShop.Repositories
{
    public class ListingRepository : IListingRepository
    {
        private readonly PetShopContext _context;

        public ListingRepository(PetShopContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ListingDto>> GetAllAsync()
        {
            return await _context.Listings
                .Include(l => l.Currency)
                .Include(l => l.Owner)
                .Include(l => l.Type)
                .Include(l => l.Photo)
                .Select(l => new ListingDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    Description = l.Description,
                    Price = l.Price,
                    Currency = l.Currency.Name,
                    Type = l.Type.Name,
                    IsAvailable = l.IsAvailable,
                    Owner = l.Owner.Email,
                    PhotoUrl = l.Photo != null ? l.Photo.Url : null,
                    OwnerId = l.OwnerId // ✅ include for checks
                })
                .ToListAsync();
        }

        public async Task<ListingDto?> GetByIdAsync(int id)
        {
            return await _context.Listings
                .Include(l => l.Currency)
                .Include(l => l.Owner)
                .Include(l => l.Type)
                .Include(l => l.Photo)
                .Where(l => l.Id == id)
                .Select(l => new ListingDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    Description = l.Description,
                    Price = l.Price,
                    Currency = l.Currency.Name,
                    Type = l.Type.Name,
                    IsAvailable = l.IsAvailable,
                    Owner = l.Owner.Email,
                    PhotoUrl = l.Photo != null ? l.Photo.Url : null,
                    OwnerId = l.OwnerId // ✅ included here too
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ListingDto> AddAsync(CreateListingDto dto, int ownerId)
        {
            var listing = new Listing
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                TypeId = dto.TypeId,
                CurrencyId = dto.CurrencyId,
                PhotoId = dto.PhotoId,
                IsAvailable = dto.IsAvailable,
                OwnerId = ownerId,
                CreatedOn = DateTime.UtcNow
            };

            _context.Listings.Add(listing);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(listing.Id)
                ?? throw new Exception("Failed to retrieve listing after creation.");
        }

        public async Task<ListingDto?> UpdateAsync(int id, UpdateListingDto dto)
        {
            var listing = await _context.Listings.FindAsync(id);
            if (listing == null) return null;

            listing.Name = dto.Name;
            listing.Description = dto.Description;
            listing.Price = dto.Price;
            listing.TypeId = dto.TypeId;
            listing.CurrencyId = dto.CurrencyId;
            listing.PhotoId = dto.PhotoId;
            listing.IsAvailable = dto.IsAvailable;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var listing = await _context.Listings.FindAsync(id);
            if (listing == null) return false;

            _context.Listings.Remove(listing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
