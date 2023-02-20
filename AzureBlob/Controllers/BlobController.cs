using AzureBlob.Models;
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
        public IActionResult AddFile(string containerName)
        {            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddFile(IFormFile file, string containerName, Blob blob)
        {
            var fileName = Path.GetFileNameWithoutExtension(file.Name) + "_" + Guid.NewGuid() + Path.GetExtension(file.FileName);
            await _blobService.UploadBlob(fileName, file, containerName, blob);
            return RedirectToAction("Index","Container");
        }

        public async Task<IActionResult> ViewFile(string name, string containerName)
        {
            return Redirect(await _blobService.GetBlob(name, containerName));
        }

        public async Task<IActionResult> DeleteFile(string name, string containerName)
        {
            await _blobService.DeleteBlob(name, containerName);
            return RedirectToAction("Index", "Container");
        }
    }
}
