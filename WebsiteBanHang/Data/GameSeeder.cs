using Microsoft.EntityFrameworkCore;
using WebGame.Models;

namespace WebGame.Data
{
    public static class GameSeeder
    {
        public static void SeedGames(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>().HasData(
                new Game
                {
                    Id = 1,
                    Title = "Baldur's Gate 3",
                    Description = "Baldur's Gate 3 is a role-playing game developed and published by Larian Studios. Choose from a wide range of D&D races and classes, or play as an origin character with a hand-crafted background.",
                    ImageUrl = "/images/games/baldurs-gate-3.jpg",
                    MetaScore = 96,
                    Genre = "RPG",
                    Developer = "Larian Studios",
                    Publisher = "Larian Studios",
                    Rating = "M",
                    ReleaseDate = new DateTime(2023, 8, 3)
                },
                new Game
                {
                    Id = 2,
                    Title = "Marvel's Spider-Man 2",
                    Description = "Marvel's Spider-Man 2 is an action-adventure game featuring both Peter Parker and Miles Morales.",
                    ImageUrl = "/images/games/spiderman2.jpg",
                    MetaScore = 90,
                    Genre = "Action-Adventure",
                    Developer = "Insomniac Games",
                    Publisher = "Sony Interactive Entertainment",
                    Rating = "T",
                    ReleaseDate = new DateTime(2023, 10, 20)
                },
                new Game
                {
                    Id = 3,
                    Title = "Resident Evil 4 Remake",
                    Description = "Resident Evil 4 is a survival horror game developed and published by Capcom.",
                    ImageUrl = "/images/games/re4.jpg",
                    MetaScore = 93,
                    Genre = "Survival Horror",
                    Developer = "Capcom",
                    Publisher = "Capcom",
                    Rating = "M",
                    ReleaseDate = new DateTime(2023, 3, 24)
                },
                new Game
                {
                    Id = 4,
                    Title = "The Legend of Zelda: Tears of the Kingdom",
                    Description = "The Legend of Zelda: Tears of the Kingdom is an action-adventure game developed and published by Nintendo, the sequel to Breath of the Wild.",
                    ImageUrl = "/images/games/zelda-totk.jpg",
                    MetaScore = 96,
                    Genre = "Action-Adventure",
                    Developer = "Nintendo",
                    Publisher = "Nintendo",
                    Rating = "E10+",
                    ReleaseDate = new DateTime(2023, 5, 12)
                },
                new Game
                {
                    Id = 5,
                    Title = "Final Fantasy XVI",
                    Description = "Final Fantasy XVI is an action role-playing game developed and published by Square Enix.",
                    ImageUrl = "/images/games/ff16.jpg",
                    MetaScore = 87,
                    Genre = "Action RPG",
                    Developer = "Square Enix",
                    Publisher = "Square Enix",
                    Rating = "M",
                    ReleaseDate = new DateTime(2023, 6, 22)
                },
                new Game
                {
                    Id = 6,
                    Title = "Star Wars Jedi: Survivor",
                    Description = "Star Wars Jedi: Survivor is an action-adventure game developed by Respawn Entertainment.",
                    ImageUrl = "/images/games/jedi-survivor.jpg",
                    MetaScore = 85,
                    Genre = "Action-Adventure",
                    Developer = "Respawn Entertainment",
                    Publisher = "Electronic Arts",
                    Rating = "T",
                    ReleaseDate = new DateTime(2023, 4, 28)
                },
                new Game
                {
                    Id = 7,
                    Title = "Dead Space Remake",
                    Description = "Dead Space is a survival horror game developed by Motive Studio and published by Electronic Arts.",
                    ImageUrl = "/images/games/dead-space.jpg",
                    MetaScore = 89,
                    Genre = "Survival Horror",
                    Developer = "Motive Studio",
                    Publisher = "Electronic Arts",
                    Rating = "M",
                    ReleaseDate = new DateTime(2023, 1, 27)
                },
                new Game
                {
                    Id = 8,
                    Title = "Hi-Fi Rush",
                    Description = "Hi-Fi Rush is a rhythm-action game developed by Tango Gameworks and published by Bethesda Softworks.",
                    ImageUrl = "/images/games/hifi-rush.jpg",
                    MetaScore = 88,
                    Genre = "Action Rhythm",
                    Developer = "Tango Gameworks",
                    Publisher = "Bethesda Softworks",
                    Rating = "T",
                    ReleaseDate = new DateTime(2023, 1, 25)
                },
                new Game
                {
                    Id = 9,
                    Title = "Street Fighter 6",
                    Description = "Street Fighter 6 is a fighting game developed and published by Capcom.",
                    ImageUrl = "/images/games/sf6.jpg",
                    MetaScore = 92,
                    Genre = "Fighting",
                    Developer = "Capcom",
                    Publisher = "Capcom",
                    Rating = "T",
                    ReleaseDate = new DateTime(2023, 6, 2)
                },
                new Game
                {
                    Id = 10,
                    Title = "Diablo IV",
                    Description = "Diablo IV is an action role-playing game developed and published by Blizzard Entertainment.",
                    ImageUrl = "/images/games/diablo4.jpg",
                    MetaScore = 88,
                    Genre = "Action RPG",
                    Developer = "Blizzard Entertainment",
                    Publisher = "Blizzard Entertainment",
                    Rating = "M",
                    ReleaseDate = new DateTime(2023, 6, 6)
                },
                new Game
                {
                    Id = 11,
                    Title = "Lies of P",
                    Description = "Lies of P is a souls-like action RPG inspired by the story of Pinocchio.",
                    ImageUrl = "/images/games/lies-of-p.jpg",
                    MetaScore = 84,
                    Genre = "Action RPG",
                    Developer = "Neowiz Games",
                    Publisher = "Neowiz Games",
                    Rating = "M",
                    ReleaseDate = new DateTime(2023, 9, 19)
                },
                new Game
                {
                    Id = 12,
                    Title = "Armored Core VI",
                    Description = "Armored Core VI: Fires of Rubicon is an action game developed by FromSoftware.",
                    ImageUrl = "/images/games/armored-core-6.jpg",
                    MetaScore = 87,
                    Genre = "Action",
                    Developer = "FromSoftware",
                    Publisher = "Bandai Namco",
                    Rating = "T",
                    ReleaseDate = new DateTime(2023, 8, 25)
                },
                new Game
                {
                    Id = 13,
                    Title = "Sea of Stars",
                    Description = "Sea of Stars is a turn-based RPG inspired by classic 16-bit RPGs.",
                    ImageUrl = "/images/games/sea-of-stars.jpg",
                    MetaScore = 89,
                    Genre = "RPG",
                    Developer = "Sabotage Studio",
                    Publisher = "Sabotage Studio",
                    Rating = "E10+",
                    ReleaseDate = new DateTime(2023, 8, 29)
                },
                new Game
                {
                    Id = 14,
                    Title = "Starfield",
                    Description = "Starfield is an action role-playing game developed by Bethesda Game Studios.",
                    ImageUrl = "/images/games/starfield.jpg",
                    MetaScore = 83,
                    Genre = "Action RPG",
                    Developer = "Bethesda Game Studios",
                    Publisher = "Bethesda Softworks",
                    Rating = "M",
                    ReleaseDate = new DateTime(2023, 9, 6)
                },
                new Game
                {
                    Id = 15,
                    Title = "Alan Wake 2",
                    Description = "Alan Wake 2 is a survival horror game developed by Remedy Entertainment.",
                    ImageUrl = "/images/games/alan-wake-2.jpg",
                    MetaScore = 89,
                    Genre = "Survival Horror",
                    Developer = "Remedy Entertainment",
                    Publisher = "Epic Games",
                    Rating = "M",
                    ReleaseDate = new DateTime(2023, 10, 27)
                },
                new Game
                {
                    Id = 16,
                    Title = "Super Mario Bros. Wonder",
                    Description = "Super Mario Bros. Wonder is a 2D side-scrolling platform game developed by Nintendo.",
                    ImageUrl = "/images/games/mario-wonder.jpg",
                    MetaScore = 92,
                    Genre = "Platform",
                    Developer = "Nintendo",
                    Publisher = "Nintendo",
                    Rating = "E",
                    ReleaseDate = new DateTime(2023, 10, 20)
                },
                new Game
                {
                    Id = 17,
                    Title = "Octopath Traveler II",
                    Description = "Octopath Traveler II is a turn-based RPG featuring eight different characters with unique stories.",
                    ImageUrl = "/images/games/octopath2.jpg",
                    MetaScore = 87,
                    Genre = "RPG",
                    Developer = "Square Enix",
                    Publisher = "Square Enix",
                    Rating = "T",
                    ReleaseDate = new DateTime(2023, 2, 24)
                },
                new Game
                {
                    Id = 18,
                    Title = "Assassin's Creed Mirage",
                    Description = "Assassin's Creed Mirage is an action-adventure game set in 9th century Baghdad.",
                    ImageUrl = "/images/games/ac-mirage.jpg",
                    MetaScore = 77,
                    Genre = "Action-Adventure",
                    Developer = "Ubisoft Bordeaux",
                    Publisher = "Ubisoft",
                    Rating = "M",
                    ReleaseDate = new DateTime(2023, 10, 5)
                },
                new Game
                {
                    Id = 19,
                    Title = "Mortal Kombat 1",
                    Description = "Mortal Kombat 1 is a fighting game developed by NetherRealm Studios.",
                    ImageUrl = "/images/games/mk1.jpg",
                    MetaScore = 84,
                    Genre = "Fighting",
                    Developer = "NetherRealm Studios",
                    Publisher = "Warner Bros. Games",
                    Rating = "M",
                    ReleaseDate = new DateTime(2023, 9, 19)
                },
                new Game
                {
                    Id = 20,
                    Title = "Atomic Heart",
                    Description = "Atomic Heart is a first-person shooter set in an alternate reality Soviet Union.",
                    ImageUrl = "/images/games/atomic-heart.jpg",
                    MetaScore = 75,
                    Genre = "Action FPS",
                    Developer = "Mundfish",
                    Publisher = "Focus Entertainment",
                    Rating = "M",
                    ReleaseDate = new DateTime(2023, 2, 21)
                },
                new Game
                {
                    Id = 21,
                    Title = "Forza Motorsport",
                    Description = "Forza Motorsport is a racing simulation game developed by Turn 10 Studios.",
                    ImageUrl = "/images/games/forza-motorsport.jpg",
                    MetaScore = 86,
                    Genre = "Racing",
                    Developer = "Turn 10 Studios",
                    Publisher = "Xbox Game Studios",
                    Rating = "E",
                    ReleaseDate = new DateTime(2023, 10, 10)
                },
                new Game
                {
                    Id = 22,
                    Title = "Wild Hearts",
                    Description = "Wild Hearts is a hunting action game where you battle giant beasts using ancient technology.",
                    ImageUrl = "/images/games/wild-hearts.jpg",
                    MetaScore = 76,
                    Genre = "Action RPG",
                    Developer = "Omega Force",
                    Publisher = "Electronic Arts",
                    Rating = "T",
                    ReleaseDate = new DateTime(2023, 2, 17)
                },
                new Game
                {
                    Id = 23,
                    Title = "Remnant II",
                    Description = "Remnant II is an action-shooter survival game set in a post-apocalyptic world.",
                    ImageUrl = "/images/games/remnant2.jpg",
                    MetaScore = 85,
                    Genre = "Action",
                    Developer = "Gunfire Games",
                    Publisher = "Gearbox Publishing",
                    Rating = "M",
                    ReleaseDate = new DateTime(2023, 7, 25)
                },
                new Game
                {
                    Id = 24,
                    Title = "Pikmin 4",
                    Description = "Pikmin 4 is a real-time strategy and puzzle game developed by Nintendo.",
                    ImageUrl = "/images/games/pikmin4.jpg",
                    MetaScore = 87,
                    Genre = "Strategy",
                    Developer = "Nintendo",
                    Publisher = "Nintendo",
                    Rating = "E10+",
                    ReleaseDate = new DateTime(2023, 7, 21)
                }
            );
        }
    }
} 