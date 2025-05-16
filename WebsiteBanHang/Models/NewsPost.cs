using System;
using System.ComponentModel.DataAnnotations;

namespace WebGame.Models
{
    public class NewsPost
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime PublishDate { get; set; } = DateTime.Now;

        [StringLength(300)]
        public string? Summary { get; set; }

        public int? GameCategoryId { get; set; }
        public GameCategory? GameCategory { get; set; }
        
        [StringLength(50)]
        public string? Category { get; set; }
    }
}
