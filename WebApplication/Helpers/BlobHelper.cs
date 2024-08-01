using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace WebApplication.Helpers
{
    public class BlobHelper : IBlobHelper
    {
        readonly CloudBlobClient _blobClient;

        public BlobHelper(IConfiguration configuration)
        {
            var key = configuration["Blob:ImageStorage"];
            var storageAccount = CloudStorageAccount.Parse(key);
            _blobClient = storageAccount.CreateCloudBlobClient();
        }

        public async Task<Guid> UploadBlobAsync(IFormFile file, string containerName)
        {
            var stream = file.OpenReadStream();

            return await UploadStreamAsync(stream, containerName);
        }

        public async Task<Guid> UploadBlobAsync(byte[] file, string containerName)
        {
            Stream stream = new MemoryStream(file);

            return await UploadStreamAsync(stream, containerName);
        }

        public async Task<Guid> UploadBlobAsync(string file, string containerName)
        {
            Stream stream = File.OpenRead(file);

            return await UploadStreamAsync(stream, containerName);
        }

        async Task<Guid> UploadStreamAsync(Stream stream, string containerName)
        {
            var newId = Guid.NewGuid();
            var containerReference = _blobClient.GetContainerReference(containerName);
            var newBlob = containerReference.GetBlockBlobReference($"{newId}");
            await newBlob.UploadFromStreamAsync(stream);

            return newId;
        }
    }
}