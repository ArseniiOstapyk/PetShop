using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using PetShop.Interfaces;
using PetShop.Models;

namespace PetShop.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly PetShopContext _context;
        private readonly S3Service _s3Service;

        public PhotoService(PetShopContext context, S3Service s3Service)
        {
            _context = context;
            _s3Service = s3Service;
        }

        public async Task<Photo> UploadPhotoAsync(IFormFile file)
        {
            var url = await _s3Service.UploadFileAsync(file);

            var photo = new Photo
            {
                Url = url,
                CreatedOn = DateTime.UtcNow
            };

            _context.Photos.Add(photo);
            await _context.SaveChangesAsync();
            return photo;
        }

        public async Task<(Stream?, string?)> GetPhotoStreamAsync(int photoId)
        {
            var photo = await _context.Photos.FindAsync(photoId);
            if (photo == null) return (null, null);

            var key = photo.Url.Split(".com/").Last();
            return await _s3Service.GetFileAsync(key);
        }

        public async Task<bool> DeletePhotoAsync(int photoId)
        {
            var photo = await _context.Photos.FindAsync(photoId);
            if (photo == null) return false;

            var key = photo.Url.Split(".com/").Last();
            await _s3Service.DeleteFileAsync(key);

            _context.Photos.Remove(photo);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
