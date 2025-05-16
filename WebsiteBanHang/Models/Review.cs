using System;
using System.ComponentModel.DataAnnotations;

namespace WebGame.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public int GameId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [Range(0, 10)]
        public int Score { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime ReviewDate { get; set; }

        public bool IsCriticReview { get; set; }

        public int HelpfulCount { get; set; }
        public int UnhelpfulCount { get; set; }

        // Navigation properties
        public virtual Game Game { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
} 