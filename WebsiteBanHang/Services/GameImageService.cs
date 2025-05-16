using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebGame.Services
{
    public interface IGameImageService
    {
        string GetImageUrlByTitle(string title);
        Task<bool> RefreshGameImagesAsync();
        string GetRandomGameImage();
    }

    public class GameImageService : IGameImageService
    {
        private readonly ILogger<GameImageService> _logger;
        private readonly Dictionary<string, string> _imageMapping;
        private readonly Random _random;

        public GameImageService(ILogger<GameImageService> logger)
        {
            _logger = logger;
            _imageMapping = InitializeImageMapping();
            _random = new Random();
        }

        public string GetImageUrlByTitle(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return GetDefaultImageUrl();
            }

            // Tìm kiếm trong mapping trước (không phân biệt hoa thường)
            string lowerTitle = title.ToLower();
            var key = _imageMapping.Keys.FirstOrDefault(k => 
                k.ToLower().Equals(lowerTitle) || 
                lowerTitle.Contains(k.ToLower()) ||
                k.ToLower().Contains(lowerTitle));

            if (key != null)
            {
                _logger.LogInformation($"Found image URL for '{title}' using key '{key}'");
                return _imageMapping[key];
            }

            // Nếu không tìm thấy, trả về ảnh mặc định
            _logger.LogWarning($"No image found for game: {title}, using default image");
            return GetDefaultImageUrl();
        }

        private string GetDefaultImageUrl()
        {
            return "https://via.placeholder.com/600x400?text=Game+Image+Not+Found";
        }

        private Dictionary<string, string> InitializeImageMapping()
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // Top-rated games
                { "Baldur's Gate 3", "https://image.api.playstation.com/vulcan/ap/rnd/202302/2321/2fa247a12223368ad30db6b6dedcede2b23fc8dbb92575.jpg" },
                { "The Legend of Zelda: Tears of the Kingdom", "https://assets.nintendo.com/image/upload/ar_16:9,c_lpad,w_1240/b_white/f_auto/q_auto/ncom/software/switch/70010000063714/276a412988e07c4d55a2996c6d38abb408b464413fb629888f2ec4d1fa628361" },
                { "Elden Ring", "https://image.api.playstation.com/vulcan/ap/rnd/202108/0410/0Imhn60XcUSstCELZQE7FTxW.png" },
                { "God of War Ragnarök", "https://image.api.playstation.com/vulcan/ap/rnd/202207/1210/4xJ8XB3bi888QTLZYdl7Oi0s.png" },
                { "Final Fantasy VII Rebirth", "https://image.api.playstation.com/vulcan/ap/rnd/202306/0215/d43071186e987e66d3ed258dd57ac05fcc1a1cfd98bed3cd.jpg" },
                
                // New releases
                { "Helldivers 2", "https://image.api.playstation.com/vulcan/ap/rnd/202211/1117/M8MVwoME0926k9TUlD7jrBrj.jpg" },
                { "Spider-Man 2", "https://image.api.playstation.com/vulcan/ap/rnd/202306/1219/1c7b42e972197abf134325730347f5bc823f2867818a16bc.jpg" },
                { "Stellar Blade", "https://image.api.playstation.com/vulcan/ap/rnd/202401/1617/9d09996e978bbf63ff00b8f7d36be61eb0a61bc0c95a9066.jpg" },
                { "Hogwarts Legacy", "https://cdn.cloudflare.steamstatic.com/steam/apps/990080/header.jpg" },
                { "Star Wars Jedi: Survivor", "https://cdn.cloudflare.steamstatic.com/steam/apps/1774580/header.jpg" },
                
                // More popular games
                { "Red Dead Redemption 2", "https://cdn.cloudflare.steamstatic.com/steam/apps/1174180/header.jpg" },
                { "The Witcher 3", "https://cdn.cloudflare.steamstatic.com/steam/apps/292030/header.jpg" },
                { "Cyberpunk 2077", "https://cdn.cloudflare.steamstatic.com/steam/apps/1091500/header.jpg" },
                { "Hades", "https://cdn.cloudflare.steamstatic.com/steam/apps/1145360/header.jpg" },
                { "Disco Elysium", "https://cdn.cloudflare.steamstatic.com/steam/apps/632470/header.jpg" },
                
                // Upcoming games
                { "Elden Ring: Shadow of the Erdtree", "https://assets.xboxservices.com/assets/32/cb/32cb3def-6165-40d1-aef2-6ade59201cfa.jpg" },
                { "Dragon Age: The Veilguard", "https://cdn.cloudflare.steamstatic.com/steam/apps/2420110/header.jpg" },
                { "Avowed", "https://cdn1.epicgames.com/offer/953137e2c14d4558a2eadb835a46456d/EGS_Avowed_ObsidianEntertainment_S2_1200x1600-85f9ace011e466a976fdf521999c7af8" },
                { "Final Fantasy XVI", "https://cdn.cloudflare.steamstatic.com/steam/apps/1817950/header.jpg" },
                { "Metaphor: ReFantazio", "https://cdn.cloudflare.steamstatic.com/steam/apps/2324650/header.jpg" }
            };
        }

        public async Task<bool> RefreshGameImagesAsync()
        {
            try
            {
                _logger.LogInformation("Refreshing game images mapping...");
                
                // Cập nhật lại _imageMapping với dữ liệu mới nhất
                // Trong trường hợp thực tế, có thể tải dữ liệu từ API hoặc cơ sở dữ liệu
                await Task.Delay(100); // Giả lập thời gian tải

                // Khởi tạo lại mapping 
                var refreshedMapping = InitializeImageMapping();
                
                // Thêm một số mapping mới (chỉ để minh họa, trong thực tế sẽ tải từ nguồn dữ liệu)
                refreshedMapping["Black Myth: Wukong"] = "https://cdn.cloudflare.steamstatic.com/steam/apps/2358720/header.jpg";
                refreshedMapping["Dragon Age: The Veilguard"] = "https://cdn.cloudflare.steamstatic.com/steam/apps/2420110/header.jpg";
                refreshedMapping["Star Wars Outlaws"] = "https://cdn.akamai.steamstatic.com/steam/apps/2579240/header.jpg";
                
                _logger.LogInformation($"Successfully refreshed game images mapping. Total entries: {refreshedMapping.Count}");
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing game images mapping");
                return false;
            }
        }

        public string GetRandomGameImage()
        {
            try
            {
                // Get all image URLs from the mapping
                var imageUrls = _imageMapping.Values.ToList();
                
                if (imageUrls.Count == 0)
                    return GetDefaultImageUrl();
                
                // Get a random image URL
                int randomIndex = _random.Next(imageUrls.Count);
                string randomImageUrl = imageUrls[randomIndex];
                
                _logger.LogInformation($"Returning random game image: {randomImageUrl}");
                return randomImageUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting random game image");
                return GetDefaultImageUrl();
            }
        }
    }
} 