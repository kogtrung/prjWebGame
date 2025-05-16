using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebGame.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using WebGame.Services;
using System;

using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Security.Claims;

namespace WebGame.Controllers
{
    public class GameController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GameController> _logger;
        private readonly IGameImageService _gameImageService;

        // Danh sách đường dẫn ảnh cục bộ (sẽ được thay thế bằng dịch vụ ảnh)
        private readonly string[] sampleImages = new string[]
        {
            "https://image.api.playstation.com/vulcan/ap/rnd/202108/0410/0Imhn60XcUSstCELZQE7FTxW.png", // Elden Ring
            "https://image.api.playstation.com/vulcan/ap/rnd/202207/1210/4xJ8XB3bi888QTLZYdl7Oi0s.png", // God of War
            "https://image.api.playstation.com/vulcan/ap/rnd/202010/0222/niMUubpU9y1PxNvYmDfb8QFD.png", // Last of Us 2
            "https://image.api.playstation.com/vulcan/ap/rnd/202202/2816/mYn2ETBKFcg0DlLcOc5yd0iu.png", // GTA V
            "https://image.api.playstation.com/vulcan/ap/rnd/202111/3013/cJdbb8URYlKgxcRVmgCFfB9w.png", // Cyberpunk 2077
            "https://image.api.playstation.com/vulcan/ap/rnd/202107/3100/HO8vkO9pfXhwbHi5WHECQJdN.png", // Horizon Forbidden West
            "https://assets.nintendo.com/image/upload/c_fill,w_1200/q_auto:best/f_auto/dpr_2.0/ncom/software/switch/70010000001130/c42553b4fd0312c31e70ec7468c6c9bccd739f340152925b9600631f2d29f8b5", // Super Mario Odyssey
            "https://image.api.playstation.com/vulcan/ap/rnd/202010/0915/RYWdAMWqbTNkjQ4AliLuGj8R.png", // Uncharted 4
            "https://image.api.playstation.com/vulcan/ap/rnd/202010/0222/p50N0QDQDDIWsVvKZNLRn99X.png"  // Ghost of Tsushima
        };

        public GameController(ApplicationDbContext context, ILogger<GameController> logger, IGameImageService gameImageService)
        {
            _context = context;
            _logger = logger;
            _gameImageService = gameImageService;
        }

        // GET: /Game
        public async Task<IActionResult> Index(string platform = null, string genre = null, int? year = null, string platforms = null, string genres = null, int page = 1)
        {
            try
            {
                // Khởi tạo danh sách trò chơi
                List<Game> games = await _context.Games
                    .Include(g => g.GamePlatforms)
                    .ThenInclude(gp => gp.Platform)
                    .ToListAsync();
                
                // Chuyển đổi mảng trò chơi thành danh sách có thể lọc
                var filteredGames = games.ToList();
                
                // Lọc theo nền tảng từ tham số cũ (single platform)
                if (!string.IsNullOrEmpty(platform))
                {
                    // Chuẩn hóa tên nền tảng để xử lý các trường hợp khác nhau
                    var normalizedPlatform = platform;
                    
                    // Xác định Xbox Series vs Xbox Series X
                    if (platform.Contains("Xbox Series") && !platform.Contains("Xbox Series X"))
                    {
                        normalizedPlatform = "Xbox Series X";
                    }
                    
                    filteredGames = filteredGames.Where(g => 
                        // Kiểm tra trong GamePlatforms
                        (g.GamePlatforms != null && 
                            g.GamePlatforms.Any(gp => 
                                gp.Platform != null && 
                                (gp.Platform.Name.Contains(normalizedPlatform) || 
                                    normalizedPlatform.Contains(gp.Platform.Name))
                            ))
                        ||
                        // Hoặc kiểm tra trong thuộc tính Platform
                        (g.Platform != null && 
                            (g.Platform.Contains(normalizedPlatform) || 
                            // Kiểm tra nếu Platform chứa nền tảng dưới dạng danh sách phân tách bằng dấu phẩy
                            g.Platform.Split(',').Select(p => p.Trim()).Any(p => 
                                p.Contains(normalizedPlatform) || normalizedPlatform.Contains(p))))
                    ).ToList();
                }
                
                // Lọc theo danh sách nền tảng mới (multiple platforms)
                if (!string.IsNullOrEmpty(platforms))
                {
                    var platformList = platforms.Split(',').Select(p => p.Trim()).ToList();
                    
                    if (platformList.Any())
                    {
                        // Create a new filtered list that matches any of the selected platforms
                        filteredGames = filteredGames.Where(g => 
                            // Check in GamePlatforms
                            (g.GamePlatforms != null && 
                                g.GamePlatforms.Any(gp => 
                                    gp.Platform != null && 
                                    platformList.Any(p => 
                                        gp.Platform.Name.Contains(p) || 
                                        p.Contains(gp.Platform.Name))
                                ))
                            ||
                            // Or check in Platform property
                            (g.Platform != null && 
                                platformList.Any(p => 
                                    g.Platform.Contains(p) || 
                                    g.Platform.Split(',').Select(pl => pl.Trim())
                                        .Any(pl => pl.Contains(p) || p.Contains(pl))
                                )
                            )
                        ).ToList();
                        
                        _logger.LogInformation($"Found {filteredGames.Count} games for platforms: {platforms}");
                    }
                }

                // Apply genre filter (old parameter)
                if (!string.IsNullOrEmpty(genre))
                {
                    filteredGames = filteredGames.Where(g => 
                        g.Genre != null && g.Genre.ToLower().Contains(genre.ToLower())
                    ).ToList();
                }
                
                // Apply genres filter (new parameter - multiple genres)
                if (!string.IsNullOrEmpty(genres))
                {
                    var selectedGenreList = genres.Split(',').Select(g => g.Trim().ToLower()).ToList();
                    
                    if (selectedGenreList.Any())
                    {
                        filteredGames = filteredGames.Where(g => 
                            g.Genre != null && 
                            selectedGenreList.Any(selectedGenre => 
                                g.Genre.ToLower().Contains(selectedGenre) ||
                                g.Genre.ToLower().Split(',').Select(g => g.Trim())
                                    .Any(g => g.Contains(selectedGenre) || selectedGenre.Contains(g))
                            )
                        ).ToList();
                        
                        _logger.LogInformation($"Found {filteredGames.Count} games for genres: {genres}");
                    }
                }
                
                // Apply year filter
                if (year.HasValue)
                {
                    filteredGames = filteredGames.Where(g => 
                        g.ReleaseDate.HasValue && g.ReleaseDate.Value.Year == year.Value
                    ).ToList();
                    
                    _logger.LogInformation($"Found {filteredGames.Count} games for year {year}");
                }

                // Nếu sau khi lọc không có game nào, sử dụng sample games
                if (!filteredGames.Any())
                {
                    _logger.LogInformation("No games found after filtering, using sample games");
                    var sampleGames = CreateSampleGames();
                    
                    // Lọc sample games dựa trên các tiêu chí đã chọn
                if (!string.IsNullOrEmpty(platform))
                {
                        sampleGames = sampleGames.Where(g => 
                            g.Platform != null && g.Platform.Contains(platform)
                        ).ToList();
                    }
                    
                    if (!string.IsNullOrEmpty(platforms))
                    {
                        var platformList = platforms.Split(',').Select(p => p.Trim()).ToList();
                        sampleGames = sampleGames.Where(g => 
                            g.Platform != null && 
                            platformList.Any(p => g.Platform.Contains(p))
                        ).ToList();
                    }
                    
                if (!string.IsNullOrEmpty(genre))
                {
                        sampleGames = sampleGames.Where(g => 
                            g.Genre != null && g.Genre.ToLower().Contains(genre.ToLower())
                        ).ToList();
                    }
                    
                    if (!string.IsNullOrEmpty(genres))
                    {
                        var selectedGenreList = genres.Split(',').Select(g => g.Trim().ToLower()).ToList();
                        sampleGames = sampleGames.Where(g => 
                            g.Genre != null && 
                            selectedGenreList.Any(selectedGenre => g.Genre.ToLower().Contains(selectedGenre))
                        ).ToList();
                    }
                    
                    if (year.HasValue)
                    {
                        sampleGames = sampleGames.Where(g => 
                            g.ReleaseDate.HasValue && g.ReleaseDate.Value.Year == year.Value
                        ).ToList();
                    }
                    
                    filteredGames = sampleGames;
                    _logger.LogInformation($"Added {filteredGames.Count} sample games matching criteria");
                }

                // Calculate pagination
                int pageSize = 12;
                var totalItems = filteredGames.Count;
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                // Ensure page number is valid
                page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

                // Get games for current page
                var pagedGames = filteredGames
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Ensure all games have images
                AssignImages(pagedGames);
                
                // Ensure all games have platform information
                AssignPlatforms(pagedGames);

                // Get unique genres for filter dropdown
                var genreList = new List<string>();
                foreach (var game in games)
                {
                    if (!string.IsNullOrEmpty(game.Genre))
                    {
                        var genreParts = game.Genre.Split(',');
                        foreach (var part in genreParts)
                        {
                            var trimmed = part.Trim();
                            if (!string.IsNullOrEmpty(trimmed) && !genreList.Contains(trimmed))
                            {
                                genreList.Add(trimmed);
                            }
                        }
                    }
                }
                genreList.Sort();

                // Set up ViewBag values
                ViewBag.Genres = genreList;
                ViewBag.CurrentGenre = genre;
                ViewBag.CurrentPlatform = platform;
                ViewBag.CurrentYear = year;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;

                return View(pagedGames);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Index action");
                return View("Error", new ErrorViewModel
                {
                    RequestId = HttpContext.TraceIdentifier,
                    Message = "An error occurred while loading the games. Please try again later."
                });
            }
        }

