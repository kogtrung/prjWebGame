using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebGame.Models;
using WebGame.Services;

namespace WebGame.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class GameController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public GameController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Admin/Game
        public async Task<IActionResult> Index()
        {
            var games = await _context.Games
                .Include(g => g.GamePlatforms)
                .ThenInclude(gp => gp.Platform)
                .ToListAsync();
            return View(games);
        }

        // GET: Admin/Game/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.GamePlatforms)
                .ThenInclude(gp => gp.Platform)
                .Include(g => g.Screenshots)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // GET: Admin/Game/Create
        public IActionResult Create()
        {
            // Lấy danh sách thể loại game phổ biến
            var genres = new List<string>
            {
                "Action", "Adventure", "RPG", "Strategy", "Simulation",
                "Sports", "Racing", "Fighting", "Shooter", "Puzzle",
                "Platformer", "MMORPG", "MOBA", "Horror", "Stealth",
                "Open World", "Battle Royale", "Survival", "Indie"
            };
            ViewData["Genres"] = new SelectList(genres.Select(g => new { Id = g, Name = g }), "Id", "Name");
            ViewData["Platforms"] = new MultiSelectList(_context.Platforms, "Id", "Name");
            return View();
        }

        // POST: Admin/Game/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,ReleaseDate,MetaScore,Genre,Developer,Publisher,Rating,ImageUrl,TrailerUrl")] Game game, List<int> SelectedPlatforms, IFormFile ImageFile)
        {
            Console.WriteLine("Create method called with game title: " + game.Title);

            // Validate required fields manually
            if (string.IsNullOrEmpty(game.Title))
            {
                ModelState.AddModelError("Title", "Tên game không được để trống");
            }
            if (string.IsNullOrEmpty(game.Description))
            {
                ModelState.AddModelError("Description", "Mô tả không được để trống");
            }
            if (string.IsNullOrEmpty(game.Genre))
            {
                ModelState.AddModelError("Genre", "Thể loại không được để trống");
            }
            if (string.IsNullOrEmpty(game.Developer))
            {
                ModelState.AddModelError("Developer", "Nhà phát triển không được để trống");
            }
            if (string.IsNullOrEmpty(game.Publisher))
            {
                ModelState.AddModelError("Publisher", "Nhà phát hành không được để trống");
            }
            if (string.IsNullOrEmpty(game.Rating))
            {
                ModelState.AddModelError("Rating", "Xếp hạng không được để trống");
            }
            if (!game.ReleaseDate.HasValue)
            {
                ModelState.AddModelError("ReleaseDate", "Ngày phát hành không được để trống");
            }

            Console.WriteLine("ModelState is " + (ModelState.IsValid ? "valid" : "invalid"));

            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    if (state.Value.Errors.Count > 0)
                    {
                        Console.WriteLine($"- {state.Key}: {string.Join(", ", state.Value.Errors.Select(e => e.ErrorMessage))}");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                Console.WriteLine("Starting to save the game");

                // Xử lý upload hình ảnh
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "games");
                    Console.WriteLine("Upload folder: " + uploadsFolder);

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Console.WriteLine("Creating directory: " + uploadsFolder);
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    Console.WriteLine("Saving image to: " + filePath);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    game.ImageUrl = "/images/games/" + uniqueFileName;
                    Console.WriteLine("Image saved at: " + game.ImageUrl);
                }
                else
                {
                    Console.WriteLine("No image file uploaded, using default image");
                    game.ImageUrl = "/images/games/default-game.jpg";
                }

                // Thêm game vào database
                try
                {
                    Console.WriteLine("Adding game to database");
                    _context.Add(game);
                    var result = await _context.SaveChangesAsync();
                    Console.WriteLine($"SaveChanges result: {result}, Game saved with ID: {game.Id}");

                    // Xử lý mối quan hệ nhiều-nhiều với Platform
                    if (SelectedPlatforms != null && SelectedPlatforms.Any())
                    {
                        Console.WriteLine("Adding platforms: " + string.Join(", ", SelectedPlatforms));
                        foreach (var platformId in SelectedPlatforms)
                        {
                            _context.GamePlatforms.Add(new GamePlatform
                            {
                                GameId = game.Id,
                                PlatformId = platformId,
                                ReleaseDate = game.ReleaseDate.GetValueOrDefault(DateTime.Now)
                            });
                        }

                        result = await _context.SaveChangesAsync();
                        Console.WriteLine($"Platform SaveChanges result: {result}");
                    }

                    TempData["Success"] = "Thêm game thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error saving game: " + ex.Message);
                    Console.WriteLine("Stack trace: " + ex.StackTrace);

                    // Lấy inner exception details
                    Exception innerEx = ex.InnerException;
                    while (innerEx != null)
                    {
                        Console.WriteLine("Inner exception: " + innerEx.Message);
                        Console.WriteLine("Inner stack trace: " + innerEx.StackTrace);
                        innerEx = innerEx.InnerException;
                    }

                    ModelState.AddModelError("", "Lỗi khi lưu game: " + ex.Message);
                    if (ex.InnerException != null)
                    {
                        ModelState.AddModelError("", "Chi tiết lỗi: " + ex.InnerException.Message);
                    }
                }
            }

            // If we got to here, something failed, redisplay form
            Console.WriteLine("Redisplaying form due to error");
            var genres = new List<string>
            {
                "Action", "Adventure", "RPG", "Strategy", "Simulation",
                "Sports", "Racing", "Fighting", "Shooter", "Puzzle",
                "Platformer", "MMORPG", "MOBA", "Horror", "Stealth",
                "Open World", "Battle Royale", "Survival", "Indie"
            };
            ViewData["Genres"] = new SelectList(genres.Select(g => new { Id = g, Name = g }), "Id", "Name", game.Genre);
            ViewData["Platforms"] = new MultiSelectList(_context.Platforms, "Id", "Name", SelectedPlatforms);
            return View(game);
        }

        // GET: Admin/Game/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.GamePlatforms)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (game == null)
            {
                return NotFound();
            }

            var genres = new List<string>
            {
                "Action", "Adventure", "RPG", "Strategy", "Simulation",
                "Sports", "Racing", "Fighting", "Shooter", "Puzzle",
                "Platformer", "MMORPG", "MOBA", "Horror", "Stealth",
                "Open World", "Battle Royale", "Survival", "Indie"
            };
            ViewData["Genres"] = new SelectList(genres.Select(g => new { Id = g, Name = g }), "Id", "Name", game.Genre);
            ViewData["Platforms"] = new MultiSelectList(_context.Platforms, "Id", "Name",
                game.GamePlatforms?.Select(gp => gp.PlatformId));

            return View(game);
        }

        // POST: Admin/Game/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Game game, List<int> SelectedPlatforms, IFormFile ImageFile)
        {
            if (id != game.Id)
            {
                return NotFound();
            }

            try
            {
                // Lấy game hiện tại để lấy ImageUrl cũ
                var existingGame = await _context.Games
                    .AsNoTracking()
                    .FirstOrDefaultAsync(g => g.Id == id);

                if (existingGame == null)
                {
                    return NotFound();
                }

                // Giữ lại ImageUrl cũ nếu không có file mới
                if (ImageFile == null || ImageFile.Length == 0)
                {
                    game.ImageUrl = existingGame.ImageUrl;
                }
                else
                {
                    // Xử lý upload ảnh mới
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "games");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    // Xóa file ảnh cũ nếu có
                    if (!string.IsNullOrEmpty(existingGame.ImageUrl) && existingGame.ImageUrl.StartsWith("/images/games/"))
                    {
                        var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, existingGame.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    game.ImageUrl = "/images/games/" + uniqueFileName;
                }

                // Cập nhật game sử dụng EntityState.Modified
                _context.Entry(game).State = EntityState.Modified;

                // Xóa platforms cũ
                var existingPlatforms = await _context.GamePlatforms
                    .Where(gp => gp.GameId == id)
                    .ToListAsync();

                if (existingPlatforms.Any())
                {
                    _context.GamePlatforms.RemoveRange(existingPlatforms);
                }

                // Thêm platforms mới
                if (SelectedPlatforms != null && SelectedPlatforms.Any())
                {
                    foreach (var platformId in SelectedPlatforms)
                    {
                        _context.GamePlatforms.Add(new GamePlatform
                        {
                            GameId = id,
                            PlatformId = platformId,
                            ReleaseDate = game.ReleaseDate.GetValueOrDefault(DateTime.Now)
                        });
                    }
                }

                // Lưu tất cả thay đổi
                await _context.SaveChangesAsync();
                TempData["Success"] = "Game updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(game.Id))
                {
                    return NotFound();
                }
                else
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật game. Vui lòng thử lại.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                if (ex.InnerException != null)
                {
                    ModelState.AddModelError("", "Chi tiết: " + ex.InnerException.Message);
                }
            }

            // If we got to here, something failed, redisplay form
            var genres = new List<string>
            {
                "Action", "Adventure", "RPG", "Strategy", "Simulation",
                "Sports", "Racing", "Fighting", "Shooter", "Puzzle",
                "Platformer", "MMORPG", "MOBA", "Horror", "Stealth",
                "Open World", "Battle Royale", "Survival", "Indie"
            };
            ViewData["Genres"] = new SelectList(genres.Select(g => new { Id = g, Name = g }), "Id", "Name", game.Genre);
            ViewData["Platforms"] = new MultiSelectList(_context.Platforms, "Id", "Name", SelectedPlatforms);

            return View(game);
        }

        // GET: Admin/Game/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.GamePlatforms)
                .ThenInclude(gp => gp.Platform)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Admin/Game/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var game = await _context.Games
                .Include(g => g.GamePlatforms)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (game == null)
            {
                return NotFound();
            }

            // Xóa file ảnh nếu có
            if (!string.IsNullOrEmpty(game.ImageUrl))
            {
                var imagePath = Path.Combine(_hostEnvironment.WebRootPath, game.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            // Xóa các screenshots nếu có
            if (game.Screenshots != null && game.Screenshots.Any())
            {
                foreach (var screenshot in game.Screenshots)
                {
                    var screenshotPath = Path.Combine(_hostEnvironment.WebRootPath, screenshot.TrimStart('/'));
                    if (System.IO.File.Exists(screenshotPath))
                    {
                        System.IO.File.Delete(screenshotPath);
                    }
                }
            }

            // Xóa game và các quan hệ liên quan
            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.Id == id);
        }
    }
}