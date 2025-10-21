using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetShop.Interfaces;

namespace PetShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhotosController : ControllerBase
    {
        private readonly IPhotoService _photoService;

        public PhotosController(IPhotoService photoService)
        {
            _photoService = photoService;
        }

        [Authorize(Roles = "Seller")]
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file == null) return BadRequest("No file uploaded.");
            var photo = await _photoService.UploadPhotoAsync(file);
            return Ok(photo);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Download(int id)
        {
            var (stream, contentType) = await _photoService.GetPhotoStreamAsync(id);
            if (stream == null) return NotFound();

            return File(stream, contentType ?? "application/octet-stream");
        }

        [Authorize(Roles = "Seller,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _photoService.DeletePhotoAsync(id);
            if (!deleted) return NotFound();

            return Ok("Photo deleted successfully.");
        }
    }
}
