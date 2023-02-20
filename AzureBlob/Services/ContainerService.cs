using Azure.Storage.Blobs;

namespace AzureBlob.Services
{
    public class ContainerService : IContainerService
    {
        private BlobServiceClient _client;

        public ContainerService(BlobServiceClient client)
        {
            _client = client;
        }

        public async Task CreateContainer(string containerName)
        {
            var blobContainerClient = await _client.CreateBlobContainerAsync(containerName);
            if(blobContainerClient != null)
            {
                await blobContainerClient.Value.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);                
            }
        }

        public async Task DeleteContainer(string containerName)
        {
            var response  = await _client.DeleteBlobContainerAsync(containerName);
            if(response != null && !response.IsError)
            {
                response.Dispose();
            }
        }

        public async Task<List<string>> GetAllContainer()
        {
            List<string> Containers = new();
            var blobContainers = _client.GetBlobContainersAsync();
            await foreach(var container in blobContainers)
            {
                Containers.Add(container.Name);
            }

            return Containers;
        }

        public async Task<List<string>> GetAllContainerAndBlobs()
        {
            throw new NotImplementedException();
        }
    }
}
