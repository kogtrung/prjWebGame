using Microsoft.EntityFrameworkCore;
using WebGame.Models;

namespace WebGame.Data
{
    public static class GameCategorySeeder
    {
        public static void SeedGameCategories(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameCategory>().HasData(
                new GameCategory { Id = 1, Name = "Action", Description = "Action games focus on physical challenges, including hand–eye coordination and reaction-time." },
                new GameCategory { Id = 2, Name = "Adventure", Description = "Adventure games focus on puzzle solving within a narrative framework." },
                new GameCategory { Id = 3, Name = "RPG", Description = "Role-playing games where players assume the roles of characters in a fictional setting." },
                new GameCategory { Id = 4, Name = "Strategy", Description = "Strategy games focus on skillful thinking and planning to achieve victory." },
                new GameCategory { Id = 5, Name = "Sports", Description = "Sports games simulate the practice of traditional physical sports." },
                new GameCategory { Id = 6, Name = "Racing", Description = "Racing games involve the player participating in racing competitions." },
                new GameCategory { Id = 7, Name = "Simulation", Description = "Simulation games are designed to simulate real world activities." },
                new GameCategory { Id = 8, Name = "Fighting", Description = "Fighting games emphasize one-on-one combat between characters." }
            );
        }
    }
} 