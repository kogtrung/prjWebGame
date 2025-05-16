using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;

namespace WebGame.Models
{
    public class Game
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        public string Platform 
        { 
            get 
            {
                if (GamePlatforms != null)
                {
                    List<string> platformNames = new List<string>();
                    foreach (var gp in GamePlatforms)
                    {
                        if (gp.Platform != null)
                        {
                            platformNames.Add(gp.Platform.Name);
                        }
                        else
                        {
                            platformNames.Add("Unknown");
                        }
                    }
                    
                    if (platformNames.Count > 0)
                    {
                        return string.Join(", ", platformNames);
                    }
                }
                return "Not specified";
            }
            set { } // Empty set để EF Core không complain
        }

        [Required]
        public string Description { get; set; }

        // Make ImageUrl nullable to allow setting a default
        public string? ImageUrl { get; set; }
        
        // URL của video giới thiệu game (YouTube hoặc nguồn khác)
        public string? VideoUrl { get; set; }

        // URL của trailer game (YouTube hoặc nguồn khác)
        public string? TrailerUrl { get; set; }

        // Danh sách các URL ảnh screenshot của game
        public List<string>? Screenshots { get; set; }

        [Required]
        [Range(0, 100)]
        public int MetaScore { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ReleaseDate { get; set; }

        [Required]
        public string Genre { get; set; }

        [Required]
        public string Developer { get; set; }

        [Required]
        public string Publisher { get; set; }

        // Review statistics
        public int UserScore { get; set; }
        public int ReviewCount { get; set; }
        public int UserReviewCount { get; set; }

        public float TrendingScore { get; set; }

        public string? Screenshot { get; set; }

        // Add Rating property
        [Required]
        public string Rating { get; set; }  // E, T, M, etc.

        // Must Play badge
        public bool MustPlay { get; set; }

        
        // Navigation properties - khởi tạo để tránh lỗi "required"
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<GamePlatform> GamePlatforms { get; set; } = new List<GamePlatform>();
    }
} 