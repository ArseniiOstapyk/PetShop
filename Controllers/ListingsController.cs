using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetShop.DTO.Listing;
using PetShop.Interfaces;
using PetShop.Models;
using PetShop.Repositories;
using System.Security.Claims;

namespace PetShop.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ListingsController : ControllerBase
    {
        private readonly IListingRepository _listingRepo;

        public ListingsController(IListingRepository listingRepo)
        {
            _listingRepo = listingRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var listings = await _listingRepo.GetAllAsync();
            return Ok(listings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var listing = await _listingRepo.GetByIdAsync(id);
            if (listing == null) return NotFound();
            return Ok(listing);
        }

        [Authorize(Roles = "Seller")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateListingDto dto)
        {
            // Extract the user ID from the JWT claims
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("Invalid token: user ID not found.");
            }

            int ownerId = int.Parse(userIdClaim.Value);

            var createdListing = await _listingRepo.AddAsync(dto, ownerId);
            return CreatedAtAction(nameof(GetById), new { id = createdListing.Id }, createdListing);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateListingDto dto)
        {
            // ✅ Extract user info from token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var roleClaim = User.FindFirst(ClaimTypes.Role);

            if (userIdClaim == null || roleClaim == null)
                return Unauthorized("Invalid token.");

            int userId = int.Parse(userIdClaim.Value);
            string role = roleClaim.Value;

            // ✅ Allow only Seller
            if (role != "Seller")
                return Forbid("Only sellers can update listings.");

            // ✅ Check ownership
            var listing = await _listingRepo.GetByIdAsync(id);
            if (listing == null)
                return NotFound("Listing not found.");

            if (listing.OwnerId != userId)
                return Forbid("You can only update your own listings.");

            var updated = await _listingRepo.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // ✅ Extract user info from token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var roleClaim = User.FindFirst(ClaimTypes.Role);

            if (userIdClaim == null || roleClaim == null)
                return Unauthorized("Invalid token.");

            int userId = int.Parse(userIdClaim.Value);
            string role = roleClaim.Value;

            // ✅ Get listing
            var listing = await _listingRepo.GetByIdAsync(id);
            if (listing == null)
                return NotFound("Listing not found.");

            // ✅ Allow if Admin OR Seller + Owner
            if (role != "Admin" && !(role == "Seller" && listing.OwnerId == userId))
                return Forbid("You are not authorized to delete this listing.");

            var deleted = await _listingRepo.DeleteAsync(id);
            if (!deleted)
                return StatusCode(500, "Failed to delete listing.");

            return NoContent();
        }
    }
}
