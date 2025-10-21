using PetShop.DTO.Lookup;

namespace PetShop.Interfaces
{
    public interface ILookupRepository
    {
        Task<IEnumerable<CurrencyDto>> GetCurrenciesAsync();
        Task<IEnumerable<ListingTypeDto>> GetListingTypesAsync();
    }
}
