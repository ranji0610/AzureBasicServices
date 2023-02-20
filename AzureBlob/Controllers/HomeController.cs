using AzureBlob.Models;
using AzureBlob.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AzureBlob.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IContainerService _containerService;
        private readonly IBlobService _blobService;

        public HomeController(ILogger<HomeController> logger, IContainerService containerService, IBlobService blobService)
        {
            _logger = logger;
            _containerService = containerService;
            _blobService = blobService;
        }

        public async Task<IActionResult> Index()
        {
            var info = await _containerService.GetAllContainerAndBlobs();
            return View(info);
        }

        public async Task<IActionResult> Images()
        {            
            return View(await _blobService.GetAllBlobsWithUrl("employee-images"));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}