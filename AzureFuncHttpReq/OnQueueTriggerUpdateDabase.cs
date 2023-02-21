using AzureFuncHttpReq.Data;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AzureFuncHttpReq
{
    public class OnQueueTriggerUpdateDabase
    {
        private readonly AzureSalesDBContext _db;

        public OnQueueTriggerUpdateDabase(AzureSalesDBContext db)
        {
            _db = db;
        }

        [FunctionName("OnQueueTriggerUpdateDabase")]
        public void Run([QueueTrigger("salesrequestinbound", Connection = "AzureWebJobsStorage")] SalesRequest myQueueItem, ILogger log)
        {
            log.LogInformation($" OnQueueTriggerUpdateDabase trigger function processed: {myQueueItem}");
            myQueueItem.Status = "Submitted";
            _db.SalesRequests.Add(myQueueItem);
            _db.SaveChanges();
        }
    }
}
