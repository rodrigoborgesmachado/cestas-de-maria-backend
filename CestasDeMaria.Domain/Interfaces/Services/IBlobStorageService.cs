namespace CestasDeMaria.Domain.Interfaces.Services
{
    public interface IBlobStorageService
    {
        Task<string> UploadFileAsync(string blobName, string content);
        Task<string> UploadFileAsync(string blobName, string base64, string fileName);
        Task<string> UploadFileAsync(byte[] bytes, string fileName);
        Task<string> UploadFileAsync(string container, byte[] bytes, string fileName);
        Task<bool> FileExistsAsync(string blobName, string keyName);
        Task<Stream> DownloadFileAsync(string blobName, string keyName);
    }
}