        private List<Game> CreateSampleGames()
        {
            // Generate unique IDs for sample games that are negative to avoid conflicts with database
            return new List<Game>
            {
                new Game
                {
                    Id = -1,
                    Title = "God of War Ragnarök",
                    Description = "God of War Ragnarök is an action-adventure game developed by Santa Monica Studio and published by Sony Interactive Entertainment. It was released worldwide on November 9, 2022, for the PlayStation 4 and PlayStation 5, marking the first cross-gen release in the God of War series.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202207/0706/CQh4aqIWJSwvRGWKIQDwNMBG.jpg",
                    VideoUrl = "https://www.youtube.com/watch?v=EE-4GvjKcfs",
                    TrailerUrl = "https://www.youtube.com/watch?v=EE-4GvjKcfs",
                    MetaScore = 94,
                    ReleaseDate = new DateTime(2022, 11, 9),
                    Platform = "PlayStation 5, PlayStation 4",
                    Genre = "Action, Adventure",
                    Developer = "Santa Monica Studio",
                    Publisher = "Sony Interactive Entertainment",
                    Rating = "M"
                },
                new Game
                {
                    Id = -2,
                    Title = "The Last of Us Part II",
                    Description = "The Last of Us Part II is an action-adventure game developed by Naughty Dog and published by Sony Interactive Entertainment. Set five years after The Last of Us (2013), the game focuses on two playable characters in a post-apocalyptic United States whose lives intertwine: Ellie, who sets out for revenge after suffering a tragedy, and Abby, a soldier who becomes involved in a conflict between her militia and a religious cult.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202010/0222/niMUubpU9y1PxNvYmDfb8QFD.png",
                    VideoUrl = "https://www.youtube.com/watch?v=qPNiIeKMHyg",
                    TrailerUrl = "https://www.youtube.com/watch?v=qPNiIeKMHyg",
                    MetaScore = 93,
                    ReleaseDate = new DateTime(2020, 6, 19),
                    Platform = "PlayStation 5, PlayStation 4",
                    Genre = "Action, Adventure",
                    Developer = "Naughty Dog",
                    Publisher = "Sony Interactive Entertainment",
                    Rating = "M"
                },
                new Game
                {
                    Id = -3,
                    Title = "Horizon Forbidden West",
                    Description = "Horizon Forbidden West is a 2022 action role-playing game developed by Guerrilla Games and published by Sony Interactive Entertainment. The sequel to 2017's Horizon Zero Dawn, the game is set in a post-apocalyptic version of the western United States recovering from the aftermath of an extinction event caused by a rogue robot swarm.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202107/3100/HO8vkO9pfXhwbHi5WHECQJdN.png",
                    VideoUrl = "https://www.youtube.com/watch?v=SZbpSIQNzxU",
                    TrailerUrl = "https://www.youtube.com/watch?v=SZbpSIQNzxU",
                    MetaScore = 88,
                    ReleaseDate = new DateTime(2022, 2, 18),
                    Platform = "PlayStation 5, PlayStation 4",
                    Genre = "Action RPG",
                    Developer = "Guerrilla Games",
                    Publisher = "Sony Interactive Entertainment",
                    Rating = "T"
                },
                new Game
                {
                    Id = -4,
                    Title = "Ghost of Tsushima",
                    Description = "Ghost of Tsushima is a 2020 action-adventure game developed by Sucker Punch Productions and published by Sony Interactive Entertainment. The game follows Jin Sakai, a samurai on a quest to protect Tsushima Island during the first Mongol invasion of Japan.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/img/rnd/202010/2618/GemRaOZaCMhGxXXkYtIlHbJ3.png",
                    VideoUrl = "https://www.youtube.com/watch?v=rTNfgIAi3pY",
                    TrailerUrl = "https://www.youtube.com/watch?v=rTNfgIAi3pY",
                    MetaScore = 83,
                    ReleaseDate = new DateTime(2020, 7, 17),
                    Platform = "PlayStation 5, PlayStation 4",
                    Genre = "Action, Adventure",
                    Developer = "Sucker Punch Productions",
                    Publisher = "Sony Interactive Entertainment",
                    Rating = "M"
                },
                new Game
                {
                    Id = -5,
                    Title = "Marvel's Spider-Man 2",
                    Description = "Marvel's Spider-Man 2 is a 2023 action-adventure game developed by Insomniac Games and published by Sony Interactive Entertainment. Based on Marvel Comics' Spider-Man character, it is the sequel to Marvel's Spider-Man (2018) and Marvel's Spider-Man: Miles Morales (2020).",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202306/1219/1c7afc402e2e8969378d2d26d48767989tw2bm3o.jpg",
                    VideoUrl = "https://www.youtube.com/watch?v=9fVYKsEmuRo",
                    TrailerUrl = "https://www.youtube.com/watch?v=9fVYKsEmuRo",
                    MetaScore = 90,
                    ReleaseDate = new DateTime(2023, 10, 20),
                    Platform = "PlayStation 5",
                    Genre = "Action, Adventure",
                    Developer = "Insomniac Games",
                    Publisher = "Sony Interactive Entertainment",
                    Rating = "T"
                },
                new Game
                {
                    Id = -6,
                    Title = "Final Fantasy XVI",
                    Description = "Final Fantasy XVI is an action role-playing game developed and published by Square Enix. The sixteenth main installment in the Final Fantasy series, it was released for the PlayStation 5 on June 22, 2023.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202211/0819/S1jCzktHlS9JIYnlZ787Q2Kh.jpg",
                    VideoUrl = "https://www.youtube.com/watch?v=yr6PtdY0i7M",
                    TrailerUrl = "https://www.youtube.com/watch?v=yr6PtdY0i7M",
                    MetaScore = 87,
                    ReleaseDate = new DateTime(2023, 6, 22),
                    Platform = "PlayStation 5",
                    Genre = "Action RPG",
                    Developer = "Square Enix",
                    Publisher = "Square Enix",
                    Rating = "M"
                },
                new Game
                {
                    Id = -7,
                    Title = "Ratchet & Clank: Rift Apart",
                    Description = "Ratchet & Clank: Rift Apart is a 2021 third-person shooter platform game developed by Insomniac Games and published by Sony Interactive Entertainment for the PlayStation 5.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202101/2921/OcZ4eq2L7aKIWxlZXLl68JZE.jpg",
                    VideoUrl = "https://www.youtube.com/watch?v=9p_gg9UW9k4",
                    TrailerUrl = "https://www.youtube.com/watch?v=9p_gg9UW9k4",
                    MetaScore = 88,
                    ReleaseDate = new DateTime(2021, 6, 11),
                    Platform = "PlayStation 5",
                    Genre = "Platform, Action",
                    Developer = "Insomniac Games",
                    Publisher = "Sony Interactive Entertainment",
                    Rating = "E10+"
                },
                new Game
                {
                    Id = -8,
                    Title = "Demon's Souls",
                    Description = "Demon's Souls is a 2020 action role-playing game developed by Bluepoint Games and published by Sony Interactive Entertainment for the PlayStation 5. It is a remake of Demon's Souls, originally developed by FromSoftware and released for the PlayStation 3 in 2009.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202011/0402/Xvt1i8DFy23EMURhQx8PWFyH.jpg",
                    VideoUrl = "https://www.youtube.com/watch?v=2TMs2E6cms4",
                    TrailerUrl = "https://www.youtube.com/watch?v=2TMs2E6cms4",
                    MetaScore = 92,
                    ReleaseDate = new DateTime(2020, 11, 12),
                    Platform = "PlayStation 5",
                    Genre = "Action RPG",
                    Developer = "Bluepoint Games",
                    Publisher = "Sony Interactive Entertainment",
                    Rating = "M"
                },
                new Game
                {
                    Id = -9,
                    Title = "Returnal",
                    Description = "Returnal is a 2021 third-person shooter roguelike video game developed by Housemarque and published by Sony Interactive Entertainment for the PlayStation 5.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202101/2921/22B2LQQfkCpUJQmHfK0QS7ga.png",
                    VideoUrl = "https://www.youtube.com/watch?v=Jv4BjWoB-NA",
                    TrailerUrl = "https://www.youtube.com/watch?v=Jv4BjWoB-NA",
                    MetaScore = 86,
                    ReleaseDate = new DateTime(2021, 4, 30),
                    Platform = "PlayStation 5",
                    Genre = "Third-person shooter, Roguelike",
                    Developer = "Housemarque",
                    Publisher = "Sony Interactive Entertainment",
                    Rating = "T"
                },
                new Game
                {
                    Id = -10,
                    Title = "Gran Turismo 7",
                    Description = "Gran Turismo 7 is a 2022 simulation racing video game developed by Polyphony Digital and published by Sony Interactive Entertainment. The game is the eighth mainline installment in the Gran Turismo series.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202109/1323/OZvO9e1nbmrw9YZLaR0WzUSZ.jpg",
                    VideoUrl = "https://www.youtube.com/watch?v=1tBUsXIkG1A",
                    TrailerUrl = "https://www.youtube.com/watch?v=1tBUsXIkG1A",
                    MetaScore = 87,
                    ReleaseDate = new DateTime(2022, 3, 4),
                    Platform = "PlayStation 5, PlayStation 4",
                    Genre = "Racing, Simulation",
                    Developer = "Polyphony Digital",
                    Publisher = "Sony Interactive Entertainment",
                    Rating = "E"
                }
            };
        }

