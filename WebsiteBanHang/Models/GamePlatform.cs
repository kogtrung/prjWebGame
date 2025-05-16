using System.ComponentModel.DataAnnotations;

namespace WebGame.Models
{
    public class GamePlatform
    {
        public int Id { get; set; }

        [Required]
        public int GameId { get; set; }

        [Required]
        public int PlatformId { get; set; }

        public DateTime ReleaseDate { get; set; }

        // Navigation properties
        public virtual Game Game { get; set; }
        public virtual Platform Platform { get; set; }
    }

    public class Platform
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Manufacturer { get; set; }

        public DateTime? ReleaseDate { get; set; }

        // Navigation property
        public virtual ICollection<GamePlatform> GamePlatforms { get; set; }
    }
} 