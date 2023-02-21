using System;
using System.Linq;
using System.Threading.Tasks;
using AzureFuncHttpReq.Data;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AzureFuncHttpReq
{
    public class OnTriggerUpdateStatusToCompleteInDb
    {
        private readonly AzureSalesDBContext _db;

        public OnTriggerUpdateStatusToCompleteInDb(AzureSalesDBContext db)
        {
            _db = db;
        }

        [FunctionName("OnTriggerUpdateStatusToCompleteInDb")]
        public async Task Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var salesRequests = _db.SalesRequests.Where(x => x.Status == "Image Processed").ToList();
            foreach(var salesRequest in salesRequests )
            {
                salesRequest.Status = "Completed";                
            }

            _db.UpdateRange(salesRequests);
            await _db.SaveChangesAsync();
        }
    }
}
