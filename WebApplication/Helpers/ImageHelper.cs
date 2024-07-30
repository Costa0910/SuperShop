using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebApplication.Helpers
{
    public class ImageHelper : IImageHelper
    {
        public async Task<string> UploadImageAsync(IFormFile file, string folder)
        {
            var newFileName = $"{Guid.NewGuid()}.jpg";

            var path = Path.Combine(Directory.GetCurrentDirectory(), $@"wwwroot\images\{folder}", newFileName);

            await using var stream = new FileStream(path, FileMode.Create);

            await file.CopyToAsync(stream);
            path = $"~/images/{folder}/{newFileName}";

            return path;
        }
    }
}