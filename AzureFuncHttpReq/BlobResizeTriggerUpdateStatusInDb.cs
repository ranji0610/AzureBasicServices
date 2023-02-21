using System;
using System.IO;
using System.Threading.Tasks;
using AzureFuncHttpReq.Data;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AzureFuncHttpReq
{
    public class BlobResizeTriggerUpdateStatusInDb
    {
        private readonly AzureSalesDBContext _azureSalesDBContext;

        public BlobResizeTriggerUpdateStatusInDb(AzureSalesDBContext azureSalesDBContext)
        {
            _azureSalesDBContext = azureSalesDBContext;
        }

        [FunctionName("BlobResizeTriggerUpdateStatusInDb")]
        public async Task Run([BlobTrigger("functionsalesrep-sm/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
            var fileName = Path.GetFileNameWithoutExtension(name);
            SalesRequest salesRequest = await _azureSalesDBContext.SalesRequests.FirstOrDefaultAsync(x => x.Id == fileName);
            if(salesRequest != null)
            {
                salesRequest.Status = "Image Processed";
                _azureSalesDBContext.SalesRequests.Update(salesRequest);
                await _azureSalesDBContext.SaveChangesAsync();
            }
        }
    }
}
