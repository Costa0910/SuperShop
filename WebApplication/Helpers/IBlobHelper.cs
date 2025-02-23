using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebApplication.Helpers
{
    public interface IBlobHelper
    {
        Task<Guid> UploadBlobAsync(IFormFile file, string containerName);
        Task<Guid> UploadBlobAsync(byte[] file, string containerName);
        Task<Guid> UploadBlobAsync(string file, string containerName);
    }
}