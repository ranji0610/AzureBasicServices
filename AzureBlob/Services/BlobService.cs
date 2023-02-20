using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureBlob.Models;

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

        public async Task<List<Blob>> GetAllBlobsWithUrl(string containerName)
        {
            List<Blob> Blobs = new();
            var blobContainerClient = _client.GetBlobContainerClient(containerName);
            var blobs = blobContainerClient.GetBlobsAsync();
            await foreach (var blob in blobs)
            {
                var blobClient = blobContainerClient.GetBlobClient(blob.Name);
                var blobModel = new Blob()
                {
                    Uri = blobClient.Uri.AbsoluteUri
                };

                var properties = await blobClient.GetPropertiesAsync();
                if (properties != null && properties.Value.Metadata.ContainsKey("Title"))
                {
                    blobModel.Title = properties.Value.Metadata["Title"];
                }

                Blobs.Add(blobModel);
            }

            return Blobs;
        }

        public Task<string> GetBlob(string name, string containerName)
        {
            var blobContainerClient = _client.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(name);
            return Task.FromResult(blobClient.Uri.AbsoluteUri);
        }

        public async Task<bool> UploadBlob(string name, IFormFile file, string containerName, Blob blob)
        {
            var blobContainerClient = _client.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(name);
            var httpHeaders = new BlobHttpHeaders()
            {
                ContentType = file.ContentType
            };

            IDictionary<string, string> metaData = new Dictionary<string, string>();
            metaData[nameof(blob.Title)] = blob.Title;
            metaData[nameof(blob.Comment)] = blob.Comment;
            var result = blobClient.UploadAsync(file.OpenReadStream(), httpHeaders, metaData).Result;
            //metaData.Remove(nameof(blob.Title));
            //await blobClient.SetMetadataAsync(metaData);
            if (result != null && !result.GetRawResponse().IsError)
            {
                return true;
            }

            return false;
        }
    }
}
