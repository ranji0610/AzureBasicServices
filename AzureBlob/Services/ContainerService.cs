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
            if (blobContainerClient != null)
            {
                await blobContainerClient.Value.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            }
        }

        public async Task DeleteContainer(string containerName)
        {
            var response = await _client.DeleteBlobContainerAsync(containerName);
            if (response != null && !response.IsError)
            {
                response.Dispose();
            }
        }

        public async Task<List<string>> GetAllContainer()
        {
            List<string> Containers = new();
            var blobContainers = _client.GetBlobContainersAsync();
            await foreach (var container in blobContainers)
            {
                Containers.Add(container.Name);
            }

            return Containers;
        }

        public async Task<List<string>> GetAllContainerAndBlobs()
        {
            List<string> ContainerAndBlobs = new();
            ContainerAndBlobs.Add("Account Name : " + _client.AccountName);
            ContainerAndBlobs.Add("=============================================================");
            await foreach (var container in _client.GetBlobContainersAsync())
            {
                ContainerAndBlobs.Add("--" + container.Name);
                var blobClient = _client.GetBlobContainerClient(container.Name);
                await foreach (var blob in blobClient.GetBlobsAsync())
                {
                    var bc = blobClient.GetBlobClient(blob.Name);
                    var props = await bc.GetPropertiesAsync();
                    string blobToAdd = blob.Name;
                    if (props != null && props.Value.Metadata.ContainsKey("title"))
                    {
                        blobToAdd += "(" + props.Value.Metadata["title"] + ")";
                    }
                    ContainerAndBlobs.Add("------" + blobToAdd);
                }

                ContainerAndBlobs.Add("=============================================================");
            }
            return ContainerAndBlobs;
        }
    }
}
