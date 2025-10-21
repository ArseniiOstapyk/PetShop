using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace PetShop.Services
{
    public class S3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IConfiguration _config;
        private readonly string _bucketName;

        public S3Service(IAmazonS3 s3Client, IConfiguration config)
        {
            _s3Client = s3Client;
            _config = config;
            _bucketName = _config["Aws:BucketName"] ?? throw new Exception("AWS Bucket not set");
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var key = $"photos/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            using var stream = file.OpenReadStream();
            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = stream,
                Key = key,
                BucketName = _bucketName,
                ContentType = file.ContentType
            };

            var fileTransferUtility = new TransferUtility(_s3Client);
            await fileTransferUtility.UploadAsync(uploadRequest);

            return $"https://{_bucketName}.s3.amazonaws.com/{key}";
        }

        public async Task<(Stream?, string?)> GetFileAsync(string key)
        {
            try
            {
                var response = await _s3Client.GetObjectAsync(_bucketName, key);
                return (response.ResponseStream, response.Headers.ContentType);
            }
            catch (AmazonS3Exception)
            {
                return (null, null);
            }
        }

        public async Task DeleteFileAsync(string key)
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(deleteRequest);
        }
    }
}
