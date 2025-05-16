using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebGame.Models;

namespace WebGame.Repositories
{
    public class EFNewsPostRepository : INewsPostRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        
        // Cache keys
        private const string AllPostsCacheKey = "AllNewsPosts";
        private const string RecentPostsCacheKey = "RecentNewsPosts";
        private const string CategoriesCacheKey = "GameCategories";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);
        
        public EFNewsPostRepository(ApplicationDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _cache = memoryCache;
        }
        
        public async Task<IEnumerable<NewsPost>> GetAllAsync()
        {
            // Try to get from cache first
            if (!_cache.TryGetValue(AllPostsCacheKey, out List<NewsPost> posts))
            {
                // If not in cache, get from database with optimized query
                posts = await _context.NewsPosts
                    .AsNoTracking()
                    .Include(p => p.GameCategory)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();
                
                // Cache the results with size information
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSize(posts.Count)
                    .SetAbsoluteExpiration(CacheDuration);
                
                _cache.Set(AllPostsCacheKey, posts, cacheEntryOptions);
            }
            
            return posts;
        }
        
        public async Task<IEnumerable<NewsPost>> GetByCategoryAsync(int categoryId)
        {
            string cacheKey = $"NewsPosts_Category_{categoryId}";
            
            if (!_cache.TryGetValue(cacheKey, out List<NewsPost> posts))
            {
                posts = await _context.NewsPosts
                    .AsNoTracking()
                    .Where(p => p.GameCategoryId == categoryId)
                    .Include(p => p.GameCategory)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();
                
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSize(posts.Count)
                    .SetAbsoluteExpiration(CacheDuration);
                
                _cache.Set(cacheKey, posts, cacheEntryOptions);
            }
            
            return posts;
        }
        
        public async Task<IEnumerable<NewsPost>> GetRecentPostsAsync(int count)
        {
            string cacheKey = $"{RecentPostsCacheKey}_{count}";
            
            if (!_cache.TryGetValue(cacheKey, out List<NewsPost> posts))
            {
                posts = await _context.NewsPosts
                    .AsNoTracking()
                    .Include(p => p.GameCategory)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(count)
                    .ToListAsync();
                
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSize(posts.Count)
                    .SetAbsoluteExpiration(CacheDuration);
                
                _cache.Set(cacheKey, posts, cacheEntryOptions);
            }
            
            return posts;
        }
        
        public async Task<IEnumerable<NewsPost>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return await GetAllAsync();
                
            // Don't cache search results to ensure freshness
            return await _context.NewsPosts
                .AsNoTracking()
                .Where(p => p.Title.Contains(query) || 
                            p.Summary.Contains(query) || 
                            p.Content.Contains(query))
                .Include(p => p.GameCategory)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
        
        public async Task<NewsPost?> GetByIdAsync(int id)
        {
            string cacheKey = $"NewsPost_{id}";
            
            if (!_cache.TryGetValue(cacheKey, out NewsPost? post))
            {
                post = await _context.NewsPosts
                    .AsNoTracking()
                    .Include(p => p.GameCategory)
                    .FirstOrDefaultAsync(p => p.Id == id);
                
                if (post != null)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSize(1)
                        .SetAbsoluteExpiration(CacheDuration);
                    
                    _cache.Set(cacheKey, post, cacheEntryOptions);
                }
            }
            
            return post;
        }
        
        public async Task AddAsync(NewsPost newsPost)
        {
            newsPost.CreatedAt = DateTime.Now;
            newsPost.PublishDate = DateTime.Now;
            
            _context.NewsPosts.Add(newsPost);
            await _context.SaveChangesAsync();
            
            // Invalidate caches
            InvalidateCache();
        }
        
        public async Task UpdateAsync(NewsPost newsPost)
        {
            // Get current post to preserve CreatedAt
            var existingPost = await _context.NewsPosts.FindAsync(newsPost.Id);
            if (existingPost == null) throw new KeyNotFoundException($"NewsPost with ID {newsPost.Id} not found");
            
            // Preserve the original creation date
            newsPost.CreatedAt = existingPost.CreatedAt;
            
            // Detach existing entity from tracking
            _context.Entry(existingPost).State = EntityState.Detached;
            
            // Mark as modified and update
            _context.Entry(newsPost).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
            // Invalidate caches
            InvalidateCache();
            _cache.Remove($"NewsPost_{newsPost.Id}");
        }
        
        public async Task DeleteAsync(int id)
        {
            var post = new NewsPost { Id = id };
            _context.Entry(post).State = EntityState.Deleted;
            await _context.SaveChangesAsync();
            
            // Invalidate caches
            InvalidateCache();
            _cache.Remove($"NewsPost_{id}");
        }
        
        public async Task<bool> ExistsAsync(int id)
        {
            // Check cache first
            if (_cache.TryGetValue($"NewsPost_{id}", out _))
                return true;
                
            return await _context.NewsPosts
                .AsNoTracking()
                .AnyAsync(p => p.Id == id);
        }
        
        public async Task<List<GameCategory>> GetAllCategoriesAsync()
        {
            if (!_cache.TryGetValue(CategoriesCacheKey, out List<GameCategory> categories))
            {
                categories = await _context.GameCategories
                    .AsNoTracking()
                    .OrderBy(c => c.Name)
                    .ToListAsync();
                
                // Create default categories if none exist
                if (!categories.Any())
                {
                    var defaultCategories = new List<GameCategory>
                    {
                        new GameCategory { Name = "Action" },
                        new GameCategory { Name = "Adventure" },
                        new GameCategory { Name = "RPG" },
                        new GameCategory { Name = "Strategy" },
                        new GameCategory { Name = "Simulation" },
                        new GameCategory { Name = "Sports" },
                        new GameCategory { Name = "Racing" },
                        new GameCategory { Name = "Puzzle" },
                        new GameCategory { Name = "Fighting" },
                        new GameCategory { Name = "Shooter" },
                        new GameCategory { Name = "Platformer" },
                        new GameCategory { Name = "Survival Horror" },
                        new GameCategory { Name = "MMORPG" },
                        new GameCategory { Name = "News" },
                        new GameCategory { Name = "Reviews" },
                        new GameCategory { Name = "Industry" }
                    };
                    
                    _context.GameCategories.AddRange(defaultCategories);
                    await _context.SaveChangesAsync();
                    
                    categories = defaultCategories;
                }
                
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSize(categories.Count)
                    .SetAbsoluteExpiration(CacheDuration);
                
                _cache.Set(CategoriesCacheKey, categories, cacheEntryOptions);
            }
            
            return categories;
        }
        
        // Helper method to invalidate all caches
        private void InvalidateCache()
        {
            _cache.Remove(AllPostsCacheKey);
            _cache.Remove(RecentPostsCacheKey);
            
            // Remove specific category caches - use known patterns instead of reflection
            for (int i = 1; i <= 20; i++) // Assume max 20 categories
            {
                _cache.Remove($"NewsPosts_Category_{i}");
            }
        }
    }
} 