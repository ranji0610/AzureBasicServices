using AzureBlob.Services;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlob.Controllers
{
    public class BlobController : Controller
    {
        private readonly IBlobService _blobService;

        public BlobController(IBlobService blobService)
        {
            _blobService = blobService;
        }

        [HttpGet]
        public async Task<IActionResult> Manage(string containerName)
        {
            var blobsObj = await _blobService.GetAllBlobs(containerName);
            return View(blobsObj);
        }

        [HttpGet]
        public IActionResult AddFile()
        {            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddFile(IFormFile file)
        {
            var fileName = Path.GetFileNameWithoutExtension(file.Name) + "_" + Guid.NewGuid() + Path.GetExtension(file.FileName);
            await _blobService.UploadBlob(fileName, file, "images");
            return RedirectToAction("Manage");
        }
    }
}
