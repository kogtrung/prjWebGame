using System.ComponentModel.DataAnnotations;

namespace WebGame.Models
{
    public class GameCategory
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        // Liên kết đến các bài viết (nếu cần)
        public ICollection<NewsPost>? NewsPosts { get; set; }
    }
}
