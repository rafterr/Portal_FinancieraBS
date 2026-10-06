using Microsoft.AspNetCore.Http;

namespace FinancieraBS.Services
{
    public interface IFirebaseStorageService
    {
        Task<string> UploadFileAsync(IFormFile file, string folder, string fileName);
        Task<bool> DeleteFileAsync(string fileUrl);
        Task<string> GetFileUrlAsync(string filePath);
    }
}
