using Microsoft.AspNetCore.Mvc;
using PetShop.Interfaces;

namespace PetShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LookupController : ControllerBase
    {
        private readonly ILookupRepository _repository;

        public LookupController(ILookupRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("Currencies")]
        public async Task<IActionResult> GetCurrencies()
        {
            var currencies = await _repository.GetCurrenciesAsync();
            return Ok(currencies);
        }

        [HttpGet("ListingTypes")]
        public async Task<IActionResult> GetListingTypes()
        {
            var types = await _repository.GetListingTypesAsync();
            return Ok(types);
        }

        [HttpGet("Roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _repository.GetRolesAsync();
            return Ok(roles);
        }
    }
}