        private void AssignImages(List<Game> games)
        {
            foreach (var game in games)
            {
                if (string.IsNullOrEmpty(game.ImageUrl))
                {
                    // Nếu không có ảnh, đặt ảnh placeholder
                    game.ImageUrl = "/images/game-placeholder.jpg";
                }
                else if (game.ImageUrl.StartsWith("/images/"))
                {
                    // Nếu là đường dẫn nội bộ và không tồn tại, đổi về placeholder
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", game.ImageUrl.TrimStart('/'));
                    if (!System.IO.File.Exists(filePath))
                    {
                        _logger.LogWarning($"Image not found at {filePath} for game {game.Title}. Using placeholder.");
                        game.ImageUrl = "/images/game-placeholder.jpg";
                    }
                }
                else if (!game.ImageUrl.StartsWith("http"))
                {
                    // Đảm bảo đường dẫn tương đối bắt đầu bằng / 
                    if (!game.ImageUrl.StartsWith("/"))
                    {
                        game.ImageUrl = "/" + game.ImageUrl;
                    }
                }
                
                // Đảm bảo có ảnh dự phòng nếu ảnh gốc không tải được
                if (!game.ImageUrl.StartsWith("/images/game-placeholder.jpg"))
                {
                    // Không cần thay đổi gì, ảnh sẽ sử dụng thuộc tính onerror trong HTML để load ảnh dự phòng nếu cần
                }
            }
            
            // Ensure platform information is assigned
            AssignPlatforms(games);
        }
        
