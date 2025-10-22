using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetShop.DTO.User;
using PetShop.Interfaces;

namespace PetShop.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;

        public UsersController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userRepo.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return NotFound("User not found.");
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _userRepo.DeleteAsync(id);
            if (!success) return NotFound("User not found or already deleted.");
            return NoContent();
        }

        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateUserRoleDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Role))
                return BadRequest("Role is required.");

            var result = await _userRepo.UpdateRoleAsync(id, dto.Role);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new { message = "Role updated successfully.", newRole = result.NewRole });
        }
    }
}
