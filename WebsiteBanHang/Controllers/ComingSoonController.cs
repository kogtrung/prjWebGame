using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebGame.Data;
using WebGame.Models;
using WebGame.Services;

namespace WebGame.Controllers
{
    public class ComingSoonController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ComingSoonController> _logger;
        private readonly IGameImageService _gameImageService;

        public ComingSoonController(
            ApplicationDbContext context,
            ILogger<ComingSoonController> logger,
            IGameImageService gameImageService)
        {
            _context = context;
            _logger = logger;
            _gameImageService = gameImageService;
        }

        // GET: /ComingSoon
        public async Task<IActionResult> Index()
        {
            try
            {
                var today = DateTime.Now;
                
                // Get upcoming games from the database
                var upcomingGames = await _context.Games
                    .Include(g => g.GamePlatforms)
                    .ThenInclude(gp => gp.Platform)
                    .Where(g => g.ReleaseDate.HasValue && g.ReleaseDate.Value > today)
                    .OrderBy(g => g.ReleaseDate)
                    .ToListAsync();
                
                // If no upcoming games found in database, use sample data
                if (upcomingGames == null || !upcomingGames.Any())
                {
                    _logger.LogInformation("No upcoming games found in database, using sample data");
                    upcomingGames = GetSampleUpcomingGames();
                }
                
                // Ensure games have images and platform information
                AssignImages(upcomingGames);
                AssignPlatforms(upcomingGames);
                
                // Group games by month and year
                var groupedGames = upcomingGames
                    .Where(g => g.ReleaseDate.HasValue)
                    .GroupBy(g => new { 
                        Year = g.ReleaseDate.Value.Year,
                        Month = g.ReleaseDate.Value.Month
                    })
                    .OrderBy(g => g.Key.Year)
                    .ThenBy(g => g.Key.Month)
                    .ToDictionary(
                        g => (object)new DateTime(g.Key.Year, g.Key.Month, 1),
                        g => g.OrderBy(game => game.ReleaseDate).ToList()
                    );

                return View(groupedGames);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ComingSoon Index action");
                return View("Error", new ErrorViewModel
                {
                    RequestId = HttpContext.TraceIdentifier,
                    Message = "An error occurred while loading upcoming games. Please try again later."
                });
            }
        }

        private List<Game> GetSampleUpcomingGames()
        {
            var games = new List<Game>();

            // Ensure all games have images
            foreach (var game in games)
            {
                if (string.IsNullOrEmpty(game.ImageUrl))
                {
                    game.ImageUrl = _gameImageService.GetImageUrlByTitle(game.Title);
                }
            }

            return games;
        }

        private void AssignImages(List<Game> games)
        {
            foreach (var game in games.Where(g => string.IsNullOrEmpty(g.ImageUrl)))
            {
                game.ImageUrl = _gameImageService.GetImageUrlByTitle(game.Title);
            }
        }

        private void AssignPlatforms(List<Game> games)
        {
            foreach (var game in games)
            {
                if (game.GamePlatforms == null || !game.GamePlatforms.Any())
                {
                    // If no platforms are set, create a default set based on the Platform string
                    var platformNames = (game.Platform ?? "PC").Split(',').Select(p => p.Trim()).ToList();
                    game.GamePlatforms = platformNames.Select(name => new GamePlatform
                    {
                        Game = game,
                        Platform = new Platform { Name = name }
                    }).ToList();
                }
            }
        }
    }
} 