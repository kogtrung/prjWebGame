using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebGame.Data;
using WebGame.Models;
using WebGame.Services;

namespace WebGame.Controllers
{
    public class ReleasesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReleasesController> _logger;
        private readonly IGameImageService _gameImageService;

        public ReleasesController(ApplicationDbContext context, ILogger<ReleasesController> logger, IGameImageService gameImageService)
        {
            _context = context;
            _logger = logger;
            _gameImageService = gameImageService;
        }

        // GET: /Releases/
        public async Task<IActionResult> Index()
        {
            return await NewReleases();
        }
        
        // GET: /Releases/NewReleases
        public async Task<IActionResult> NewReleases()
        {
            try
            {
                await EnsureDatabaseStructure();

                // Load all games with their platforms
                var allGames = await _context.Games
                    .Include(g => g.GamePlatforms)
                    .ThenInclude(gp => gp.Platform)
                    .ToListAsync();
                    
                // Filter for released games in memory
                var games = allGames
                    .Where(g => g.ReleaseDate.HasValue && g.ReleaseDate.Value <= DateTime.Now)
                        .OrderByDescending(g => g.ReleaseDate)
                    .Take(12)
                    .ToList();

                // If we don't have enough games, use sample data
                if (games.Count < 8)
                {
                    games = GetSampleNewReleases();
                }

                // Get all platforms for filter dropdown
                var platforms = await _context.Platforms
                    .OrderBy(p => p.Name)
                    .Select(p => p.Name)
                    .ToListAsync();
                ViewBag.Platforms = platforms;

                // Process genres in memory
                var uniqueGenres = new List<string>();
                foreach (var game in allGames)
                {
                    if (!string.IsNullOrEmpty(game.Genre))
                    {
                        string[] parts = game.Genre.Split(',');
                        foreach (var part in parts)
                        {
                            string trimmed = part.Trim();
                            if (!string.IsNullOrEmpty(trimmed) && !uniqueGenres.Contains(trimmed))
                            {
                                uniqueGenres.Add(trimmed);
                            }
                        }
                    }
                }
                uniqueGenres.Sort();
                ViewBag.Genres = uniqueGenres;

                // Group games by month
                var groupedGames = new Dictionary<DateTime, List<Game>>();
                var groupedByYearMonth = games.GroupBy(g => 
                    new { 
                        Month = g.ReleaseDate?.Month ?? DateTime.Now.Month, 
                        Year = g.ReleaseDate?.Year ?? DateTime.Now.Year 
                    }).ToList();
                    
                foreach (var group in groupedByYearMonth.OrderByDescending(g => g.Key.Year).ThenByDescending(g => g.Key.Month))
                {
                    var key = new DateTime(group.Key.Year, group.Key.Month, 1);
                    groupedGames[key] = group.ToList();
                }

                // Ensure all games have images
                AssignImages(games);
                
                // Ensure all games have proper platform information
                AssignPlatforms(games);

                return View("NewReleases", groupedGames);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading new releases");
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    Message = "There was an error loading the new releases. Please try again later."
                });
            }
        }

        private List<Game> GetSampleNewReleases()
        {
            var today = DateTime.Today;
            
            return new List<Game>
            {
                new Game
                {
                    Id = -20001,
                    Title = "Helldivers 2",
                    Description = "Join Astro and his crew in an out-of-this-world 3D platforming adventure across the cosmos to rescue his lost crewmates.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202401/1621/a14f1805e2144f6c1d5663517146c57d8bc5694e69ef0f14.jpg",
                    VideoUrl = "https://www.youtube.com/embed/yxUA0Ruv6Vs",
                    TrailerUrl = "https://www.youtube.com/embed/yxUA0Ruv6Vs",
                    MetaScore = 95,
                    ReleaseDate = today.AddDays(-15),
                    Platform = "PC, PS5",
                    Genre = "Action, Shooter",
                    Developer = "Arrowhead Game Studios",
                    Publisher = "Sony Interactive Entertainment",
                    Rating = "M"
                },
                new Game
                {
                    Id = -20002,
                    Title = "Tekken 8",
                    Description = "The next chapter in the legendary fighting game franchise brings unparalleled graphics and new fighting mechanics.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202306/1219/60eca3ac155247e21850c7d075d01ebf0f2c35df65b85480.jpg",
                    VideoUrl = "https://www.youtube.com/embed/2hPuRQz6IlM",
                    TrailerUrl = "https://www.youtube.com/embed/2hPuRQz6IlM",
                    MetaScore = 88,
                    ReleaseDate = today.AddDays(-20),
                    Platform = "PC, PS5, Xbox Series X",
                    Genre = "Fighting",
                    Developer = "Bandai Namco",
                    Publisher = "Bandai Namco",
                    Rating = "T"
                },
                new Game
                {
                    Id = -20003,
                    Title = "Like a Dragon: Infinite Wealth",
                    Description = "Experience an epic RPG adventure across Japan and Hawaii with Ichiban Kasuga and Kazuma Kiryu.",
                    ImageUrl = "https://assets.nintendo.com/image/upload/c_fill,w_1200/q_auto:best/f_auto/dpr_2.0/ncom/software/switch/70010000063714/276a412988e07c4d55a2996c6d38abb408b464413fb629888f2ec4d1fa628361",
                    VideoUrl = "https://www.youtube.com/embed/iw0_g0mHE08",
                    TrailerUrl = "https://www.youtube.com/embed/iw0_g0mHE08",
                    MetaScore = 89,
                    ReleaseDate = today.AddDays(-25),
                    Platform = "PC, PS5, Xbox Series X",
                    Genre = "RPG, Action",
                    Developer = "Ryu Ga Gotoku Studio",
                    Publisher = "Sega",
                    Rating = "M"
                },
                new Game
                {
                    Id = -20004,
                    Title = "Prince of Persia: The Lost Crown",
                    Description = "A fresh take on the iconic franchise featuring a new hero and stunning 2.5D Metroidvania gameplay.",
                    ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/2344840/header.jpg",
                    VideoUrl = "https://www.youtube.com/embed/Qdgx8ExDzXk",
                    TrailerUrl = "https://www.youtube.com/embed/Qdgx8ExDzXk",
                    MetaScore = 89,
                    ReleaseDate = today.AddDays(-10),
                    Platform = "PC, PS5, Xbox Series X, Nintendo Switch",
                    Genre = "Action, Adventure",
                    Developer = "Ubisoft Montpellier",
                    Publisher = "Ubisoft",
                    Rating = "T"
                },
                new Game
                {
                    Id = -20005,
                    Title = "Granblue Fantasy: Relink",
                    Description = "An action RPG set in the sky-faring world of Granblue Fantasy with stunning visuals and deep combat.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202306/0116/80a373a2d1c2fc7d8f4af91906c1c213c76604fe3d38bf06.jpg",
                    VideoUrl = "https://www.youtube.com/embed/1tJb8yaDcNI",
                    TrailerUrl = "https://www.youtube.com/embed/1tJb8yaDcNI",
                    MetaScore = 87,
                    ReleaseDate = today.AddDays(-5),
                    Platform = "PC, PS5",
                    Genre = "Action RPG",
                    Developer = "Cygames",
                    Publisher = "Cygames",
                    Rating = "T"
                }
            };
        }

        private async Task EnsureDatabaseStructure()
        {
            if (!_context.Platforms.Any())
            {
                // Add basic platforms if none exist
                _context.Platforms.AddRange(
                    new Platform { Name = "PC" },
                    new Platform { Name = "PlayStation 5" },
                    new Platform { Name = "PlayStation 4" },
                    new Platform { Name = "Xbox Series X" },
                    new Platform { Name = "Xbox One" },
                    new Platform { Name = "Nintendo Switch" }
                );
                await _context.SaveChangesAsync();
            }
        }

        private void AssignImages(List<Game> games)
        {
            foreach (var game in games)
            {
                if (string.IsNullOrEmpty(game.ImageUrl))
                {
                    game.ImageUrl = _gameImageService.GetRandomGameImage();
                }
            }
        }

        private void AssignPlatforms(List<Game> games)
        {
            foreach (var game in games)
            {
                if (game.GamePlatforms == null || !game.GamePlatforms.Any())
                {
                    if (!string.IsNullOrEmpty(game.Platform))
                    {
                        var platformNames = game.Platform.Split(',').Select(p => p.Trim()).ToList();
                        game.GamePlatforms = new List<GamePlatform>();
                        
                        foreach (var platformName in platformNames)
                        {
                            var platform = _context.Platforms.FirstOrDefault(p => p.Name.Contains(platformName));
                            if (platform != null)
                            {
                                game.GamePlatforms.Add(new GamePlatform
                                {
                                    Game = game,
                                    Platform = platform,
                                    GameId = game.Id,
                                    PlatformId = platform.Id
                                });
                            }
                        }
                    }
                }
            }
        }
    }
} 