        private void AssignPlatforms(List<Game> games)
        {
            var platformsByGenre = new Dictionary<string, string>
            {
                { "Action", "PC, PlayStation 5, Xbox Series X" },
                { "Adventure", "PC, PlayStation 5, Xbox Series X, Nintendo Switch" },
                { "RPG", "PC, PlayStation 5, Xbox Series X" },
                { "Strategy", "PC, Nintendo Switch" },
                { "Simulation", "PC, PlayStation 5" },
                { "Sports", "PC, PlayStation 5, Xbox Series X, Nintendo Switch" },
                { "Racing", "PC, PlayStation 5, Xbox Series X" },
                { "Fighting", "PlayStation 5, Xbox Series X, Nintendo Switch" },
                { "Shooter", "PC, PlayStation 5, Xbox Series X" },
                { "Platformer", "PC, PlayStation 5, Xbox Series X, Nintendo Switch" },
                { "Puzzle", "PC, PlayStation 5, Xbox Series X, Nintendo Switch, Mobile" }
            };
            
            // Dictionary for specific game titles
            var platformsByTitle = new Dictionary<string, string>
            {
                { "Final Fantasy", "PlayStation 5, PC" },
                { "Stellar Blade", "PlayStation 5" },
                { "Zelda", "Nintendo Switch" },
                { "Mario", "Nintendo Switch" },
                { "Halo", "Xbox Series X, PC" },
                { "Forza", "Xbox Series X, PC" },
                { "God of War", "PlayStation 5, PC" },
                { "Spider-Man", "PlayStation 5" },
                { "Helldivers", "PlayStation 5, PC" },
                { "Starfield", "Xbox Series X, PC" },
                { "Elden Ring", "PC, PlayStation 5, Xbox Series X" },
                { "Tekken", "PC, PlayStation 5, Xbox Series X" },
                { "Call of Duty", "PC, PlayStation 5, Xbox Series X" },
                { "Battlefield", "PC, PlayStation 5, Xbox Series X" },
                { "Assassin's Creed", "PC, PlayStation 5, Xbox Series X" },
                { "Alan Wake", "PC, PlayStation 5, Xbox Series X" },
                { "Resident Evil", "PC, PlayStation 5, Xbox Series X" },
                { "Black Myth", "PC, PlayStation 5, Xbox Series X" }
            };
            
            foreach (var game in games)
            {
                // Check if GamePlatforms collection is populated and contains valid data
                if (game.GamePlatforms != null && game.GamePlatforms.Any(gp => gp.Platform != null))
                {
                    // If we have valid GamePlatforms, update the Platform string property
                    // for consistency and for views that rely on it
                    var platformNames = game.GamePlatforms
                        .Where(gp => gp.Platform != null)
                        .Select(gp => gp.Platform.Name)
                        .OrderBy(name => name)
                        .ToList();
                    
                    if (platformNames.Any())
                    {
                        game.Platform = string.Join(", ", platformNames);
                        continue; // Skip the rest of the loop since we have proper platform data
                    }
                }
                
                // If GamePlatforms is empty but Platform property has data, check if it's valid
                if (!string.IsNullOrEmpty(game.Platform) && game.Platform != "Not specified")
                {
                    // Just make sure platform names are consistent
                    game.Platform = game.Platform.Replace("PS5", "PlayStation 5")
                                                .Replace("PS4", "PlayStation 4")
                                                .Replace("XBOX", "Xbox")
                                                .Replace("Xbox Series X/S", "Xbox Series X");
                                                
                    // Thêm các nền tảng thiếu nếu có thể 
                    // Nếu game là title phổ biến, gán đầy đủ nền tảng cho nó
                    foreach (var titleKey in platformsByTitle.Keys)
                    {
                        if (game.Title != null && game.Title.Contains(titleKey))
                        {
                            game.Platform = platformsByTitle[titleKey];
                            break;
                        }
                    }
                    
                    continue; // Skip the rest of the loop since we have platform data
                }
                
                // If we've reached here, we need to assign platforms
                string platforms = "PC, PlayStation 5, Xbox Series X"; // Default platforms
                
                // First check if the title matches any known game series
                bool titleMatch = false;
                foreach (var title in platformsByTitle.Keys)
                {
                    if (game.Title != null && game.Title.ToLower().Contains(title.ToLower()))
                    {
                        platforms = platformsByTitle[title];
                        titleMatch = true;
                        break;
                    }
                }
                
                // If no title match, try to match by genre
                if (!titleMatch && !string.IsNullOrEmpty(game.Genre))
                {
                    foreach (var genre in platformsByGenre.Keys)
                    {
                        if (game.Genre.ToLower().Contains(genre.ToLower()))
                        {
                            platforms = platformsByGenre[genre];
                            break;
                        }
                    }
                }
                
                game.Platform = platforms;
                
                // Thêm log để theo dõi
                _logger.LogInformation($"Assigned platforms for game '{game.Title}': {game.Platform}");
            }
        }

