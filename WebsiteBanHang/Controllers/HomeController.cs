using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebGame.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebGame.Data;
using WebGame.Services;

namespace WebGame.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IGameImageService _gameImageService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IGameImageService gameImageService)
        {
            _logger = logger;
            _context = context;
            _gameImageService = gameImageService;
        }

        private void UpdateDatabaseSchema()
        {
            try
            {
                if (_context?.Database != null)
                {
                    // Ensure database exists
                    _context.Database.EnsureCreated();
                    
                    // Check if Games table is empty
                    if (!_context.Games.Any())
                    {
                        // Add sample games
                        var sampleGames = CreateSampleGames();
                        _context.Games.AddRange(sampleGames);
                        _context.SaveChanges();
                        
                        _logger.LogInformation("Added sample games to database");
                    }
                    
                    _logger.LogInformation("Database and tables are ready");
                }
                else
                {
                    _logger.LogWarning("Database context is null");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing database");
            }
        }

        public IActionResult Games()
        {
            try
            {
                // Cập nhật database trước
                UpdateDatabaseSchema();
                
                // Sử dụng FirstOrDefault() thay vì ToList() để tránh lỗi nếu database không tồn tại
                var games = _context.Games?
                    .Include(g => g.GamePlatforms)
                    .ThenInclude(gp => gp.Platform)
                    .OrderByDescending(g => g.ReleaseDate)
                    .ToList() ?? new List<Game>();
                    
                return View(games);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Games action");
                ViewBag.ErrorMessage = "Có lỗi trong việc tải dữ liệu. Vui lòng thử lại sau.";
                return View(new List<Game>());
            }
        }

        public IActionResult News()
        {
            try
            {
                // Cập nhật database trước
                UpdateDatabaseSchema();
                
                // Sử dụng FirstOrDefault() thay vì ToList() để tránh lỗi nếu database không tồn tại
                var news = _context.NewsPosts?
                    .OrderByDescending(n => n.CreatedAt)
                    .ToList() ?? new List<NewsPost>();
                    
                return View(news);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in News action");
                ViewBag.ErrorMessage = "Có lỗi trong việc tải dữ liệu. Vui lòng thử lại sau.";
                return View(new List<NewsPost>());
            }
        }

        public IActionResult BrowseGames(string platform = null, string genre = null, int? year = null)
        {
            try
            {
                if (_context?.Games == null)
                {
                    _logger.LogError("Games DbSet is null");
                    return View(new List<Game>());
                }
                
                // Create a base query without using Platform in filtering
                var query = _context.Games.AsQueryable();

                // Apply genre filter if provided
                if (!string.IsNullOrEmpty(genre))
                {
                    query = query.Where(g => g.Genre.Contains(genre));
                }

                // Apply year filter if provided
                if (year.HasValue)
                {
                    query = query.Where(g => g.ReleaseDate.HasValue && g.ReleaseDate.Value.Year == year.Value);
                }

                // Handle platform filter separately
                List<Game> games;
                if (!string.IsNullOrEmpty(platform))
                {
                    // Get game IDs with the platform
                    var gameIdsWithPlatform = _context.GamePlatforms
                        .Include(gp => gp.Platform)
                        .Where(gp => gp.Platform.Name.Contains(platform))
                        .Select(gp => gp.GameId)
                        .Distinct()
                        .ToList();
                        
                    // Apply the filter
                    query = query.Where(g => gameIdsWithPlatform.Contains(g.Id));
                }
                
                // Execute the query with platforms included
                games = query
                    .OrderByDescending(g => g.MetaScore)
                    .Include(g => g.GamePlatforms)
                    .ThenInclude(gp => gp.Platform)
                    .ToList();

                return View(games);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in BrowseGames action");
                ViewBag.ErrorMessage = "An error occurred while loading the games. Please try again later.";
                return View(new List<Game>());
            }
        }

        public async Task<IActionResult> Search(string q)
        {
            try
            {
                if (string.IsNullOrEmpty(q))
                {
                    return RedirectToAction("Index");
                }

                if (_context?.Games == null || _context?.NewsPosts == null)
                {
                    _logger.LogError("Games or NewsPosts DbSet is null");
                    ViewBag.SearchTerm = q;
                    ViewBag.NewsResults = new List<NewsPost>();
                    ViewBag.ResultCount = 0;
                    return View(new List<Game>());
                }

                // Two-step approach to avoid Platform property in the query
                // First, get game IDs by searching on basic properties
                var gameIds = await _context.Games
                    .Where(g => g.Title.Contains(q) || 
                           g.Description.Contains(q) || 
                           g.Genre.Contains(q) || 
                           g.Developer.Contains(q) || 
                           g.Publisher.Contains(q))
                    .Select(g => g.Id)
                    .ToListAsync();

                // Second, search by platform and add those IDs
                var platformGameIds = await _context.GamePlatforms
                    .Include(gp => gp.Platform)
                    .Where(gp => gp.Platform.Name.Contains(q))
                    .Select(gp => gp.GameId)
                    .ToListAsync();
                    
                // Combine the IDs
                gameIds = gameIds.Union(platformGameIds).ToList();
                
                // Load games with their platforms
                var games = await _context.Games
                    .Where(g => gameIds.Contains(g.Id))
                    .Include(g => g.GamePlatforms)
                    .ThenInclude(gp => gp.Platform)
                    .OrderByDescending(g => g.MetaScore)
                    .ToListAsync();

                // Search news as before
                var news = await _context.NewsPosts
                    .Where(n => n.Title.Contains(q) || 
                          n.Content.Contains(q) ||
                          (n.Category != null && n.Category.Contains(q)))
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(5)
                    .ToListAsync();

                ViewBag.SearchTerm = q;
                ViewBag.NewsResults = news;
                ViewBag.ResultCount = games.Count + news.Count;

                return View(games);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Search action");
                ViewBag.SearchTerm = q;
                ViewBag.NewsResults = new List<NewsPost>();
                ViewBag.ErrorMessage = "An error occurred during search. Please try again later.";
                return View(new List<Game>());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult UpdateDatabase()
        {
            try
            {
                // Chỉ đảm bảo kết nối hoạt động
                if (_context?.Database != null)
                {
                    // Nếu tới được đây, database có vẻ ổn định nhưng không thực hiện thêm gì
                    return Content("Database connection established. To add required columns, please run database migrations.");
                }
                else
                {
                    return Content("Error: Database context is null");
                }
            }
            catch (Exception ex)
            {
                return Content($"Error checking database: {ex.Message}");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Phương thức để gán hình ảnh từ dịch vụ
        private void AssignGameImages(List<Game> games)
        {
            if (games == null || !games.Any())
                return;

            foreach (var game in games)
            {
                try
                {
                    if (string.IsNullOrEmpty(game.ImageUrl))
                    {
                        // Try to get image URL from service first
                        string imageUrl = _gameImageService.GetImageUrlByTitle(game.Title);
                        
                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            game.ImageUrl = imageUrl;
                            _logger.LogInformation($"Assigned image from service to {game.Title}: {imageUrl}");
                        }
                        else
                        {
                            // If no image found from service, use a default image based on game title
                            string defaultImage = GetDefaultImageForGame(game.Title);
                            game.ImageUrl = defaultImage;
                            _logger.LogInformation($"Assigned default image to {game.Title}: {defaultImage}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error assigning image to game {game.Title}");
                    game.ImageUrl = "/images/default-game.jpg";
                }
            }
        }

        private string GetDefaultImageForGame(string gameTitle)
        {
            // Map of game titles to their default image URLs
            var defaultImages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Helldivers 2", "https://cdn.akamai.steamstatic.com/steam/apps/553850/header.jpg" },
                { "Stellar Blade", "https://image.api.playstation.com/vulcan/ap/rnd/202309/2022/49a52576e9d3011dac9884c87d4c2ebc8ce08f7cde9d3c07.png" },
                { "The Legend of Zelda: Tears of the Kingdom", "https://assets.nintendo.com/image/upload/c_fill,w_1200/q_auto:best/f_auto/dpr_2.0/ncom/software/switch/70010000063715/276a412988e07c4d55a2996c6d38abb408b464413b2dfeb44d2aa460b9f622e1" },
                { "Final Fantasy VII Rebirth", "https://image.api.playstation.com/vulcan/ap/rnd/202306/1219/aeaf61c83ecc2eec23f2c101b28814c864e4f66fe6e49d0a.png" },
                { "Black Myth: Wukong", "https://cdn.cloudflare.steamstatic.com/steam/apps/2358720/header.jpg" },
                { "Hogwarts Legacy", "https://cdn.akamai.steamstatic.com/steam/apps/990080/header.jpg" },
                { "Like a Dragon: Infinite Wealth", "https://cdn.akamai.steamstatic.com/steam/apps/1899880/header.jpg" }
            };

            return defaultImages.TryGetValue(gameTitle, out string imageUrl) 
                ? imageUrl 
                : "/images/default-game.jpg";
        }

        public IActionResult Index()
        {
            try
            {
                // Get featured games without using Platform property in the LINQ query
                var gameIds = _context.Games
                    .OrderByDescending(g => g.MetaScore)
                    .Take(12)
                    .Select(g => g.Id)
                    .ToList();
                    
                // Now load the games with platforms
                var featuredGames = _context.Games
                    .Where(g => gameIds.Contains(g.Id))
                    .Include(g => g.GamePlatforms)
                    .ThenInclude(gp => gp.Platform)
                    .OrderByDescending(g => g.MetaScore)
                    .ToList();
                
                // Check if we have any upcoming games in the database
                var upcomingGames = _context.Games
                    .Where(g => g.ReleaseDate.HasValue && g.ReleaseDate.Value > DateTime.Today)
                    .Include(g => g.GamePlatforms)
                    .ThenInclude(gp => gp.Platform)
                    .OrderBy(g => g.ReleaseDate)
                    .Take(5)
                    .ToList();
                
                // If no upcoming games in database, add sample upcoming games
                if (!upcomingGames.Any())
                {
                    // Get sample upcoming games 
                    var sampleGames = GetSampleUpcomingGames();
                    
                    // Add these to our featured games list
                    featuredGames.AddRange(sampleGames);
                }
                else
                {
                    // Add database upcoming games to the featured games if not already included
                    foreach (var game in upcomingGames)
                    {
                        if (!featuredGames.Any(g => g.Id == game.Id))
                        {
                            featuredGames.Add(game);
                        }
                    }
                }

                // Ensure images are assigned
                AssignImages(featuredGames);
                
                return View(featuredGames);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Index action");
                ViewBag.ErrorMessage = "An error occurred while loading data. Please try again later.";
                return View(new List<Game>());
            }
        }

        private void AssignImages(List<Game> games)
        {
            foreach (var game in games)
            {
                if (string.IsNullOrEmpty(game.ImageUrl))
                {
                    // Sử dụng dịch vụ ảnh để lấy URL hình ảnh theo tên game
                    game.ImageUrl = _gameImageService.GetImageUrlByTitle(game.Title);
                }
            }
        }

        private List<Game> CreateSampleGames()
        {
            return new List<Game>
            {
                new Game
                {
                    Id = 1,
                    Title = "Baldur's Gate 3",
                    Description = "A party-based RPG set in the Dungeons & Dragons universe, featuring deep character customization, a rich story, and innovative turn-based combat.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202302/2321/2fa247a12223368ad30db6b6dedcede2b23fc8dbb92575.jpg",
                    MetaScore = 97,
                    ReleaseDate = new DateTime(2023, 8, 3),
                    Platform = "PC, PlayStation 5",
                    Genre = "RPG",
                    Developer = "Larian Studios",
                    Publisher = "Larian Studios"
                },
                new Game
                {
                    Id = 2,
                    Title = "The Legend of Zelda: Tears of the Kingdom",
                    Description = "The sequel to Breath of the Wild takes Link's adventure to new heights in the skies above Hyrule, introducing innovative gameplay mechanics and a vast new world to explore.",
                    ImageUrl = "https://assets.nintendo.com/image/upload/ar_16:9,c_lpad,w_1240/b_white/f_auto/q_auto/ncom/software/switch/70010000063714/276a412988e07c4d55a2996c6d38abb408b464413fb629888f2ec4d1fa628361",
                    MetaScore = 96,
                    ReleaseDate = new DateTime(2023, 5, 12),
                    Platform = "Nintendo Switch",
                    Genre = "Action, Adventure",
                    Developer = "Nintendo",
                    Publisher = "Nintendo"
                },
                new Game
                {
                    Id = 3,
                    Title = "Elden Ring",
                    Description = "A dark fantasy action RPG created by FromSoftware and George R.R. Martin, featuring a vast open world filled with danger, discovery, and challenging combat.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202108/0410/0Imhn60XcUSstCELZQE7FTxW.png",
                    MetaScore = 96,
                    ReleaseDate = new DateTime(2022, 2, 25),
                    Platform = "PC, PlayStation 4, PlayStation 5, Xbox One, Xbox Series X/S",
                    Genre = "Action RPG",
                    Developer = "FromSoftware",
                    Publisher = "Bandai Namco Entertainment"
                },
                new Game
                {
                    Id = 4,
                    Title = "God of War Ragnarök",
                    Description = "The epic conclusion to Kratos and Atreus's Norse saga, as they face the coming of Ragnarök and confront powerful enemies including Thor and Freya.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202207/1210/4xJ8XB3bi888QTLZYdl7Oi0s.png",
                    MetaScore = 94,
                    ReleaseDate = new DateTime(2022, 11, 9),
                    Platform = "PlayStation 4, PlayStation 5",
                    Genre = "Action-Adventure",
                    Developer = "Santa Monica Studio",
                    Publisher = "Sony Interactive Entertainment"
                },
                new Game
                {
                    Id = 5,
                    Title = "Final Fantasy VII Rebirth",
                    Description = "The second chapter in the Final Fantasy VII remake project, continuing Cloud and his allies' journey as they pursue Sephiroth and uncover the truth about their past.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202306/0215/d43071186e987e66d3ed258dd57ac05fcc1a1cfd98bed3cd.jpg",
                    MetaScore = 93,
                    ReleaseDate = new DateTime(2024, 2, 29),
                    Platform = "PlayStation 5",
                    Genre = "RPG",
                    Developer = "Square Enix",
                    Publisher = "Square Enix"
                },
                new Game
                {
                    Id = 6,
                    Title = "Helldivers 2",
                    Description = "Helldivers 2 is a thrilling cooperative third-person shooter where players fight to protect Super Earth from alien threats across a hostile galaxy.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202211/1117/M8MVwoME0926k9TUlD7jrBrj.jpg",
                    MetaScore = 92,
                    ReleaseDate = new DateTime(2024, 2, 8),
                    Platform = "PC, PlayStation 5",
                    Genre = "Action, Third-Person Shooter",
                    Developer = "Arrowhead Game Studios",
                    Publisher = "PlayStation Studios"
                },
                new Game
                {
                    Id = 7,
                    Title = "Spider-Man 2",
                    Description = "Peter Parker and Miles Morales return in this sequel to Marvel's Spider-Man, facing new threats including Venom and Kraven the Hunter in an expanded New York City.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202306/1219/1c7b42e972197abf134325730347f5bc823f2867818a16bc.jpg",
                    MetaScore = 91,
                    ReleaseDate = new DateTime(2023, 10, 20),
                    Platform = "PlayStation 5",
                    Genre = "Action-Adventure",
                    Developer = "Insomniac Games",
                    Publisher = "Sony Interactive Entertainment"
                },
                new Game
                {
                    Id = 8,
                    Title = "Stellar Blade",
                    Description = "A stylish action-adventure game that follows Eve, a warrior on a mission to reclaim Earth from an invading alien force known as the NA:tives.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202401/1617/9d09996e978bbf63ff00b8f7d36be61eb0a61bc0c95a9066.jpg",
                    MetaScore = 88,
                    ReleaseDate = new DateTime(2024, 4, 26),
                    Platform = "PlayStation 5",
                    Genre = "Action, Adventure",
                    Developer = "SHIFT UP",
                    Publisher = "Sony Interactive Entertainment"
                },
                new Game
                {
                    Id = 9,
                    Title = "Red Dead Redemption 2",
                    Description = "An epic tale of life in America's unforgiving heartland, featuring a vast and atmospheric world with the freedom to go where you want and do what you want.",
                    ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/1174180/header.jpg",
                    MetaScore = 97,
                    ReleaseDate = new DateTime(2018, 10, 26),
                    Platform = "PC, PlayStation 4, Xbox One",
                    Genre = "Action-Adventure",
                    Developer = "Rockstar Games",
                    Publisher = "Rockstar Games"
                },
                new Game
                {
                    Id = 10,
                    Title = "The Witcher 3: Wild Hunt",
                    Description = "An action RPG set in a rich, fantasy open world featuring monster slayer Geralt of Rivia, with a rich narrative and engaging combat.",
                    ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/292030/header.jpg",
                    MetaScore = 93,
                    ReleaseDate = new DateTime(2015, 5, 19),
                    Platform = "PC, PlayStation 4, PlayStation 5, Xbox One, Xbox Series X/S, Nintendo Switch",
                    Genre = "RPG",
                    Developer = "CD Projekt Red",
                    Publisher = "CD Projekt"
                },
                new Game
                {
                    Id = 11,
                    Title = "Cyberpunk 2077",
                    Description = "An open-world, action-adventure RPG set in Night City, a megalopolis obsessed with power, glamour, and body modification.",
                    ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/1091500/header.jpg",
                    MetaScore = 86,
                    ReleaseDate = new DateTime(2020, 12, 10),
                    Platform = "PC, PlayStation 4, PlayStation 5, Xbox One, Xbox Series X/S",
                    Genre = "RPG",
                    Developer = "CD Projekt Red",
                    Publisher = "CD Projekt"
                },
                new Game
                {
                    Id = 12,
                    Title = "Hades",
                    Description = "A roguelike dungeon crawler where you defy the god of the dead as you hack and slash your way out of the Underworld.",
                    ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/1145360/header.jpg",
                    MetaScore = 93,
                    ReleaseDate = new DateTime(2020, 9, 17),
                    Platform = "PC, PlayStation 4, PlayStation 5, Xbox One, Xbox Series X/S, Nintendo Switch",
                    Genre = "Action, Roguelike",
                    Developer = "Supergiant Games",
                    Publisher = "Supergiant Games"
                }
            };
        }

        private List<Game> GetSampleUpcomingGames()
        {
            return new List<Game>
            {
                
            };
        }
    }
}
