using System.Collections.Generic;
using WebGame.Models;

namespace WebGame.Models
{
    public class HomeViewModel
    {
        public List<Game> LatestGames { get; set; } = new List<Game>();
        public List<Game> TopRatedGames { get; set; } = new List<Game>();
        public List<Game> TopGames { get; set; } = new List<Game>();
        public List<NewsPost> LatestNews { get; set; } = new List<NewsPost>();
    }
} 