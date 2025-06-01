using System.Diagnostics;
using DeployGithubAsp.netCore.Models;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace DeployGithubAsp.netCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                ViewBag.Error = "No file selected.";
                return View();
            }

            var ext = Path.GetExtension(imageFile.FileName).ToLower();
            if (ext != ".tiff" && ext != ".img")
            {
                ViewBag.Error = "Only .TIFF or .IMG files are allowed.";
                return View();
            }

            // ✅ Set direct save folder on D:\BK
            var saveFolder = @"D:\BK";
            Directory.CreateDirectory(saveFolder);

            // File paths
            var originalPath = Path.Combine(saveFolder, Guid.NewGuid() + ext);
            var jpgPath = Path.ChangeExtension(originalPath, ".jpg");

            // Save uploaded file
            using (var stream = new FileStream(originalPath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Convert to JPG
            using (var image = await Image.LoadAsync(originalPath))
            {
                image.Mutate(x => x.AutoOrient());
                await image.SaveAsJpegAsync(jpgPath, new JpegEncoder { Quality = 90 });
            }

            ViewBag.ConvertedImagePath = jpgPath; // Optional: show path
            return View();
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
