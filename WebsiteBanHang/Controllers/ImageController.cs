using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using WebGame.Services;
using Microsoft.AspNetCore.Hosting;

namespace WebGame.Controllers
{
    public class ImageController : Controller
    {
        private readonly ILogger<ImageController> _logger;
        private readonly IGameImageService _gameImageService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImageController(ILogger<ImageController> logger, IGameImageService gameImageService, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _gameImageService = gameImageService;
            _webHostEnvironment = webHostEnvironment;
        }
        
        [HttpGet("images/games/{filename}")]
        public IActionResult GetGameImage(string filename)
        {
            try
            {
                // Check if file exists in wwwroot/images/games directory
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "games", filename);
                
                if (System.IO.File.Exists(imagePath))
                {
                    // Return the physical file
                    return PhysicalFile(imagePath, "image/jpeg");
                }
                
                // If not found, return a placeholder image
                string placeholderPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "placeholder.jpg");
                
                if (System.IO.File.Exists(placeholderPath))
                {
                    return PhysicalFile(placeholderPath, "image/jpeg");
                }
                
                // If placeholder not found, return a default image from a CDN
                return Redirect("https://via.placeholder.com/600x400?text=Game+Image+Not+Found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving game image: {Filename}", filename);
                return Redirect("https://via.placeholder.com/600x400?text=Error");
            }
        }
        
        [HttpGet("api/images/refresh")]
        public async Task<IActionResult> RefreshGameImages()
        {
            try
            {
                bool result = await _gameImageService.RefreshGameImagesAsync();
                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing game images");
                return StatusCode(500, new { error = "Failed to refresh game images" });
            }
        }
    }
} 