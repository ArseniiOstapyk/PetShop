using PetShop.Models;

namespace PetShop.Interfaces
{
    public interface IPhotoService
    {
        Task<Photo> UploadPhotoAsync(IFormFile file);
        Task<(Stream?, string?)> GetPhotoStreamAsync(int photoId);
        Task<bool> DeletePhotoAsync(int photoId);
    }
}
