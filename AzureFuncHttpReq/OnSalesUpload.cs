using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace AzureFuncHttpReq
{
    public static class OnSalesUpload
    {
        [FunctionName("OnSalesUpload")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [Queue("SalesRequestInBound", Connection = "AzureWebJobsStorage")] IAsyncCollector<SalesRequest> salesRequestQueue,
            ILogger log)
        {
            log.LogInformation("Sales Request received.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<SalesRequest>(requestBody);
            await salesRequestQueue.AddAsync(data);
            string responseMessage = "Sales request uploaded to queue for :" + data.Name;
            return new OkObjectResult(responseMessage);
        }
    }
}