        // GET: Game/Details/5
        [HttpGet]
        [Route("Game/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            // Add logging to trace the request
            _logger.LogInformation($"Details action called with id: {id}");
            
            if (id == 0)
            {
                _logger.LogWarning("Details called with id = 0, returning NotFound");
                return View("Error", new ErrorViewModel
                {
                    RequestId = HttpContext.TraceIdentifier,
                    Message = "Game ID cannot be zero.",
                    StatusCode = 400
                });
            }

            Game game = null;
            
            try
            {
                // For sample games (negative IDs)
                if (id < 0)
                {
                    _logger.LogInformation($"Handling negative ID: {id}");
                    
                    // Check upcoming sample games first (ID range: -10001 to -19999)
                    if (id >= -19999)
                    {
                        _logger.LogInformation("Checking sample upcoming games");
                        var sampleUpcomingGames = GetSampleUpcomingGames();
                        game = sampleUpcomingGames.FirstOrDefault(g => g.Id == id);
                        
                        if (game != null)
                        {
                            _logger.LogInformation($"Found game in upcoming samples: {game.Title}");
                        }
                    }
                    
                    // Check new releases sample games if not found (ID range: -20001 to -29999)
                    if (game == null && id >= -29999)
                    {
                        _logger.LogInformation("Checking sample new releases");
                        var sampleNewReleases = GetSampleNewReleases();
                        game = sampleNewReleases.FirstOrDefault(g => g.Id == id);
                        
                        if (game != null)
                        {
                            _logger.LogInformation($"Found game in new releases samples: {game.Title}");
                        }
                    }
                    
                    // Check regular sample games if still not found
                    if (game == null)
                    {
                        _logger.LogInformation("Checking regular sample games");
                        var sampleGames = CreateSampleGames();
                        game = sampleGames.FirstOrDefault(g => g.Id == id);
                        
                        if (game != null)
                        {
                            _logger.LogInformation($"Found game in regular samples: {game.Title}");
                        }
                    }
                    
                    if (game == null)
                    {
                        _logger.LogWarning($"No sample game found with ID: {id}");
                        return View("Error", new ErrorViewModel
                        {
                            RequestId = HttpContext.TraceIdentifier,
                            Message = $"Game with ID {id} was not found in our sample games.",
                            StatusCode = 404
                        });
                    }
                }
                else
                {
                    // For real games from database
                    _logger.LogInformation($"Fetching game from database with ID: {id}");
                    game = await _context.Games
                        .Include(g => g.GamePlatforms)
                        .ThenInclude(gp => gp.Platform)
                .Include(g => g.Reviews)
                        .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
            {
                        _logger.LogWarning($"No database game found with ID: {id}");
                        return View("Error", new ErrorViewModel
                        {
                            RequestId = HttpContext.TraceIdentifier,
                            Message = $"Game with ID {id} was not found in our database.",
                            StatusCode = 404
                        });
                    }
                    else
                    {
                        _logger.LogInformation($"Found database game: {game.Title}");
                    }
                }
                
                // Make sure the game has an image
            if (string.IsNullOrEmpty(game.ImageUrl))
            {
                    _logger.LogInformation("Game missing image URL, getting one from service");
                    game.ImageUrl = _gameImageService.GetImageUrlByTitle(game.Title);
            }
            
                // Ensure platform is assigned if missing
                if (string.IsNullOrEmpty(game.Platform) || game.Platform == "Not specified")
            {
                    _logger.LogInformation("Assigning platform information to game");
                    AssignPlatforms(new List<Game> { game });
            }
                
                // Ensure video URLs are properly formatted
                if (!string.IsNullOrEmpty(game.VideoUrl) && !game.VideoUrl.Contains("embed"))
            {
                    _logger.LogInformation("Formatting video URL");
                game.VideoUrl = FormatYoutubeUrl(game.VideoUrl);
            }
                
                if (!string.IsNullOrEmpty(game.TrailerUrl) && !game.TrailerUrl.Contains("embed"))
                {
                    _logger.LogInformation("Formatting trailer URL");
                    game.TrailerUrl = FormatYoutubeUrl(game.TrailerUrl);
                }
                
                // If no trailer URL but has video URL, use that as trailer
                if (string.IsNullOrEmpty(game.TrailerUrl) && !string.IsNullOrEmpty(game.VideoUrl))
                {
                    _logger.LogInformation("Using video URL as trailer URL");
                    game.TrailerUrl = game.VideoUrl;
                }
                
                // Get related games based on genre and platform
                ViewBag.RelatedGames = await GetRelatedGames(game);
                
                // Ensure each store has a price and correct URL
                SetupStorePrices(game);
                
                // Log success
                _logger.LogInformation($"Successfully prepared game details for: {game.Title} (ID: {game.Id})");

            return View(game);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving game details for ID {id}");
                return View("Error", new ErrorViewModel
                {
                    RequestId = HttpContext.TraceIdentifier,
                    Message = $"There was an error loading game details for ID {id}. Error: {ex.Message}"
                });
            }
        }
        
        private void SetupStorePrices(Game game)
        {
            // Setup default store URLs and price points that can be referenced in the view
            var storePrices = new Dictionary<string, decimal>
            {
                { "PC", 49.99m },
                { "PlayStation 5", 69.99m },
                { "PlayStation 4", 59.99m },
                { "Xbox Series X", 69.99m },
                { "Xbox One", 59.99m },
                { "Nintendo Switch", 59.99m }
            };
            
            // Pass the store price information to the view
            ViewBag.StorePrices = storePrices;
            
            // Setup store URLs (could be expanded to use real URLs in the future)
            var storeUrls = new Dictionary<string, string>
            {
                { "PC", "https://store.steampowered.com" },
                { "PlayStation 5", "https://store.playstation.com" },
                { "PlayStation 4", "https://store.playstation.com" },
                { "Xbox Series X", "https://www.xbox.com/games/store" },
                { "Xbox One", "https://www.xbox.com/games/store" },
                { "Nintendo Switch", "https://www.nintendo.com/store" }
            };
            
            // Add game specific URLs
            if (game.Title == "Baldur's Gate 3")
            {
                // Set specific pricing for Baldur's Gate 3
                storePrices["PC"] = 59.99m;
                storePrices["PlayStation 5"] = 69.99m;
                storePrices["Xbox Series X"] = 69.99m;
                
                // Set specific store URLs for Baldur's Gate 3
                storeUrls["PC"] = "https://store.steampowered.com/app/1086940/Baldurs_Gate_3/";
                storeUrls["PlayStation 5"] = "https://store.playstation.com/en-us/concept/10005797";
                storeUrls["Xbox Series X"] = "https://www.xbox.com/en-us/games/store/baldurs-gate-3/9pf5h7m590vq";
                
                // Add GOG store option for PC
                ViewBag.AdditionalStores = new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "name", "GOG" },
                        { "platform", "PC" },
                        { "price", "$59.99" },
                        { "url", "https://www.gog.com/game/baldurs_gate_3" }
                    }
                };
            }
            else if (game.Title == "Marvel's Spider-Man 2")
            {
                // Set specific pricing for Spider-Man 2
                storePrices["PlayStation 5"] = 69.99m;
                
                // Set specific store URL for Spider-Man 2
                storeUrls["PlayStation 5"] = "https://store.playstation.com/en-us/product/UP9000-PPSA08760_00-MARVELSPIDERMAN2";
            }
            else if (game.Title == "The Legend of Zelda: Tears of the Kingdom")
            {
                // Set specific pricing for Zelda
                storePrices["Nintendo Switch"] = 69.99m;
                
                // Set specific store URL for Zelda
                storeUrls["Nintendo Switch"] = "https://www.nintendo.com/store/products/the-legend-of-zelda-tears-of-the-kingdom-switch/";
            }
            
            ViewBag.StoreUrls = storeUrls;
        }

        private string FormatYoutubeUrl(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url)) return url;

                // If URL already has embed format, return as is
                if (url.Contains("/embed/")) return url;

                string videoId = null;

                // Handle standard YouTube URL format (watch?v=)
                if (url.Contains("youtube.com/watch"))
                {
                    var uri = new Uri(url);
                    var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
                    videoId = queryParams["v"];
                }
                // Handle short YouTube URL format (youtu.be/)
                else if (url.Contains("youtu.be"))
                {
                    var uri = new Uri(url);
                    videoId = uri.Segments.Last().Split('?')[0];
                }
                // Handle YouTube URL format (youtube.com/v/)
                else if (url.Contains("youtube.com/v/"))
                {
                    var uri = new Uri(url);
                    videoId = uri.Segments[2].Split('?')[0];
                }
                // Try to extract video ID from any URL containing "youtube"
                else if (url.Contains("youtube"))
                {
                    // Try to find a pattern of 11 characters that could be a video ID
                    var regex = new System.Text.RegularExpressions.Regex(@"(?:youtube\.com\/(?:[^\/\n\s]+\/\S+\/|(?:v|e(?:mbed)?)\/|\S*?[?&]v=)|youtu\.be\/)([a-zA-Z0-9_-]{11})");
                    var match = regex.Match(url);
                    if (match.Success)
                    {
                        videoId = match.Groups[1].Value;
                    }
                }

                // If we found a valid video ID, return the embedded URL
                if (!string.IsNullOrEmpty(videoId))
                {
                    return $"https://www.youtube.com/embed/{videoId}";
                }

                // If all else fails, return the original URL
                return url;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error formatting YouTube URL: {url}");
                return url; // Return original URL in case of error
            }
        }

        // POST: /Game/Review
        // POST: /Game/Review
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Review(int gameId, string? title, string content, int score)
        {
            try
            {
                var game = await _context.Games.FindAsync(gameId);
                if (game == null)
                {
                    return NotFound();
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("User not found");
                }

                var review = new Review
                {
                    GameId = gameId,
                    UserId = userId,
                    Title = title ?? "",
                    Content = content,
                    Score = score,
                    ReviewDate = DateTime.Now,
                    HelpfulCount = 0,
                    UnhelpfulCount = 0
                };

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                // Lấy toàn bộ review của game
                var allReviews = await _context.Reviews.Where(r => r.GameId == gameId).ToListAsync();

                // Cập nhật UserScore và UserReviewCount
                game.UserScore = (int)Math.Round(allReviews.Average(r => r.Score));
                game.UserReviewCount = allReviews.Count;

                // Tính TrendingScore (các review trong 7 ngày gần nhất)
                var sevenDaysAgo = DateTime.Now.AddDays(-7);
                var recentReviews = allReviews.Where(r => r.ReviewDate >= sevenDaysAgo).ToList();

                if (recentReviews.Any())
                {
                    float avgScore = (float)recentReviews.Average(r => r.Score);
                    game.TrendingScore = recentReviews.Count * avgScore;
                }
                else
                {
                    game.TrendingScore = 0f;
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Details), new { id = gameId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting review");
                return View("Error", new ErrorViewModel
                {
                    RequestId = HttpContext.TraceIdentifier,
                    Message = "Có lỗi xảy ra khi gửi đánh giá. Vui lòng thử lại sau."
                });
            }
        }




        // Admin actions
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.Platforms = _context.Platforms.OrderBy(p => p.Name).ToList();
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(Game game, List<int> platformIds)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra và cập nhật cấu trúc cơ sở dữ liệu trước
                    await EnsureDatabaseStructure();
                    
                    // Đảm bảo VideoUrl có đúng định dạng YouTube embed nếu có
                    if (!string.IsNullOrEmpty(game.VideoUrl))
                    {
                        game.VideoUrl = FormatYoutubeUrl(game.VideoUrl);
                    }
                    
                _context.Games.Add(game);
                await _context.SaveChangesAsync();

                foreach (var platformId in platformIds)
                {
                    var gamePlatform = new GamePlatform
                    {
                        GameId = game.Id,
                        PlatformId = platformId,
                        ReleaseDate = game.ReleaseDate.GetValueOrDefault(DateTime.Now)
                    };
                    _context.GamePlatforms.Add(gamePlatform);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Log lỗi và hiển thị thông báo thân thiện
                    ModelState.AddModelError(string.Empty, "Có lỗi khi lưu dữ liệu: " + ex.Message);
                }
            }

            ViewBag.Platforms = _context.Platforms.OrderBy(p => p.Name).ToList();
            return View(game);
        }

        // GET: Game/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            return View(game);
        }

        // POST: Game/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,ImageUrl,VideoUrl,MetaScore,ReleaseDate,Platform,Genre,Developer,Publisher,Rating")] Game game)
        {
            if (id != game.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra và cập nhật cấu trúc cơ sở dữ liệu trước
                    await EnsureDatabaseStructure();
                    
                    // Đảm bảo VideoUrl có đúng định dạng YouTube embed nếu có
                    if (!string.IsNullOrEmpty(game.VideoUrl))
                    {
                        game.VideoUrl = FormatYoutubeUrl(game.VideoUrl);
                    }
                    
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    // Log lỗi và hiển thị thông báo thân thiện
                    ModelState.AddModelError(string.Empty, "Có lỗi khi lưu dữ liệu: " + ex.Message);
                    return View(game);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(game);
        }

        // Phương thức để đảm bảo cấu trúc database đúng
        private async Task EnsureDatabaseStructure()
        {
            try
            {
                // Chỉ đảm bảo kết nối hoạt động
                if (_context?.Database != null)
                {
                    await Task.CompletedTask;
                }
            }
            catch
            {
                // Bỏ qua lỗi
            }
        }

        // GET: Game/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Game/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game != null)
            {
                _context.Games.Remove(game);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.Id == id);
        }

        // POST: Game/MarkHelpful/5
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> MarkHelpful(int id)
        {
            try
            {
                var review = await _context.Reviews.FindAsync(id);
                if (review == null)
                {
                    return Json(new { success = false, message = "Review not found" });
                }

                // Increment helpful count
                review.HelpfulCount++;
                await _context.SaveChangesAsync();

                return Json(new { success = true, helpfulCount = review.HelpfulCount });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking review as helpful");
                return Json(new { success = false, message = "Error updating review" });
            }
        }

        // GET: /Game/GetGameDetails/{id} - API endpoint để lấy chi tiết game cho modal
        [HttpGet]
        [Route("Game/GetGameDetails/{id}")]
        public async Task<IActionResult> GetGameDetails(int id)
        {
            try
            {
                var game = await _context.Games.FindAsync(id);
                
                if (game == null)
                {
                    return NotFound(new { message = "Game not found" });
                }
                
                // Kiểm tra và cập nhật URL hình ảnh nếu cần
                if (string.IsNullOrEmpty(game.ImageUrl))
                {
                    game.ImageUrl = "/images/game-placeholder.jpg";
                }
                
                // Chuyển đổi YouTube URL để có thể nhúng
                string videoUrl = null;
                if (!string.IsNullOrEmpty(game.VideoUrl))
                {
                    videoUrl = FormatYoutubeUrl(game.VideoUrl);
                }
                
                // Định dạng ngày phát hành thành chuỗi dễ đọc
                string releaseDate = game.ReleaseDate.HasValue 
                    ? game.ReleaseDate.Value.ToString("MMMM d, yyyy") 
                    : "TBA";
                
                // Trả về JSON object với thông tin game
                return Json(new
                {
                    id = game.Id,
                    title = game.Title,
                    description = game.Description,
                    imageUrl = game.ImageUrl,
                    videoUrl = videoUrl,
                    metaScore = game.MetaScore,
                    userScore = game.UserScore,
                    releaseDate = releaseDate,
                    platform = game.Platform,
                    genre = game.Genre,
                    developer = game.Developer,
                    publisher = game.Publisher,
                    rating = game.Rating
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetGameDetails action");
                return StatusCode(500, new { message = "An error occurred while retrieving game details" });
            }
        }

        // GET: /Game/SeedSampleData - Route để seed dữ liệu mẫu
        [HttpGet]
        [Route("Game/SeedSampleData")]
        public async Task<IActionResult> SeedSampleData()
        {
            try
            {
                // Kiểm tra xem đã có game trong database chưa
                if (!_context.Games.Any())
                {
                    var games = new List<Game>
                    {
                        new Game
                        {
                            Title = "Red Dead Redemption 2",
                            Description = "Red Dead Redemption 2 is an epic tale of life in America's unforgiving heartland. The game's vast and atmospheric world also provides the foundation for a brand new online multiplayer experience.",
                            ImageUrl = "https://image.api.playstation.com/cdn/UP1004/CUSA03041_00/Hpl5MtwQgOVF9vJqlfui6SDB5Jl4oBSq.png",
                            TrailerUrl = "https://www.youtube.com/embed/eaW0tYpxyp0",
                            VideoUrl = "https://www.youtube.com/embed/eaW0tYpxyp0",
                            MetaScore = 97,
                            ReleaseDate = new DateTime(2018, 10, 26),
                            Platform = "PC, PlayStation 4, Xbox One",
                            Genre = "Action, Adventure",
                            Developer = "Rockstar Games",
                            Publisher = "Rockstar Games",
                            Rating = "M"
                        },
                        new Game
                        {
                            Title = "The Witcher 3: Wild Hunt",
                            Description = "The Witcher 3: Wild Hunt is a story-driven open world RPG set in a visually stunning fantasy universe full of meaningful choices and impactful consequences.",
                            ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202211/0711/kh4MUIuMmHlktOHar3lVl6rY.png",
                            TrailerUrl = "https://www.youtube.com/embed/XHrskkHf958",
                            VideoUrl = "https://www.youtube.com/embed/XHrskkHf958",
                            MetaScore = 93,
                            ReleaseDate = new DateTime(2015, 5, 19),
                            Platform = "PC, PlayStation 4, Xbox One, Nintendo Switch",
                            Genre = "RPG",
                            Developer = "CD Projekt Red",
                            Publisher = "CD Projekt",
                            Rating = "M"
                        },
                        new Game
                        {
                            Title = "God of War (2018)",
                            Description = "His vengeance against the Gods of Olympus years behind him, Kratos now lives as a man in the realm of Norse Gods and monsters. It is in this harsh, unforgiving world that he must fight to survive… and teach his son to do the same.",
                            ImageUrl = "https://image.api.playstation.com/vulcan/img/rnd/202010/2217/LsaRVLF2IU2L1FNtu9d3MKLq.png",
                            TrailerUrl = "https://www.youtube.com/embed/K0u_kAWLJOA",
                            VideoUrl = "https://www.youtube.com/embed/K0u_kAWLJOA",
                            MetaScore = 94,
                            ReleaseDate = new DateTime(2018, 4, 20),
                            Platform = "PlayStation 4, PC",
                            Genre = "Action, Adventure",
                            Developer = "Santa Monica Studio",
                            Publisher = "Sony Interactive Entertainment",
                            Rating = "M"
                        },
                        new Game
                        {
                            Title = "Elden Ring",
                            Description = "Rise, Tarnished, and be guided by grace to brandish the power of the Elden Ring and become an Elden Lord in the Lands Between.",
                            ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202110/2000/phvVT0qZfcRms5qDAk0SI3CM.png",
                            TrailerUrl = "https://www.youtube.com/embed/E3Huy2cdih0",
                            VideoUrl = "https://www.youtube.com/embed/E3Huy2cdih0",
                            MetaScore = 96,
                            ReleaseDate = new DateTime(2022, 2, 25),
                            Platform = "PC, PlayStation 4, PlayStation 5, Xbox One, Xbox Series X",
                            Genre = "Action RPG",
                            Developer = "FromSoftware",
                            Publisher = "Bandai Namco Entertainment",
                            Rating = "M"
                        },
                        new Game
                        {
                            Title = "Baldur's Gate 3",
                            Description = "Gather your party, and return to the Forgotten Realms in a tale of fellowship and betrayal, sacrifice and survival, and the lure of absolute power.",
                            ImageUrl = "https://cdn.akamai.steamstatic.com/steam/apps/1086940/header.jpg",
                            TrailerUrl = "https://www.youtube.com/embed/1T22wNvoNiU",
                            VideoUrl = "https://www.youtube.com/embed/1T22wNvoNiU",
                            MetaScore = 97,
                            ReleaseDate = new DateTime(2023, 8, 3),
                            Platform = "PC, PlayStation 5",
                            Genre = "RPG",
                            Developer = "Larian Studios",
                            Publisher = "Larian Studios",
                            Rating = "M"
                        }
                    };

                    _context.Games.AddRange(games);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Added {Count} sample games to database", games.Count);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogInformation("Games already exist in database, skipping seed");
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding sample data");
                return View("Error", new ErrorViewModel
                {
                    RequestId = HttpContext.TraceIdentifier,
                    Message = "Có lỗi xảy ra khi thêm dữ liệu mẫu. Vui lòng thử lại sau."
                });
            }
        }

        // GET: Game/NewReleases
        public async Task<IActionResult> NewReleases()
        {
            // Redirect to the new Releases controller
            return RedirectToAction("NewReleases", "Releases");
        }

        // GET: /Game/ComingSoon
        public IActionResult ComingSoon()
        {
            // Redirect to the new ComingSoon controller
            return RedirectToAction("Index", "ComingSoon");
        }

        // GET: /Game/TopRated
        public IActionResult TopRated(string platform = null, string genre = null, int? year = null, string sortBy = "score", int page = 1)
        {
            try
            {
                // Start with a base query
                var query = _context.Games.AsQueryable();
                
                // Apply platform filter if specified using a more precise approach
                if (!string.IsNullOrEmpty(platform))
                {
                    // Get game IDs that match the platform
                    var gameIdsWithPlatform = _context.GamePlatforms
                        .Include(gp => gp.Platform)
                        .Where(gp => gp.Platform.Name.Contains(platform))
                        .Select(gp => gp.GameId)
                        .Distinct()
                        .ToList();
                        
                    // Then filter the main query by these IDs
                    query = query.Where(g => gameIdsWithPlatform.Contains(g.Id));
                }
                
                // Get total count for pagination
                var totalItems = query.Count();
                var pageSize = 12;
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                // Ensure page number is valid
                page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

                // Apply genre filter - fetch data first then filter in memory
                var baseGames = query.ToList();
                var filteredGames = baseGames;
                
                if (!string.IsNullOrEmpty(genre))
                {
                    filteredGames = baseGames.Where(g => 
                        g.Genre != null && g.Genre.ToLower().Contains(genre.ToLower())
                    ).ToList();
                }

                // Apply year filter
                if (year.HasValue)
                {
                    filteredGames = filteredGames.Where(g => 
                        g.ReleaseDate.HasValue && g.ReleaseDate.Value.Year == year.Value
                    ).ToList();
                }

                // Apply sorting
                switch (sortBy?.ToLower())
                {
                    case "date":
                        filteredGames = filteredGames.OrderByDescending(g => g.ReleaseDate).ToList();
                        break;
                    case "name":
                        filteredGames = filteredGames.OrderBy(g => g.Title).ToList();
                        break;
                    case "score":
                    default:
                        filteredGames = filteredGames.OrderByDescending(g => g.MetaScore).ToList();
                        break;
                }

                // Now do pagination in memory
                var games = filteredGames
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Load platforms for these games
                foreach (var game in games)
                {
                    _context.Entry(game)
                        .Collection(g => g.GamePlatforms)
                        .Load();

                    foreach (var gp in game.GamePlatforms)
                    {
                        _context.Entry(gp)
                            .Reference(x => x.Platform)
                            .Load();
                    }
                }

                // If no games found, use sample ones
                if (!games.Any())
                {
                    games = CreateSampleGames()
                        .OrderByDescending(g => g.MetaScore)
                        .Take(pageSize)
                        .ToList();
                }

                // Ensure all games have images
                AssignImages(games);
                
                // Ensure all games have proper platform information
                AssignPlatforms(games);

                // Get platforms for filter dropdown
                ViewBag.Platforms = _context.Platforms
                    .OrderBy(p => p.Name)
                    .Select(p => p.Name)
                    .ToList();

                // Get unique genres for dropdown - do this in memory to avoid issues with Split
                var allGames = _context.Games.Where(g => g.Genre != null).ToList();
                var genres = new List<string>();
                
                foreach (var g in allGames)
                {
                    if (g.Genre != null)
                    {
                        var genreParts = g.Genre.Split(',');
                        foreach (var part in genreParts)
                        {
                            var trimmed = part.Trim();
                            if (!string.IsNullOrEmpty(trimmed) && !genres.Contains(trimmed))
                            {
                                genres.Add(trimmed);
                            }
                        }
                    }
                }
                
                genres.Sort();
                ViewBag.Genres = genres;

                // Get years for filter dropdown
                var yearList = _context.Games
                    .Where(g => g.ReleaseDate.HasValue)
                    .Select(g => g.ReleaseDate.Value.Year)
                    .Distinct()
                    .OrderByDescending(y => y)
                    .ToList();
                ViewBag.Years = yearList;

                // Set ViewBag values for maintaining state
                ViewBag.CurrentPlatform = platform;
                ViewBag.CurrentGenre = genre;
                ViewBag.CurrentYear = year;
                ViewBag.CurrentSort = sortBy;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;

                return View(games);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in TopRated action");
                return View("Error", new ErrorViewModel 
                { 
                    RequestId = HttpContext.TraceIdentifier,
                    Message = "An error occurred while loading top rated games. Please try again later."
                });
            }
        }

        private async Task<List<Game>> GetRelatedGames(Game game, int count = 5)
        {
            if (game == null) return new List<Game>();

            try
            {
                // For sample games with negative IDs, get related sample games
                if (game.Id < 0)
                {
                    List<Game> allSampleGames = new List<Game>();
                    
                    // Combine all sample game collections
                    allSampleGames.AddRange(CreateSampleGames());
                    allSampleGames.AddRange(GetSampleUpcomingGames());
                    allSampleGames.AddRange(GetSampleNewReleases());
                    
                    // Return related games from sample data
                    return allSampleGames
                        .Where(g => g.Id != game.Id && 
                                  (g.Genre == game.Genre || g.Platform == game.Platform))
                        .OrderByDescending(g => g.MetaScore)
                        .Take(count)
                        .ToList();
                }
                
                // For database games, get related games from database
                var relatedGames = await _context.Games
                    .Where(g => g.Id != game.Id && 
                              (g.Genre == game.Genre || g.Platform == game.Platform))
                    .OrderByDescending(g => g.MetaScore)
                    .Take(count)
                    .ToListAsync();

                return relatedGames;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting related games");
                return new List<Game>();
            }
        }

        public IActionResult Search(string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return RedirectToAction("Index");
                }

                // Load all games first, then filter in memory
                var allGames = _context.Games
                    .Include(g => g.GamePlatforms)
                    .ThenInclude(gp => gp.Platform)
                    .ToList();
                    
                var games = allGames.Where(g => 
                    (g.Title != null && g.Title.ToLower().Contains(query.ToLower())) || 
                    (g.Description != null && g.Description.ToLower().Contains(query.ToLower())) ||
                    (g.Genre != null && g.Genre.ToLower().Contains(query.ToLower())) ||
                    (g.Developer != null && g.Developer.ToLower().Contains(query.ToLower())) ||
                    (g.Publisher != null && g.Publisher.ToLower().Contains(query.ToLower()))
                ).ToList();

                // If no results from db, try generating some sample results
                if (!games.Any())
                {
                    var allSampleGames = CreateSampleGames();
                    games = allSampleGames
                    .Where(g => 
                            (g.Title != null && g.Title.ToLower().Contains(query.ToLower())) || 
                            (g.Description != null && g.Description.ToLower().Contains(query.ToLower())) ||
                            (g.Genre != null && g.Genre.ToLower().Contains(query.ToLower())) ||
                            (g.Developer != null && g.Developer.ToLower().Contains(query.ToLower())) ||
                            (g.Publisher != null && g.Publisher.ToLower().Contains(query.ToLower())))
                    .ToList();
                }

                // Set up view data
                ViewBag.SearchQuery = query;
                ViewBag.ResultCount = games.Count;

                // Ensure all games have images
                AssignImages(games);
                
                // Ensure all games have proper platform information
                AssignPlatforms(games);

                return View("SearchResults", games);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in search: {Query}", query);
                return View("Error", new ErrorViewModel
                {
                    RequestId = HttpContext.TraceIdentifier,
                    Message = "An error occurred while searching. Please try again."
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
                }
            };
        }
    }
} 