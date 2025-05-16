using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebGame.Models;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace WebGame.Controllers
{
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NewsController> _logger;

        public NewsController(ApplicationDbContext context, ILogger<NewsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index(string search = null, int? categoryId = null)
        {
            try
            {
                // Kiểm tra kết nối database
                if (!_context.Database.CanConnect())
                {
                    _logger.LogWarning("Cannot connect to database in NewsController.Index");
                    return View("Error", new ErrorViewModel
                    {
                        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                        Message = "Không thể kết nối đến cơ sở dữ liệu. Vui lòng thử lại sau."
                    });
                }

                // Thêm dữ liệu mẫu nếu chưa có
                if (!_context.NewsPosts.Any())
                {
                    var sampleNews = new List<NewsPost>
                    {
                        new NewsPost
                        {
                            Title = "Notable Video Game Releases: New and Upcoming",
                            Summary = "Find release dates and scores for every major upcoming and recent video game release for all platforms, updated weekly.",
                            Content = "Detailed information about upcoming game releases including DOOM: The Dark Ages, Atomfall, The First Berserker: Khazan, and more...",
                            ImageUrl = "/images/news/upcoming-games.jpg",
                            CreatedAt = DateTime.Now.AddDays(-2),
                            GameCategoryId = 1
                        },
                        new NewsPost
                        {
                            Title = "New Free & Subscription Games for All Platforms",
                            Summary = "Our frequently updated list shows the latest free games available from Epic Games Store, Steam, GOG, and more as well as new titles added to Game Pass, PlayStation Plus.",
                            Content = "Latest free games and subscription updates across all major gaming platforms...",
                            ImageUrl = "/images/news/free-games.jpg",
                            CreatedAt = DateTime.Now.AddDays(-6),
                            GameCategoryId = 1
                        },
                        new NewsPost
                        {
                            Title = "Monster Hunter Wilds - First Look and Release Date",
                            Summary = "Get an exclusive first look at the upcoming Monster Hunter Wilds, including gameplay features, new monsters, and confirmed release date.",
                            Content = "Detailed preview of Monster Hunter Wilds featuring new gameplay mechanics...",
                            ImageUrl = "/images/news/monster-hunter.jpg",
                            CreatedAt = DateTime.Now.AddDays(-1),
                            GameCategoryId = 1
                        },
                        new NewsPost
                        {
                            Title = "Assassin's Creed Shadows Revealed",
                            Summary = "Ubisoft unveils the next chapter in the Assassin's Creed saga with a brand new setting and protagonist.",
                            Content = "Full details about the new Assassin's Creed game...",
                            ImageUrl = "/images/news/ac-shadows.jpg",
                            CreatedAt = DateTime.Now.AddHours(-12),
                            GameCategoryId = 1
                        },
                        new NewsPost
                        {
                            Title = "Xbox Game Pass - April 2025 Games Announced",
                            Summary = "Microsoft reveals the next wave of games coming to Xbox Game Pass, including several day-one releases.",
                            Content = "Complete list of new Xbox Game Pass additions...",
                            ImageUrl = "/images/news/gamepass-april.jpg",
                            CreatedAt = DateTime.Now.AddDays(-3),
                            GameCategoryId = 1
                        }
                    };

                    _context.NewsPosts.AddRange(sampleNews);
                    _context.SaveChanges();
                }

                // Tạo truy vấn cơ bản
                IQueryable<NewsPost> newsPosts = _context.NewsPosts
                    .Include(n => n.GameCategory)
                    .AsNoTracking()
                    .OrderByDescending(n => n.CreatedAt);

                // Áp dụng tìm kiếm nếu có
                if (!string.IsNullOrEmpty(search))
                {
                    newsPosts = newsPosts.Where(n => n.Title.Contains(search) || 
                                                   n.Summary.Contains(search) || 
                                                   n.Content.Contains(search));
                }

                // Lọc theo danh mục nếu có
                if (categoryId.HasValue)
                {
                    newsPosts = newsPosts.Where(n => n.GameCategoryId == categoryId);
                }

                // Lấy danh sách bài viết và danh mục
                var newsPostsList = newsPosts.ToList();
                var categories = _context.GameCategories?.ToList() ?? new List<GameCategory>();

                // Truyền dữ liệu sang view
                ViewBag.Categories = categories;
                ViewBag.CurrentSearch = search;
                ViewBag.CurrentCategoryId = categoryId;

                return View(newsPostsList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in News Index action");
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    Message = "Có lỗi xảy ra khi tải tin tức. Vui lòng thử lại sau."
                });
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var post = await _context.NewsPosts.FindAsync(id);
            if (post == null) return NotFound();
            return View(post);
        }

        // GET: News/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: News/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Title,Content,ImageUrl,Summary,GameCategoryId")] NewsPost newsPost)
        {
            if (ModelState.IsValid)
            {
                newsPost.CreatedAt = DateTime.Now;
                _context.Add(newsPost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(newsPost);
        }

        // GET: News/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsPost = await _context.NewsPosts.FindAsync(id);
            if (newsPost == null)
            {
                return NotFound();
            }
            return View(newsPost);
        }

        // POST: News/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,ImageUrl,CreatedAt,Summary,GameCategoryId")] NewsPost newsPost)
        {
            if (id != newsPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(newsPost);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsPostExists(newsPost.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(newsPost);
        }

        // GET: News/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsPost = await _context.NewsPosts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (newsPost == null)
            {
                return NotFound();
            }

            return View(newsPost);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var newsPost = await _context.NewsPosts.FindAsync(id);
            if (newsPost != null)
            {
                _context.NewsPosts.Remove(newsPost);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NewsPostExists(int id)
        {
            return _context.NewsPosts.Any(e => e.Id == id);
        }
    }
}
