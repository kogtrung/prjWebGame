using Microsoft.EntityFrameworkCore;
using WebGame.Models;

namespace WebGame.Data
{
    public static class PlatformSeeder
    {
        public static void SeedPlatforms(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Platform>().HasData(
                new Platform { Id = 1, Name = "PlayStation 5", Manufacturer = "Sony", ReleaseDate = new DateTime(2020, 11, 12) },
                new Platform { Id = 2, Name = "PlayStation 4", Manufacturer = "Sony", ReleaseDate = new DateTime(2013, 11, 15) },
                new Platform { Id = 3, Name = "PlayStation 3", Manufacturer = "Sony", ReleaseDate = new DateTime(2006, 11, 11) },
                new Platform { Id = 4, Name = "Xbox Series X|S", Manufacturer = "Microsoft", ReleaseDate = new DateTime(2020, 11, 10) },
                new Platform { Id = 5, Name = "Xbox One", Manufacturer = "Microsoft", ReleaseDate = new DateTime(2013, 11, 22) },
                new Platform { Id = 6, Name = "Xbox 360", Manufacturer = "Microsoft", ReleaseDate = new DateTime(2005, 11, 22) },
                new Platform { Id = 7, Name = "Nintendo Switch", Manufacturer = "Nintendo", ReleaseDate = new DateTime(2017, 3, 3) },
                new Platform { Id = 8, Name = "Nintendo Wii U", Manufacturer = "Nintendo", ReleaseDate = new DateTime(2012, 11, 18) },
                new Platform { Id = 9, Name = "Nintendo Wii", Manufacturer = "Nintendo", ReleaseDate = new DateTime(2006, 11, 19) },
                new Platform { Id = 10, Name = "Nintendo 3DS", Manufacturer = "Nintendo", ReleaseDate = new DateTime(2011, 2, 26) },
                new Platform { Id = 11, Name = "PC", Manufacturer = "Various", ReleaseDate = null },
                new Platform { Id = 12, Name = "Nintendo 64", Manufacturer = "Nintendo", ReleaseDate = new DateTime(1996, 6, 23) },
                new Platform { Id = 13, Name = "PlayStation", Manufacturer = "Sony", ReleaseDate = new DateTime(1994, 12, 3) },
                new Platform { Id = 14, Name = "PlayStation 2", Manufacturer = "Sony", ReleaseDate = new DateTime(2000, 3, 4) }
            );
        }
    }
} 