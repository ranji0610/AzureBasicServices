using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AzureBlob.Services
{
    public class BlobService : IBlobService
    {
        private BlobServiceClient _client;

        public BlobService(BlobServiceClient client)
        {
            _client = client;
        }
        public async Task<bool> DeleteBlob(string name, string containerName)
        {
            var blobContainerClient = _client.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(name);
            return await blobClient.DeleteIfExistsAsync();
        }

        public async Task<List<string>> GetAllBlobs(string containerName)
        {
            List<string> Blobs = new();
            var blobContainerClient = _client.GetBlobContainerClient(containerName);
            var blobs = blobContainerClient.GetBlobsAsync();
            await foreach (var blob in blobs)
            {
                Blobs.Add(blob.Name);
            }

            return Blobs;
        }

        public Task<string> GetBlob(string name, string containerName)
        {
            var blobContainerClient = _client.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(name);
            return Task.FromResult(blobClient.Uri.AbsoluteUri);
        }

        public Task<bool> UploadBlob(string name, IFormFile file, string containerName)
        {
            var blobContainerClient = _client.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(name);
            var httpHeaders = new BlobHttpHeaders()
            {
                ContentType = file.ContentType
            };

            var result = blobClient.UploadAsync(file.OpenReadStream(), httpHeaders).Result;

            if (result != null && !result.GetRawResponse().IsError)
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}
