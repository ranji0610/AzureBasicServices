using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureFunction.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace AzureFunction.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        static readonly HttpClient _httpClient = new HttpClient();
        private readonly BlobServiceClient _blobServiceClient;

        public HomeController(ILogger<HomeController> logger, BlobServiceClient blobServiceClient)
        {
            _logger = logger;
            _blobServiceClient = blobServiceClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(SalesRequest salesRequest, IFormFile formFile)
        {
            salesRequest.Id = Guid.NewGuid().ToString();
            using (var content = new StringContent(JsonConvert.SerializeObject(salesRequest), System.Text.Encoding.UTF8, "application/json"))
            {
                HttpResponseMessage responseMessage = await _httpClient.PostAsync("http://localhost:7239/api/OnSalesUpload", content);
                var response = await responseMessage.Content.ReadAsStringAsync();
            };

            if(formFile != null)
            {
                var fileName = salesRequest.Id + Path.GetExtension(formFile.FileName).ToLower();
                var blobContainerClient = _blobServiceClient.GetBlobContainerClient("functionsalesrep");
                var blobClient = blobContainerClient.GetBlobClient(fileName);
                var httpHeaders = new BlobHttpHeaders()
                {
                    ContentType = formFile.ContentType
                };

                await blobClient.UploadAsync(formFile.OpenReadStream(), httpHeaders);                
            }

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}