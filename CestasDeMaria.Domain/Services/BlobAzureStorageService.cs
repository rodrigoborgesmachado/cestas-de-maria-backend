using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CestasDeMaria.Domain.Interfaces.Services;
using CestasDeMaria.Domain.ModelClasses;
using Microsoft.Extensions.Options;
using System.Text;

namespace CestasDeMaria.Domain.Services
{
    public class BlobAzureStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly Settings _settings;

        public BlobAzureStorageService(IOptions<Settings> options)
        {
            _settings = options.Value;
            _blobServiceClient = new BlobServiceClient(_settings.AzureStorageConnectionString);

        }

        public async Task<string> UploadFileAsync(string blobName, string content)
        {
            return await UploadFileAsync(_settings.AzureStorageDefaultContainer, Encoding.UTF8.GetBytes(content), blobName);
        }

        public async Task<string> UploadFileAsync(byte[] bytes, string fileName)
        {
            return await UploadFileAsync(_settings.AzureStorageDefaultContainer, bytes, fileName);
        }

        public async Task<string> UploadFileAsync(string blobName, string base64, string fileName)
        {
            if (string.IsNullOrEmpty(base64))
            {
                throw new ArgumentNullException(nameof(base64));
            }

            string temp = base64.Split(',')[0];
            base64 = base64.Split(',')[1];
            temp = temp.Split(";")[0];
            temp = temp.Split("/")[1];

            fileName = $"{fileName}.{temp}";

            return await UploadFileAsync(blobName, Convert.FromBase64String(base64), fileName);
        }

        public async Task<string> UploadFileAsync(string container, byte[] bytes, string fileName)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient(container);

            var blobClient = blobContainer.GetBlobClient(fileName);

            using var stream = new MemoryStream(bytes);
            await blobClient.UploadAsync(stream, overwrite: true);

            return blobClient.Uri.ToString();
        }

        public async Task<bool> FileExistsAsync(string blobName, string keyName)
        {
            var _blobContainerClient = _blobServiceClient.GetBlobContainerClient(_settings.AzureStorageDefaultContainer);
            BlobClient blobClient = _blobContainerClient.GetBlobClient(blobName);

            return await blobClient.ExistsAsync();
        }

        public async Task<Stream> DownloadFileAsync(string blobName, string keyName)
        {
            var _blobContainerClient = _blobServiceClient.GetBlobContainerClient(_settings.AzureStorageDefaultContainer);
            BlobClient blobClient = _blobContainerClient.GetBlobClient(blobName);

            if (await blobClient.ExistsAsync())
            {
                BlobDownloadInfo download = await blobClient.DownloadAsync();

                return download.Content;
            }
            else
            {
                throw new FileNotFoundException($"The blob '{blobName}' does not exist.");
            }
        }
    }
}
