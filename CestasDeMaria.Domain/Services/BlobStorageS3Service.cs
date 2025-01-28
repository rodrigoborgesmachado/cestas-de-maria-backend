using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using CestasDeMaria.Domain.Interfaces.Services;
using CestasDeMaria.Domain.ModelClasses;
using Microsoft.Extensions.Options;
using System.Text;

namespace CestasDeMaria.Domain.Services
{
    public class BlobStorageS3Service : IBlobStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly Settings _settings;

        public BlobStorageS3Service(IOptions<Settings> options)
        {
            _settings = options.Value;

            var s3Config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(_settings.S3Region)
            };
            _s3Client = new AmazonS3Client(_settings.S3AccessKey, _settings.S3SecretKey, s3Config);
        }

        public async Task<string> UploadFileAsync(byte[] bytes, string keyName)
        {
            return await UploadFileAsync(_settings.AwsS3DefaultBucket, bytes, keyName);
        }

        public async Task<string> UploadFileAsync(string bucketName, string base64, string keyName)
        {
            if (string.IsNullOrEmpty(base64))
            {
                throw new ArgumentNullException(nameof(base64));
            }

            string temp = base64.Split(',')[0];
            base64 = base64.Split(',')[1];
            temp = temp.Split(";")[0];
            temp = temp.Split("/")[1];

            keyName = $"{keyName}.{temp}";

            return await UploadFileAsync(bucketName, Convert.FromBase64String(base64), keyName);
        }

        public async Task<string> UploadFileAsync(string bucketName, byte[] bytes, string keyName)
        {
            using var stream = new MemoryStream(bytes);
            var putRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = keyName,
                InputStream = stream,
                ContentType = "application/octet-stream"
            };

            var response = await _s3Client.PutObjectAsync(putRequest);

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return $"https://{bucketName}.s3.amazonaws.com/{keyName}";
            }

            throw new Exception($"Failed to upload file to S3. Status code: {response.HttpStatusCode}");
        }

        public async Task<bool> FileExistsAsync(string bucketName, string keyName)
        {
            try
            {
                var request = new GetObjectMetadataRequest
                {
                    BucketName = bucketName,
                    Key = keyName
                };

                var response = await _s3Client.GetObjectMetadataAsync(request);
                return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        public async Task<Stream> DownloadFileAsync(string bucketName, string keyName)
        {
            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName
                };

                var response = await _s3Client.GetObjectAsync(request);
                return response.ResponseStream;
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new FileNotFoundException($"The file '{keyName}' does not exist in the bucket '{bucketName}'.");
            }
        }

        public async Task<string> UploadFileAsync(string blobName, string content)
        {
            return await UploadFileAsync(_settings.AwsS3DefaultBucket, Encoding.UTF8.GetBytes(content), blobName);
        }
    }
}
