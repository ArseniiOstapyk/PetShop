using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using PetShop.DTO.Lookup;
using PetShop.Interfaces;


namespace PetShop.Repositories
{
    public class LookupRepository : ILookupRepository
    {
        private readonly PetShopContext _context;

        public LookupRepository(PetShopContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CurrencyDto>> GetCurrenciesAsync()
        {
            return await _context.Currencies
                .Select(c => new CurrencyDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Symbol = c.Sign
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ListingTypeDto>> GetListingTypesAsync()
        {
            return await _context.ListingTypes
                .Select(t => new ListingTypeDto
                {
                    Id = t.Id,
                    Name = t.Name!
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<RoleDto>> GetRolesAsync()
        {
            return await _context.Roles
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name!
                })
                .OrderBy(r => r.Name)
                .ToListAsync();
        }
    }
}
