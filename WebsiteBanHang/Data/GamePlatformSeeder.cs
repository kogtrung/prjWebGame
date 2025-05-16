using Microsoft.EntityFrameworkCore;
using WebGame.Models;

namespace WebGame.Data
{
    public static class GamePlatformSeeder
    {
        public static void SeedGamePlatforms(ModelBuilder modelBuilder)
        {
            // God of War (9)
            modelBuilder.Entity<GamePlatform>().HasData(
                new GamePlatform { Id = 14, GameId = 9, PlatformId = 2, ReleaseDate = new DateTime(2018, 4, 20) },  // PS4
                new GamePlatform { Id = 15, GameId = 9, PlatformId = 11, ReleaseDate = new DateTime(2022, 1, 14) }  // PC
            );

            // The Witcher 3 (10)
            modelBuilder.Entity<GamePlatform>().HasData(
                new GamePlatform { Id = 16, GameId = 10, PlatformId = 2, ReleaseDate = new DateTime(2015, 5, 19) },  // PS4
                new GamePlatform { Id = 17, GameId = 10, PlatformId = 5, ReleaseDate = new DateTime(2015, 5, 19) },  // Xbox One
                new GamePlatform { Id = 18, GameId = 10, PlatformId = 11, ReleaseDate = new DateTime(2015, 5, 19) },  // PC
                new GamePlatform { Id = 19, GameId = 10, PlatformId = 7, ReleaseDate = new DateTime(2019, 10, 15) }  // Switch
            );

            // Persona 5 Royal (11)
            modelBuilder.Entity<GamePlatform>().HasData(
                new GamePlatform { Id = 20, GameId = 11, PlatformId = 2, ReleaseDate = new DateTime(2020, 3, 31) },  // PS4
                new GamePlatform { Id = 21, GameId = 11, PlatformId = 7, ReleaseDate = new DateTime(2022, 10, 21) }  // Switch
            );

            // Elden Ring (12)
            modelBuilder.Entity<GamePlatform>().HasData(
                new GamePlatform { Id = 22, GameId = 12, PlatformId = 1, ReleaseDate = new DateTime(2022, 2, 25) },  // PS5
                new GamePlatform { Id = 23, GameId = 12, PlatformId = 2, ReleaseDate = new DateTime(2022, 2, 25) },  // PS4
                new GamePlatform { Id = 24, GameId = 12, PlatformId = 4, ReleaseDate = new DateTime(2022, 2, 25) },  // Xbox Series X|S
                new GamePlatform { Id = 25, GameId = 12, PlatformId = 5, ReleaseDate = new DateTime(2022, 2, 25) },  // Xbox One
                new GamePlatform { Id = 26, GameId = 12, PlatformId = 11, ReleaseDate = new DateTime(2022, 2, 25) }  // PC
            );

            // Final Fantasy VII Remake (13)
            modelBuilder.Entity<GamePlatform>().HasData(
                new GamePlatform { Id = 27, GameId = 13, PlatformId = 2, ReleaseDate = new DateTime(2020, 4, 10) },  // PS4
                new GamePlatform { Id = 28, GameId = 13, PlatformId = 11, ReleaseDate = new DateTime(2021, 12, 16) }  // PC
            );

            // Cyberpunk 2077 (14)
            modelBuilder.Entity<GamePlatform>().HasData(
                new GamePlatform { Id = 29, GameId = 14, PlatformId = 1, ReleaseDate = new DateTime(2020, 12, 10) },  // PS5
                new GamePlatform { Id = 30, GameId = 14, PlatformId = 2, ReleaseDate = new DateTime(2020, 12, 10) },  // PS4
                new GamePlatform { Id = 31, GameId = 14, PlatformId = 4, ReleaseDate = new DateTime(2020, 12, 10) },  // Xbox Series X|S
                new GamePlatform { Id = 32, GameId = 14, PlatformId = 5, ReleaseDate = new DateTime(2020, 12, 10) },  // Xbox One
                new GamePlatform { Id = 33, GameId = 14, PlatformId = 11, ReleaseDate = new DateTime(2020, 12, 10) }  // PC
            );

            // Ghost of Tsushima (15)
            modelBuilder.Entity<GamePlatform>().HasData(
                new GamePlatform { Id = 34, GameId = 15, PlatformId = 1, ReleaseDate = new DateTime(2021, 8, 20) },  // PS5
                new GamePlatform { Id = 35, GameId = 15, PlatformId = 2, ReleaseDate = new DateTime(2020, 7, 17) }   // PS4
            );

            // Horizon Zero Dawn (16)
            modelBuilder.Entity<GamePlatform>().HasData(
                new GamePlatform { Id = 36, GameId = 16, PlatformId = 2, ReleaseDate = new DateTime(2017, 2, 28) },  // PS4
                new GamePlatform { Id = 37, GameId = 16, PlatformId = 11, ReleaseDate = new DateTime(2020, 8, 7) }   // PC
            );

            // Sekiro: Shadows Die Twice (17)
            modelBuilder.Entity<GamePlatform>().HasData(
                new GamePlatform { Id = 38, GameId = 17, PlatformId = 2, ReleaseDate = new DateTime(2019, 3, 22) },  // PS4
                new GamePlatform { Id = 39, GameId = 17, PlatformId = 5, ReleaseDate = new DateTime(2019, 3, 22) },  // Xbox One
                new GamePlatform { Id = 40, GameId = 17, PlatformId = 11, ReleaseDate = new DateTime(2019, 3, 22) }  // PC
            );

            // Hades (18)
            modelBuilder.Entity<GamePlatform>().HasData(
                new GamePlatform { Id = 41, GameId = 18, PlatformId = 2, ReleaseDate = new DateTime(2021, 8, 13) },  // PS4
                new GamePlatform { Id = 42, GameId = 18, PlatformId = 1, ReleaseDate = new DateTime(2021, 8, 13) },  // PS5
                new GamePlatform { Id = 43, GameId = 18, PlatformId = 5, ReleaseDate = new DateTime(2021, 8, 13) },  // Xbox One
                new GamePlatform { Id = 44, GameId = 18, PlatformId = 4, ReleaseDate = new DateTime(2021, 8, 13) },  // Xbox Series X|S
                new GamePlatform { Id = 45, GameId = 18, PlatformId = 7, ReleaseDate = new DateTime(2020, 9, 17) },  // Switch
                new GamePlatform { Id = 46, GameId = 18, PlatformId = 11, ReleaseDate = new DateTime(2020, 9, 17) }  // PC
            );

            // Death Stranding (19)
            modelBuilder.Entity<GamePlatform>().HasData(
                new GamePlatform { Id = 47, GameId = 19, PlatformId = 2, ReleaseDate = new DateTime(2019, 11, 8) },  // PS4
                new GamePlatform { Id = 48, GameId = 19, PlatformId = 11, ReleaseDate = new DateTime(2020, 7, 14) }  // PC
            );

            // It Takes Two (20)
            modelBuilder.Entity<GamePlatform>().HasData(
                new GamePlatform { Id = 49, GameId = 20, PlatformId = 1, ReleaseDate = new DateTime(2021, 3, 26) },  // PS5
                new GamePlatform { Id = 50, GameId = 20, PlatformId = 2, ReleaseDate = new DateTime(2021, 3, 26) },  // PS4
                new GamePlatform { Id = 51, GameId = 20, PlatformId = 4, ReleaseDate = new DateTime(2021, 3, 26) },  // Xbox Series X|S
                new GamePlatform { Id = 52, GameId = 20, PlatformId = 5, ReleaseDate = new DateTime(2021, 3, 26) },  // Xbox One
                new GamePlatform { Id = 53, GameId = 20, PlatformId = 11, ReleaseDate = new DateTime(2021, 3, 26) }  // PC
            );

            // Monster Hunter Rise (21)
            modelBuilder.Entity<GamePlatform>().HasData(
                new GamePlatform { Id = 54, GameId = 21, PlatformId = 7, ReleaseDate = new DateTime(2021, 3, 26) },  // Switch
                new GamePlatform { Id = 55, GameId = 21, PlatformId = 11, ReleaseDate = new DateTime(2022, 1, 12) },  // PC
                new GamePlatform { Id = 56, GameId = 21, PlatformId = 1, ReleaseDate = new DateTime(2023, 1, 20) },  // PS5
                new GamePlatform { Id = 57, GameId = 21, PlatformId = 2, ReleaseDate = new DateTime(2023, 1, 20) },  // PS4
                new GamePlatform { Id = 58, GameId = 21, PlatformId = 4, ReleaseDate = new DateTime(2023, 1, 20) },  // Xbox Series X|S
                new GamePlatform { Id = 59, GameId = 21, PlatformId = 5, ReleaseDate = new DateTime(2023, 1, 20) }   // Xbox One
            );

            // Deathloop (22)
            modelBuilder.Entity<GamePlatform>().HasData(
                new GamePlatform { Id = 60, GameId = 22, PlatformId = 1, ReleaseDate = new DateTime(2021, 9, 14) },  // PS5
                new GamePlatform { Id = 61, GameId = 22, PlatformId = 11, ReleaseDate = new DateTime(2021, 9, 14) },  // PC
                new GamePlatform { Id = 62, GameId = 22, PlatformId = 4, ReleaseDate = new DateTime(2022, 9, 20) }   // Xbox Series X|S
            );

            // Ratchet & Clank: Rift Apart (23)
            modelBuilder.Entity<GamePlatform>().HasData(
                new GamePlatform { Id = 63, GameId = 23, PlatformId = 1, ReleaseDate = new DateTime(2021, 6, 11) },  // PS5
                new GamePlatform { Id = 64, GameId = 23, PlatformId = 11, ReleaseDate = new DateTime(2023, 7, 26) }  // PC
            );

            // Demon's Souls (24)
            modelBuilder.Entity<GamePlatform>().HasData(
                new GamePlatform { Id = 65, GameId = 24, PlatformId = 1, ReleaseDate = new DateTime(2020, 11, 12) }  // PS5
            );
        }
    }
} 