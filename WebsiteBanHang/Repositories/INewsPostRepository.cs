using WebGame.Models;

namespace WebGame.Repositories
{
    public interface INewsPostRepository
    {
        Task<IEnumerable<NewsPost>> GetAllAsync();
        Task<IEnumerable<NewsPost>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<NewsPost>> GetRecentPostsAsync(int count);
        Task<IEnumerable<NewsPost>> SearchAsync(string query);
        Task<NewsPost?> GetByIdAsync(int id);
        Task AddAsync(NewsPost newsPost);
        Task UpdateAsync(NewsPost newsPost);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<List<GameCategory>> GetAllCategoriesAsync();
    }
} 