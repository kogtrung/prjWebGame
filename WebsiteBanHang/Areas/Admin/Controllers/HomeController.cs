using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebGame.Models;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;

namespace WebGame.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IMemoryCache cache)
        {
            _logger = logger;
            _context = context;
            _cache = cache;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Get count of key data for dashboard
                ViewBag.GamesCount = await _context.Games.CountAsync();
                ViewBag.NewsCount = await _context.NewsPosts.CountAsync();
                
                // Get recent content for quick access
                ViewBag.RecentGames = await _context.Games
                    .OrderByDescending(g => g.Id)
                    .Take(5)
                    .ToListAsync();
                
                ViewBag.RecentNews = await _context.NewsPosts
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(5)
                    .ToListAsync();
                
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading admin dashboard");
                ViewData["ErrorMessage"] = "There was an error loading the dashboard data. Please try again later.";
                return View();
            }
        }

        private async Task<List<Game>> GetRecentGamesAsync()
        {
            try
            {
                return await _context.Games
                    .AsNoTracking()
                    .OrderByDescending(g => g.Id)
                    .Take(5)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to load recent games");
                return new List<Game>();
            }
        }

        private async Task<List<NewsPost>> GetRecentNewsAsync()
        {
            try
            {
                return await _context.NewsPosts
                    .AsNoTracking()
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(5)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to load recent news");
                return new List<NewsPost>();
            }
        }

        // Helper method to safely count records from a table
        private async Task<int> SafeCountAsync<T>() where T : class
        {
            try
            {
                return await _context.Set<T>().CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to count {typeof(T).Name} records");
                return 0;
            }
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

    // Class to hold dashboard statistics for caching
    public class DashboardStats
    {
        public int GamesCount { get; set; }
        public int NewsCount { get; set; }
        public int CategoriesCount { get; set; }
        public int ProductsCount { get; set; }
        public List<Game> RecentGames { get; set; } = new List<Game>();
        public List<NewsPost> RecentNews { get; set; } = new List<NewsPost>();
    }
}
