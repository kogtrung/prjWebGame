using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebGame.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebGame.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                if (context.Database.EnsureCreated())
                {
                    // Database was just created, seed initial data
                    SeedInitialData(context);
                }
                
                // Create PublishDate column if it doesn't exist
                try
                {
                    // First check if column exists to avoid error
                    var command = context.Database.GetDbConnection().CreateCommand();
                    command.CommandText = "SELECT COUNT(*) FROM pragma_table_info('NewsPosts') WHERE name = 'PublishDate'";
                    context.Database.OpenConnection();
                    var result = command.ExecuteScalar();
                    
                    if (Convert.ToInt32(result) == 0)
                    {
                        // Column doesn't exist, add it
                        context.Database.ExecuteSqlRaw(
                            "ALTER TABLE NewsPosts ADD COLUMN PublishDate TEXT DEFAULT CURRENT_TIMESTAMP");
                    }
                    context.Database.CloseConnection();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error checking/adding PublishDate column: {ex.Message}");
                    // Column might already exist, ignore the error
                }
                
                // Update any existing records to set PublishDate = CreatedAt
                try 
                {
                    context.Database.ExecuteSqlRaw(
                        "UPDATE NewsPosts SET PublishDate = CreatedAt WHERE PublishDate IS NULL");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating PublishDate values: {ex.Message}");
                    // Error updating, ignore
                }
                
                // Kiểm tra số lượng trò chơi và thêm dữ liệu nếu cần
                var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
                var logger = loggerFactory.CreateLogger("SeedData");
                
                int gameCount = context.Games.Count();
                logger.LogInformation($"Database has {gameCount} games after initialization");
                
                if (gameCount < 30)
                {
                    logger.LogWarning("Game count is low, seeding more games...");
                    
                    try 
                    {
                        SeedMoreClassicGamesAsync(context).GetAwaiter().GetResult();
                        SeedMoreModernGamesAsync(context).GetAwaiter().GetResult();
                        context.SaveChanges();
                        
                        logger.LogInformation($"After additional seeding: {context.Games.Count()} games in database");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error seeding additional games");
                    }
                }
            }
        }

        private static void SeedInitialData(ApplicationDbContext context)
        {
            if (context.Games.Any())
            {
                return;   // DB has been seeded
            }

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger("SeedData");
            
            logger.LogInformation("Adding seed games to database...");
            
            var games = new List<Game>
            {
                // Top Rated Games
                new Game
                {
                    Title = "The Legend of Zelda: Ocarina of Time",
                    Description = "As a young boy, Link is tricked by Ganondorf, the King of the Gerudo Thieves. The evil human uses Link to gain access to the Sacred Realm, where he places his tainted hands on Triforce and transforms the beautiful Hyrulean landscape into a barren wasteland. Link is determined to fix the problems he helped to create, so with the help of Rauru he travels through time gathering the powers of the Seven Sages.",
                    ImageUrl = "https://assets-prd.ignimgs.com/2022/01/19/zelda-ocarina-of-time-1642626761461.jpg",
                    TrailerUrl = "https://www.youtube.com/embed/mF9CxAulk04",
                    VideoUrl = "https://www.youtube.com/embed/mF9CxAulk04",
                    Screenshots = new List<string>
                    {
                        "https://assets.nintendo.com/image/upload/c_fill,w_1200/q_auto:best/f_auto/dpr_2.0/ncom/en_US/games/n64/the-legend-of-zelda-ocarina-of-time-3d/screenshot-gallery/screenshot01",
                        "https://assets.nintendo.com/image/upload/c_fill,w_1200/q_auto:best/f_auto/dpr_2.0/ncom/en_US/games/n64/the-legend-of-zelda-ocarina-of-time-3d/screenshot-gallery/screenshot02",
                        "https://assets.nintendo.com/image/upload/c_fill,w_1200/q_auto:best/f_auto/dpr_2.0/ncom/en_US/games/n64/the-legend-of-zelda-ocarina-of-time-3d/screenshot-gallery/screenshot03"
                    },
                    MetaScore = 99,
                    ReleaseDate = new DateTime(1998, 11, 23),
                    Platform = "Nintendo 64",
                    Genre = "Action, Adventure",
                    Developer = "Nintendo",
                    Publisher = "Nintendo",
                    Rating = "E"
                },
                new Game
                {
                    Title = "Grand Theft Auto IV",
                    Description = "[Metacritic's 2008 Xbox 360 Game of the Year] What does the American dream mean today? For Niko Bellic, fresh off the boat from Europe, it is the hope he can escape his past. For his cousin, Roman, it is the vision that together they can find fortune in Liberty City, gateway to the land of opportunity.",
                    ImageUrl = "https://assets-prd.ignimgs.com/2021/12/17/grand-theft-auto-iv-button-1639777209362.jpg",
                    TrailerUrl = "https://www.youtube.com/embed/eaW0tYpxyp0",
                    VideoUrl = "https://www.youtube.com/embed/eaW0tYpxyp0",
                    Screenshots = new List<string>
                    {
                        "https://media.rockstargames.com/rockstargames/img/global/news/upload/actual_1464012195.jpg",
                        "https://media.rockstargames.com/rockstargames/img/global/news/upload/actual_1464012204.jpg",
                        "https://media.rockstargames.com/rockstargames/img/global/news/upload/actual_1464012213.jpg"
                    },
                    MetaScore = 98,
                    ReleaseDate = new DateTime(2008, 4, 29),
                    Platform = "PlayStation 3, Xbox 360, PC",
                    Genre = "Action, Adventure",
                    Developer = "Rockstar North",
                    Publisher = "Rockstar Games",
                    Rating = "M"
                },
                new Game
                {
                    Title = "Red Dead Redemption 2",
                    Description = "Developed by the creators of Grand Theft Auto V and Red Dead Redemption...",
                    ImageUrl = "https://assets-prd.ignimgs.com/2021/12/14/red-dead-redemption-2-button-fin-1639517031911.jpg",
                    TrailerUrl = "https://www.youtube.com/embed/eaW0tYpxyp0",
                    VideoUrl = "https://www.youtube.com/embed/eaW0tYpxyp0",
                    Screenshots = new List<string>
                    {
                        "https://media.rockstargames.com/rockstargames/img/global/news/upload/actual_1464012195.jpg",
                        "https://media.rockstargames.com/rockstargames/img/global/news/upload/actual_1464012204.jpg",
                        "https://media.rockstargames.com/rockstargames/img/global/news/upload/actual_1464012213.jpg"
                    },
                    MetaScore = 97,
                    ReleaseDate = new DateTime(2018, 10, 26),
                    Platform = "PlayStation 4, Xbox One, PC",
                    Genre = "Action, Adventure",
                    Developer = "Rockstar Games",
                    Publisher = "Rockstar Games",
                    Rating = "M"
                },
                new Game
                {
                    Title = "Super Mario Galaxy",
                    Description = "The ultimate Nintendo hero is taking the ultimate step... out into space.",
                    ImageUrl = "https://assets-prd.ignimgs.com/2021/12/14/super-mario-galaxy-1639520696462.jpg",
                    TrailerUrl = "https://www.youtube.com/embed/mF9CxAulk04",
                    VideoUrl = "https://www.youtube.com/embed/mF9CxAulk04",
                    Screenshots = new List<string>
                    {
                        "https://assets.nintendo.com/image/upload/c_fill,w_1200/q_auto:best/f_auto/dpr_2.0/ncom/en_US/games/n64/the-legend-of-zelda-ocarina-of-time-3d/screenshot-gallery/screenshot01",
                        "https://assets.nintendo.com/image/upload/c_fill,w_1200/q_auto:best/f_auto/dpr_2.0/ncom/en_US/games/n64/the-legend-of-zelda-ocarina-of-time-3d/screenshot-gallery/screenshot02",
                        "https://assets.nintendo.com/image/upload/c_fill,w_1200/q_auto:best/f_auto/dpr_2.0/ncom/en_US/games/n64/the-legend-of-zelda-ocarina-of-time-3d/screenshot-gallery/screenshot03"
                    },
                    MetaScore = 97,
                    ReleaseDate = new DateTime(2007, 11, 12),
                    Platform = "Nintendo Wii",
                    Genre = "Platformer",
                    Developer = "Nintendo",
                    Publisher = "Nintendo",
                    Rating = "E"
                },
                new Game
                {
                    Title = "BioShock",
                    Description = "Welcome to Rapture, an underwater utopia turned dystopian nightmare, in this groundbreaking first-person shooter that blends immersive storytelling with unique plasmid-based powers.",
                    ImageUrl = "https://cdn.2kgames.com/bioshock.com/images/landing/bio1/Bio1_landing_video_thumbnail.jpg",
                    MetaScore = 96,
                    ReleaseDate = new DateTime(2007, 8, 21),
                    Platform = "Xbox 360, PlayStation 3, PC",
                    Genre = "FPS, Action",
                    Developer = "Irrational Games",
                    Publisher = "2K Games",
                    Rating = "M"
                },
                new Game
                {
                    Title = "Mass Effect 2",
                    Description = "Commander Shepard embarks on a suicide mission to save humanity in this epic space RPG featuring deep character development, moral choices, and tactical combat.",
                    ImageUrl = "https://eaassets-a.akamaihd.net/pulse.ea.com/content/legacy/legacy-blog-post-assets/mass-effect/ME_Legacy_11.jpg",
                    MetaScore = 96,
                    ReleaseDate = new DateTime(2010, 1, 26),
                    Platform = "Xbox 360, PlayStation 3, PC",
                    Genre = "RPG, Action",
                    Developer = "BioWare",
                    Publisher = "Electronic Arts",
                    Rating = "M"
                },
                new Game
                {
                    Title = "Disco Elysium",
                    Description = "A groundbreaking RPG with unprecedented freedom of choice, featuring an innovative skill system and a richly detailed world that reacts to your character's thoughts and actions.",
                    ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/632470/ss_39106745c8abf3c057ba1c0a0f86af880e3c19d9.jpg",
                    MetaScore = 97,
                    ReleaseDate = new DateTime(2019, 10, 15),
                    Platform = "PC, PlayStation, Xbox, Switch",
                    Genre = "RPG",
                    Developer = "ZA/UM",
                    Publisher = "ZA/UM",
                    Rating = "M"
                },
                new Game
                {
                    Title = "The Witcher 3: Wild Hunt",
                    Description = "An epic RPG in a dark fantasy world where you play as Geralt of Rivia...",
                    ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/292030/ss_107600c1337accc09104f7a8aa7f275f23cad096.jpg",
                    TrailerUrl = "https://www.youtube.com/embed/c0i88t0Kacs",
                    VideoUrl = "https://www.youtube.com/embed/c0i88t0Kacs",
                    Screenshots = new List<string>
                    {
                        "https://cdn.cloudflare.steamstatic.com/steam/apps/292030/ss_64eb760f9a2b967d000fa577f1ad87f31231578d.jpg",
                        "https://cdn.cloudflare.steamstatic.com/steam/apps/292030/ss_b74d60ee215337d765e4d20c8ca6710c6c0a0fac.jpg",
                        "https://cdn.cloudflare.steamstatic.com/steam/apps/292030/ss_dc55eeb409d6e187456a8e159018e8da098fa468.jpg"
                    },
                    MetaScore = 93,
                    ReleaseDate = new DateTime(2015, 5, 19),
                    Platform = "PC, PlayStation 4, Xbox One, Switch",
                    Genre = "RPG, Action",
                    Developer = "CD Projekt Red",
                    Publisher = "CD Projekt",
                    Rating = "M"
                },
                new Game
                {
                    Title = "Portal 2",
                    Description = "The sequel to the award-winning Portal features new characters, more challenging puzzles, and a deeper story, all wrapped in Valve's signature dark humor.",
                    ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/620/ss_f3f6787d74739d3b2ec8a484b5c994b3d31ef325.jpg",
                    MetaScore = 95,
                    ReleaseDate = new DateTime(2011, 4, 19),
                    Platform = "PC, PlayStation 3, Xbox 360",
                    Genre = "Puzzle, First-Person",
                    Developer = "Valve",
                    Publisher = "Valve",
                    Rating = "E10+"
                },
                new Game
                {
                    Title = "Half-Life 2",
                    Description = "Gordon Freeman returns in Valve's revolutionary FPS sequel featuring the gravity gun, physics-based puzzles, and a dystopian world under alien occupation.",
                    ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/220/ss_7eaa80a35a729e7b917881ba29d9edce1ad17b5b.jpg",
                    MetaScore = 96,
                    ReleaseDate = new DateTime(2004, 11, 16),
                    Platform = "PC, Xbox, PlayStation 3",
                    Genre = "FPS, Action",
                    Developer = "Valve",
                    Publisher = "Valve",
                    Rating = "M"
                },
                new Game
                {
                    Title = "Hades",
                    Description = "A rogue-like dungeon crawler where you defy the god of the dead as you hack and slash your way out of the Underworld of Greek myth.",
                    ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/1145360/ss_c0fed447426b69981cf1721756acf75369801b23.jpg",
                    MetaScore = 93,
                    ReleaseDate = new DateTime(2020, 9, 17),
                    Platform = "PC, Switch, PlayStation, Xbox",
                    Genre = "Action, Roguelike",
                    Developer = "Supergiant Games",
                    Publisher = "Supergiant Games",
                    Rating = "T"
                },
                new Game
                {
                    Title = "Persona 5 Royal",
                    Description = "The enhanced version of Persona 5 adds new characters, story elements, and gameplay features to the acclaimed JRPG about phantom thieves who steal corrupted desires.",
                    ImageUrl = "https://assets.atlus.com/images/p5r/screens/p5r_gameplay_3.jpg",
                    MetaScore = 95,
                    ReleaseDate = new DateTime(2020, 3, 31),
                    Platform = "PlayStation 4, PC, Switch, Xbox",
                    Genre = "JRPG",
                    Developer = "Atlus",
                    Publisher = "Atlus",
                    Rating = "M"
                },
                // Upcoming games for variety
                new Game
                {
                    Title = "Monster Hunter Wilds",
                    Description = "The latest entry in Capcom's monster hunting franchise, featuring a vast new open world, dynamic weather systems, and more fearsome beasts to hunt.",
                    ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/2246340/ss_bf0c3e4af0c3a9fd7233dd469a5f53e9a2d9d10f.jpg",
                    MetaScore = 0, // No score yet
                    ReleaseDate = new DateTime(2025, 2, 15),
                    Platform = "PC, PlayStation 5, Xbox Series X/S",
                    Genre = "Action, RPG",
                    Developer = "Capcom",
                    Publisher = "Capcom",
                    Rating = "T"
                },
                new Game
                {
                    Title = "Assassin's Creed Shadows",
                    Description = "Set in feudal Japan, this new Assassin's Creed title lets players experience the conflict between samurai and ninja traditions through dual protagonists.",
                    ImageUrl = "https://staticctf.ubisoft.com/J3yJr34U2pZ2Ieem48Dwy9uqj5PNUQTn/3l4DsrZUd6dByq0mVyELnH/93bfd9bad4fe3b39573e31bad7e6fd5d/acs-keyart-thumbnail.jpg",
                    MetaScore = 0, // No score yet
                    ReleaseDate = new DateTime(2024, 11, 15),
                    Platform = "PC, PlayStation 5, Xbox Series X/S",
                    Genre = "Action, Adventure, Stealth",
                    Developer = "Ubisoft Quebec",
                    Publisher = "Ubisoft",
                    Rating = "M"
                },
                new Game
                {
                    Title = "Final Fantasy XVI",
                    Description = "The latest mainline entry in the iconic Final Fantasy series, featuring a dark medieval fantasy setting, visceral real-time combat, and the return of summons as Eikons.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202211/0204/OvPnVnFj7fXz9LzUGREWdSO6.png",
                    MetaScore = 87,
                    ReleaseDate = new DateTime(2023, 6, 22),
                    Platform = "PlayStation 5, PC",
                    Genre = "Action RPG, Fantasy",
                    Developer = "Square Enix",
                    Publisher = "Square Enix",
                    Rating = "M"
                },
                new Game
                {
                    Title = "Dragon Age: The Veilguard",
                    Description = "The latest installment in BioWare's epic Dragon Age series, featuring a new protagonist facing the consequences of torn Veil between the world of mortals and spirits.",
                    ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/2399830/ss_ceab9984087b61aca35b6d9a87c8da6805ee14b9.jpg",
                    MetaScore = 0, // No score yet
                    ReleaseDate = new DateTime(2024, 10, 31),
                    Platform = "PC, PlayStation 5, Xbox Series X/S",
                    Genre = "RPG, Fantasy",
                    Developer = "BioWare",
                    Publisher = "Electronic Arts",
                    Rating = "M"
                },
                new Game
                {
                    Title = "Starfield",
                    Description = "Bethesda Game Studios' first new universe in 25 years, an epic space RPG set amongst the stars where you can create any character and explore with unparalleled freedom.",
                    ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/1716740/ss_c4b2c9aad583034d0d9d0e525f36cc5185960597.jpg",
                    MetaScore = 83,
                    ReleaseDate = new DateTime(2023, 9, 6),
                    Platform = "PC, Xbox Series X/S",
                    Genre = "RPG, Open World, Space",
                    Developer = "Bethesda Game Studios",
                    Publisher = "Bethesda Softworks",
                    Rating = "M"
                },
                new Game
                {
                    Title = "Metroid Prime 4",
                    Description = "The long-awaited fourth installment in the beloved Metroid Prime series, continuing the first-person adventure of bounty hunter Samus Aran.",
                    ImageUrl = "https://assets.nintendo.com/image/upload/c_fill,f_auto,q_auto,w_1200/v1/ncom/en_US/games/switch/m/metroid-prime-4-switch/hero",
                    MetaScore = 0, // No score yet
                    ReleaseDate = new DateTime(2025, 6, 15),
                    Platform = "Nintendo Switch",
                    Genre = "Action, Adventure, First-Person",
                    Developer = "Retro Studios",
                    Publisher = "Nintendo",
                    Rating = "T"
                },
                new Game
                {
                    Title = "Alan Wake 2",
                    Description = "The long-awaited sequel to Remedy's psychological thriller, following writer Alan Wake as he attempts to escape the nightmarish Dark Place and face his darkness.",
                    ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/1850570/ss_8f3f4af04b018711e3f92a4e1fa187aebeaeba6d.jpg",
                    MetaScore = 89,
                    ReleaseDate = new DateTime(2023, 10, 27),
                    Platform = "PC, PlayStation 5, Xbox Series X/S",
                    Genre = "Survival Horror, Third-Person",
                    Developer = "Remedy Entertainment",
                    Publisher = "Epic Games",
                    Rating = "M"
                },
                new Game
                {
                    Title = "Hollow Knight: Silksong",
                    Description = "The sequel to the critically acclaimed Hollow Knight, featuring Hornet, the princess-protector of Hallownest, as she adventures through a new kingdom haunted by silk and song.",
                    ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/1030300/ss_c5749528ac10a58c8309dff33e8c6ca65ca4e8de.jpg",
                    MetaScore = 0, // No score yet
                    ReleaseDate = new DateTime(2024, 12, 31),
                    Platform = "PC, Nintendo Switch, PlayStation, Xbox",
                    Genre = "Metroidvania, Action, Adventure",
                    Developer = "Team Cherry",
                    Publisher = "Team Cherry",
                    Rating = "E10+"
                },
                new Game
                {
                    Title = "Marvel's Spider-Man 2",
                    Description = "Peter Parker and Miles Morales return in a new adventure featuring Venom as they face new threats to New York City while balancing their dual lives.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202306/1219/1c7afc327006972ce1eda375f31f58db5525a235acae7d8ffa1fe7f8e5de8332.jpg",
                    MetaScore = 90,
                    ReleaseDate = new DateTime(2023, 10, 20),
                    Platform = "PlayStation 5",
                    Genre = "Action, Adventure",
                    Developer = "Insomniac Games",
                    Publisher = "Sony Interactive Entertainment",
                    Rating = "T"
                },
                new Game
                {
                    Title = "Baldur's Gate 3",
                    Description = "A mind-flaying adventure based on the Dungeons & Dragons tabletop RPG, where players can create their own heroes and journey through the Forgotten Realms.",
                    ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/1086940/header.jpg",
                    TrailerUrl = "https://www.youtube.com/embed/1T22wNvoNiU",
                    Screenshots = new List<string>
                    {
                        "https://cdn.cloudflare.steamstatic.com/steam/apps/1086940/ss_c30d0541d7947f89e095ce89f87e46d0f4b6f086.jpg",
                        "https://cdn.cloudflare.steamstatic.com/steam/apps/1086940/ss_c30d0541d7947f89e095ce89f87e46d0f4b6f086.jpg",
                        "https://cdn.cloudflare.steamstatic.com/steam/apps/1086940/ss_c30d0541d7947f89e095ce89f87e46d0f4b6f086.jpg"
                    },
                    MetaScore = 96,
                    ReleaseDate = new DateTime(2023, 8, 3),
                    Platform = "PC, PlayStation 5, Xbox Series X/S",
                    Genre = "RPG, Turn-Based",
                    Developer = "Larian Studios",
                    Publisher = "Larian Studios",
                    Rating = "M"
                },
                new Game
                {
                    Title = "Elden Ring: Shadow of the Erdtree",
                    Description = "The major expansion to the award-winning Elden Ring, offering a new shadowy realm to explore and conquer with new challenges, bosses, and rewards.",
                    ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/1956680/ss_3c3299b875e758ca49d8f415f06d60dec7a57bef.jpg",
                    MetaScore = 94,
                    ReleaseDate = new DateTime(2024, 6, 21),
                    Platform = "PC, PlayStation 4, PlayStation 5, Xbox One, Xbox Series X/S",
                    Genre = "Action RPG, Open World",
                    Developer = "FromSoftware",
                    Publisher = "Bandai Namco",
                    Rating = "M"
                },
                new Game
                {
                    Title = "Resident Evil 9",
                    Description = "The next mainline entry in Capcom's iconic survival horror series, with new characters, locations, and terrifying biological horrors to survive.",
                    ImageUrl = "https://cdn2.unrealengine.com/re4-keyart-1920x1080-1920x1080-33623c2a928d.jpg",
                    MetaScore = 0, // No score yet
                    ReleaseDate = new DateTime(2025, 10, 31),
                    Platform = "PC, PlayStation 5, Xbox Series X/S",
                    Genre = "Survival Horror",
                    Developer = "Capcom",
                    Publisher = "Capcom",
                    Rating = "M"
                },
                new Game
                {
                    Title = "Final Fantasy VII Rebirth",
                    Description = "The second part of the Final Fantasy VII Remake project, continuing Cloud and his allies' journey beyond Midgar to pursue Sephiroth.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202306/0215/dd1b371a09fdf99ea3df6348fa02c05eeb72d6ffee2458bf.jpg",
                    MetaScore = 92,
                    ReleaseDate = new DateTime(2024, 2, 29),
                    Platform = "PlayStation 5",
                    Genre = "RPG, Action",
                    Developer = "Square Enix",
                    Publisher = "Square Enix",
                    Rating = "T"
                },
                new Game
                {
                    Title = "The Last of Us Part 3",
                    Description = "The next chapter in Naughty Dog's acclaimed post-apocalyptic series, continuing the emotional journey of surviving in a world ravaged by a fungal pandemic.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202206/0720/eEczyEMDd2BLa3dtkGJVE9Id.png",
                    MetaScore = 0, // No score yet
                    ReleaseDate = new DateTime(2026, 5, 29),
                    Platform = "PlayStation 5",
                    Genre = "Action, Adventure, Survival",
                    Developer = "Naughty Dog",
                    Publisher = "Sony Interactive Entertainment",
                    Rating = "M"
                },
                new Game
                {
                    Title = "Forza Motorsport",
                    Description = "The reboot of Microsoft's premier racing simulation, featuring stunning visual fidelity, dynamic weather, and a comprehensive roster of vehicles and tracks.",
                    ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/2440510/ss_de9e8d5e5c3aec9d6c11841cf775f2ef3a156b88.jpg",
                    MetaScore = 86,
                    ReleaseDate = new DateTime(2023, 10, 10),
                    Platform = "PC, Xbox Series X/S",
                    Genre = "Racing, Simulation",
                    Developer = "Turn 10 Studios",
                    Publisher = "Xbox Game Studios",
                    Rating = "E"
                },
                new Game
                {
                    Title = "Silent Hill 2 Remake",
                    Description = "A remake of the psychological horror masterpiece, following James Sunderland as he returns to Silent Hill after receiving a letter from his deceased wife.",
                    ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/2124490/ss_be916a551d160b33dfbd8bae33ff80f9fae01183.jpg",
                    MetaScore = 0, // No score yet
                    ReleaseDate = new DateTime(2024, 10, 8),
                    Platform = "PC, PlayStation 5",
                    Genre = "Survival Horror, Psychological Horror",
                    Developer = "Bloober Team",
                    Publisher = "Konami",
                    Rating = "M"
                },
                new Game
                {
                    Title = "Black Myth: Wukong",
                    Description = "An action RPG based on Journey to the West, following the Destined One, Sun Wukong, as he faces gods and monsters in a fantastical version of ancient China.",
                    ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/2358720/ss_7a3883a05e474f661e18243977a1b9d211a92ba9.jpg",
                    MetaScore = 83,
                    ReleaseDate = new DateTime(2024, 8, 20),
                    Platform = "PC, PlayStation 5",
                    Genre = "Action RPG",
                    Developer = "Game Science",
                    Publisher = "Game Science",
                    Rating = "M"
                },
                new Game
                {
                    Title = "Diablo IV",
                    Description = "The fourth entry in Blizzard's dark fantasy action RPG series, featuring a grim open world, five classes, and the eternal struggle between angels and demons.",
                    ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/2344520/ss_c84e3eda67fc98a80bf4bd7979cfe2e7ec7da0d1.jpg",
                    MetaScore = 88,
                    ReleaseDate = new DateTime(2023, 6, 6),
                    Platform = "PC, PlayStation, Xbox",
                    Genre = "Action RPG, Hack and Slash",
                    Developer = "Blizzard Entertainment",
                    Publisher = "Blizzard Entertainment",
                    Rating = "M"
                },
                new Game
                {
                    Title = "Ghost of Tsushima",
                    Description = "An open-world action-adventure that follows a samurai warrior fighting to protect his home during the first Mongol invasion of Japan.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202010/0222/niMUubpU9y1PxNvYmDfb8QFD.png",
                    MetaScore = 83,
                    ReleaseDate = new DateTime(2020, 7, 17),
                    Platform = "PlayStation 4, PlayStation 5, PC",
                    Genre = "Action, Adventure, Open World",
                    Developer = "Sucker Punch Productions",
                    Publisher = "Sony Interactive Entertainment",
                    Rating = "M"
                },
                new Game
                {
                    Title = "Death Stranding",
                    Description = "Hideo Kojima's post-apocalyptic delivery adventure where players reconnect isolated cities and prevent mankind's extinction.",
                    ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/1190460/ss_64eb760f9a2b967d000fa577f1ad87f31231578d.jpg",
                    MetaScore = 86,
                    ReleaseDate = new DateTime(2019, 11, 8),
                    Platform = "PlayStation 4, PC",
                    Genre = "Action, Adventure",
                    Developer = "Kojima Productions",
                    Publisher = "505 Games",
                    Rating = "M"
                },
                new Game
                {
                    Title = "Among Us",
                    Description = "A party game of teamwork and betrayal where players work to prepare their spaceship for departure, but beware of impostors bent on killing everyone.",
                    ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/945360/ss_29b880a611a84395a815da2f7c94e3f11bbd598e.jpg",
                    MetaScore = 85,
                    ReleaseDate = new DateTime(2018, 11, 16),
                    Platform = "PC, Mobile, Switch, PlayStation, Xbox",
                    Genre = "Social Deduction, Party",
                    Developer = "Innersloth",
                    Publisher = "Innersloth",
                    Rating = "E10+"
                },
                new Game
                {
                    Title = "Kingdom Hearts III",
                    Description = "The conclusion to Sora's long journey, featuring Disney and Pixar worlds as he searches for the power of waking to save his lost friends.",
                    ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/1446780/ss_3b98d66f1739be4f30e5b91e5b79f1346d0689f1.jpg",
                    MetaScore = 83,
                    ReleaseDate = new DateTime(2019, 1, 25),
                    Platform = "PlayStation 4, Xbox One, PC",
                    Genre = "Action RPG",
                    Developer = "Square Enix",
                    Publisher = "Square Enix",
                    Rating = "E10+"
                },
                new Game
                {
                    Title = "Cyberpunk 2077",
                    Description = "An open-world action-adventure set in Night City, a megalopolis obsessed with power, glamour, and body modification.",
                    ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/1091500/ss_b529b0abc43f55fc23fe8058eddb6e37c9229fca.jpg",
                    MetaScore = 86,
                    ReleaseDate = new DateTime(2020, 12, 10),
                    Platform = "PC, PlayStation, Xbox",
                    Genre = "RPG, Open World",
                    Developer = "CD Projekt Red",
                    Publisher = "CD Projekt",
                    Rating = "M"
                },
                new Game
                {
                    Title = "Assassin's Creed Valhalla",
                    Description = "Embark on a Viking saga as Eivor, a fierce raider who must secure a future for their clan in the rich lands of England.",
                    ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/2208920/ss_e5ca539eec4a9dbdc6be6b1f295400275c1b75fb.jpg",
                    MetaScore = 84,
                    ReleaseDate = new DateTime(2020, 11, 10),
                    Platform = "PC, PlayStation, Xbox",
                    Genre = "Action RPG, Open World",
                    Developer = "Ubisoft Montreal",
                    Publisher = "Ubisoft",
                    Rating = "M"
                },
                new Game
                {
                    Title = "Sekiro: Shadows Die Twice",
                    Description = "A third-person action-adventure where you play as a shinobi seeking revenge against the samurai clan who attacked him and kidnapped his lord.",
                    ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/814380/ss_271f950a9a48f9f066fab5b2d77de5f9998d1dee.jpg",
                    MetaScore = 90,
                    ReleaseDate = new DateTime(2019, 3, 22),
                    Platform = "PC, PlayStation 4, Xbox One",
                    Genre = "Action, Adventure",
                    Developer = "FromSoftware",
                    Publisher = "Activision",
                    Rating = "M"
                },
                new Game
                {
                    Title = "Control",
                    Description = "A supernatural third-person action-adventure that takes place in a secretive agency in New York that's been invaded by an otherworldly threat.",
                    ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/870780/ss_6eccc970b5d2d4c58968a552a3679d2e442ed8a2.jpg",
                    MetaScore = 85,
                    ReleaseDate = new DateTime(2019, 8, 27),
                    Platform = "PC, PlayStation, Xbox, Switch",
                    Genre = "Action, Adventure",
                    Developer = "Remedy Entertainment",
                    Publisher = "505 Games",
                    Rating = "M"
                },
                new Game
                {
                    Title = "Monster Hunter World",
                    Description = "Hunt down enormous monsters in diverse environments, either alone or in cooperative gameplay, then use their materials to create stronger weapons and armor.",
                    ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/582010/ss_c882b95bf98c5179c4c245b53f10f300a1134243.jpg",
                    MetaScore = 90,
                    ReleaseDate = new DateTime(2018, 1, 26),
                    Platform = "PC, PlayStation 4, Xbox One",
                    Genre = "Action RPG",
                    Developer = "Capcom",
                    Publisher = "Capcom",
                    Rating = "T"
                },
                new Game
                {
                    Title = "God of War",
                    Description = "His vengeance against the Gods of Olympus years behind him...",
                    ImageUrl = "https://image.api.playstation.com/vulcan/img/rnd/202010/2217/p3pYq0QxntZQREXRVzwB2AJZ.png",
                    TrailerUrl = "https://www.youtube.com/embed/K0u_kAWLJOA",
                    VideoUrl = "https://www.youtube.com/embed/K0u_kAWLJOA",
                    Screenshots = new List<string>
                    {
                        "https://image.api.playstation.com/vulcan/ap/rnd/202207/1210/4xJ8XB3bi888QTLZYdl7Oi0s.png",
                        "https://image.api.playstation.com/vulcan/ap/rnd/202207/1210/8jW4GqpGq9UJXxRr0PqbGXss.png",
                        "https://image.api.playstation.com/vulcan/ap/rnd/202207/1210/3Wxh1n0FJBdmkY1G5H3fAApu.png"
                    },
                    MetaScore = 94,
                    ReleaseDate = new DateTime(2018, 4, 20),
                    Platform = "PlayStation 4, PC",
                    Genre = "Action, Adventure",
                    Developer = "Santa Monica Studio",
                    Publisher = "Sony Interactive Entertainment",
                    Rating = "M"
                },
                new Game
                {
                    Title = "Elden Ring",
                    Description = "THE NEW FANTASY ACTION RPG. Rise, Tarnished...",
                    ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/1245620/header.jpg",
                    TrailerUrl = "https://www.youtube.com/embed/E3Huy2cdih0",
                    VideoUrl = "https://www.youtube.com/embed/E3Huy2cdih0",
                    Screenshots = new List<string>
                    {
                        "https://cdn.cloudflare.steamstatic.com/steam/apps/1245620/ss_ae44317e3bd07b7690b4d62cc5d0d1df30367a91.jpg",
                        "https://cdn.cloudflare.steamstatic.com/steam/apps/1245620/ss_c372274833ae6e5437b914fa7a6c88a8b284f2b4.jpg",
                        "https://cdn.cloudflare.steamstatic.com/steam/apps/1245620/ss_f7c7d7a9bfc24b6a0e6493d5deadf5cc78c69c5a.jpg"
                    },
                    MetaScore = 96,
                    ReleaseDate = new DateTime(2022, 2, 25),
                    Platform = "PC, PlayStation 5, Xbox Series X/S",
                    Genre = "Action RPG",
                    Developer = "FromSoftware",
                    Publisher = "Bandai Namco",
                    Rating = "M"
                },
                new Game
                {
                    Title = "Baldur's Gate 3",
                    Description = "An ancient evil has returned to Baldur's Gate, intent on devouring it from the inside out. The fate of Faerun lies in your hands.",
                    ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/1086940/header.jpg",
                    TrailerUrl = "https://www.youtube.com/embed/1T22wNvoNiU",
                    Screenshots = new List<string>
                    {
                        "https://cdn.cloudflare.steamstatic.com/steam/apps/1086940/ss_c30d0541d7947f89e095ce89f87e46d0f4b6f086.jpg",
                        "https://cdn.cloudflare.steamstatic.com/steam/apps/1086940/ss_c30d0541d7947f89e095ce89f87e46d0f4b6f086.jpg",
                        "https://cdn.cloudflare.steamstatic.com/steam/apps/1086940/ss_c30d0541d7947f89e095ce89f87e46d0f4b6f086.jpg"
                    },
                    MetaScore = 96,
                    ReleaseDate = new DateTime(2023, 8, 3),
                    Platform = "PC, PlayStation 5, Xbox Series X/S",
                    Genre = "RPG, Turn-Based Strategy",
                    Developer = "Larian Studios",
                    Publisher = "Larian Studios",
                    Rating = "M"
                }
            };
            
            context.Games.AddRange(games);
            context.SaveChanges();
            logger.LogInformation($"Added {games.Count} games to database");
        }

        public static void SeedGames(ApplicationDbContext context, ILogger logger)
        {
            try
            {
                if (!context.Games.Any())
                {
                    var games = new List<Game>
                    {
                        // Các game hiện có
                        new Game
                        {
                            Title = "The Legend of Zelda: Ocarina of Time",
                            Description = "The Legend of Zelda: Ocarina of Time is an action-adventure game that revolutionized the genre with its innovative 3D gameplay and epic storyline.",
                            ImageUrl = "https://upload.wikimedia.org/wikipedia/en/5/57/The_Legend_of_Zelda_Ocarina_of_Time.jpg",
                            TrailerUrl = "https://www.youtube.com/embed/mF9CxAulNPY",
                            VideoUrl = "https://www.youtube.com/embed/mF9CxAulNPY",
                            MetaScore = 99,
                            ReleaseDate = new DateTime(1998, 11, 23),
                            Platform = "Nintendo 64",
                            Genre = "Action-Adventure",
                            Developer = "Nintendo",
                            Publisher = "Nintendo",
                            Rating = "E"
                        },
                        new Game
                        {
                            Title = "SoulCalibur",
                            Description = "SoulCalibur is a weapons-based fighting game that set new standards for the genre with its revolutionary 8-way movement system.",
                            ImageUrl = "https://upload.wikimedia.org/wikipedia/en/4/44/Soul_Calibur_Dreamcast_Game_Cover.jpg",
                            TrailerUrl = "https://www.youtube.com/embed/HxQaPhXEZG0",
                            VideoUrl = "https://www.youtube.com/embed/HxQaPhXEZG0",
                            MetaScore = 98,
                            ReleaseDate = new DateTime(1999, 9, 8),
                            Platform = "Dreamcast",
                            Genre = "Fighting",
                            Developer = "Project Soul",
                            Publisher = "Namco",
                            Rating = "T"
                        },
                        new Game
                        {
                            Title = "Grand Theft Auto IV",
                            Description = "Grand Theft Auto IV follows Niko Bellic's story in Liberty City, a dark and gritty take on modern America.",
                            ImageUrl = "https://upload.wikimedia.org/wikipedia/en/b/b7/Grand_Theft_Auto_IV_cover.jpg",
                            TrailerUrl = "https://www.youtube.com/embed/M80K51DosFo",
                            VideoUrl = "https://www.youtube.com/embed/M80K51DosFo",
                            MetaScore = 98,
                            ReleaseDate = new DateTime(2008, 4, 29),
                            Platform = "PlayStation 3, Xbox 360, PC",
                            Genre = "Action-Adventure",
                            Developer = "Rockstar North",
                            Publisher = "Rockstar Games",
                            Rating = "M"
                        },
                        new Game
                        {
                            Title = "Tony Hawk's Pro Skater 2",
                            Description = "Tony Hawk's Pro Skater 2 is the highly acclaimed sequel that defined skateboarding games with its perfect blend of arcade action and technical skating.",
                            ImageUrl = "https://upload.wikimedia.org/wikipedia/en/9/9c/Tony_Hawk%27s_Pro_Skater_2_cover.png",
                            TrailerUrl = "https://www.youtube.com/embed/IrZ1b7TtF_8",
                            VideoUrl = "https://www.youtube.com/embed/IrZ1b7TtF_8",
                            MetaScore = 98,
                            ReleaseDate = new DateTime(2000, 9, 20),
                            Platform = "PlayStation",
                            Genre = "Sports",
                            Developer = "Neversoft",
                            Publisher = "Activision",
                            Rating = "T"
                        },
                        // Thêm 10 game mới
                        new Game
                        {
                            Title = "Metal Gear Solid 3: Snake Eater",
                            Description = "A Cold War-era tactical espionage game that follows Naked Snake's mission in the Soviet jungle.",
                            ImageUrl = "https://upload.wikimedia.org/wikipedia/en/b/b3/Mgs3box.jpg",
                            TrailerUrl = "https://www.youtube.com/embed/0L5YfCQUGlc",
                            VideoUrl = "https://www.youtube.com/embed/0L5YfCQUGlc",
                            MetaScore = 95,
                            ReleaseDate = new DateTime(2004, 11, 17),
                            Platform = "PlayStation 2",
                            Genre = "Stealth Action",
                            Developer = "Konami",
                            Publisher = "Konami",
                            Rating = "M"
                        },
                        new Game
                        {
                            Title = "Chrono Trigger",
                            Description = "A groundbreaking RPG about time travel and saving the world, featuring multiple endings and innovative battle system.",
                            ImageUrl = "https://upload.wikimedia.org/wikipedia/en/a/a7/Chrono_Trigger_Box.jpg",
                            TrailerUrl = "https://www.youtube.com/embed/w06uuGJflHs",
                            VideoUrl = "https://www.youtube.com/embed/w06uuGJflHs",
                            MetaScore = 92,
                            ReleaseDate = new DateTime(1995, 3, 11),
                            Platform = "Super Nintendo",
                            Genre = "RPG",
                            Developer = "Square",
                            Publisher = "Square",
                            Rating = "E"
                        },
                        new Game
                        {
                            Title = "Half-Life 2",
                            Description = "A revolutionary first-person shooter that set new standards for physics-based gameplay and storytelling.",
                            ImageUrl = "https://upload.wikimedia.org/wikipedia/en/2/25/Half-Life_2_cover.jpg",
                            TrailerUrl = "https://www.youtube.com/embed/ID1dWN3n7q4",
                            VideoUrl = "https://www.youtube.com/embed/ID1dWN3n7q4",
                            MetaScore = 96,
                            ReleaseDate = new DateTime(2004, 11, 16),
                            Platform = "PC",
                            Genre = "First-Person Shooter",
                            Developer = "Valve",
                            Publisher = "Valve",
                            Rating = "M"
                        },
                        new Game
                        {
                            Title = "Shadow of the Colossus",
                            Description = "An artistic masterpiece where players hunt down massive colossi in a forbidden land to save a loved one.",
                            ImageUrl = "https://upload.wikimedia.org/wikipedia/en/b/b3/Shadow_of_the_Colossus_cover_art.jpg",
                            TrailerUrl = "https://www.youtube.com/embed/pdZQ98mWeto",
                            VideoUrl = "https://www.youtube.com/embed/pdZQ98mWeto",
                            MetaScore = 91,
                            ReleaseDate = new DateTime(2005, 10, 18),
                            Platform = "PlayStation 2",
                            Genre = "Action-Adventure",
                            Developer = "Team Ico",
                            Publisher = "Sony Computer Entertainment",
                            Rating = "T"
                        },
                        new Game
                        {
                            Title = "BioShock",
                            Description = "A philosophical first-person shooter set in the underwater city of Rapture, exploring themes of free will and objectivism.",
                            ImageUrl = "https://upload.wikimedia.org/wikipedia/en/6/6d/BioShock_cover.jpg",
                            TrailerUrl = "https://www.youtube.com/embed/Lmw78t8NgIE",
                            VideoUrl = "https://www.youtube.com/embed/Lmw78t8NgIE",
                            MetaScore = 96,
                            ReleaseDate = new DateTime(2007, 8, 21),
                            Platform = "PC, Xbox 360, PlayStation 3",
                            Genre = "First-Person Shooter",
                            Developer = "2K Boston",
                            Publisher = "2K Games",
                            Rating = "M"
                        },
                        new Game
                        {
                            Title = "Portal",
                            Description = "A unique puzzle game where players use a portal gun to solve increasingly complex spatial puzzles.",
                            ImageUrl = "https://upload.wikimedia.org/wikipedia/en/9/9f/Portal_standalonebox.jpg",
                            TrailerUrl = "https://www.youtube.com/embed/TluRVBhmf8w",
                            VideoUrl = "https://www.youtube.com/embed/TluRVBhmf8w",
                            MetaScore = 90,
                            ReleaseDate = new DateTime(2007, 10, 10),
                            Platform = "PC, Xbox 360, PlayStation 3",
                            Genre = "Puzzle",
                            Developer = "Valve",
                            Publisher = "Valve",
                            Rating = "T"
                        },
                        new Game
                        {
                            Title = "Mass Effect 2",
                            Description = "An epic sci-fi RPG where Commander Shepard must assemble a team for a suicide mission to save humanity.",
                            ImageUrl = "https://upload.wikimedia.org/wikipedia/en/0/05/MassEffect2_cover.PNG",
                            TrailerUrl = "https://www.youtube.com/embed/lx9sPQpjgjU",
                            VideoUrl = "https://www.youtube.com/embed/lx9sPQpjgjU",
                            MetaScore = 94,
                            ReleaseDate = new DateTime(2010, 1, 26),
                            Platform = "PC, Xbox 360, PlayStation 3",
                            Genre = "Action RPG",
                            Developer = "BioWare",
                            Publisher = "Electronic Arts",
                            Rating = "M"
                        },
                        new Game
                        {
                            Title = "Super Mario Galaxy",
                            Description = "A revolutionary 3D platformer that takes Mario's adventures into space with unique gravity-based gameplay.",
                            ImageUrl = "https://upload.wikimedia.org/wikipedia/en/7/76/SuperMarioGalaxy.jpg",
                            TrailerUrl = "https://www.youtube.com/embed/rmN8DHZHo7E",
                            VideoUrl = "https://www.youtube.com/embed/rmN8DHZHo7E",
                            MetaScore = 97,
                            ReleaseDate = new DateTime(2007, 11, 1),
                            Platform = "Nintendo Wii",
                            Genre = "Platformer",
                            Developer = "Nintendo",
                            Publisher = "Nintendo",
                            Rating = "E"
                        },
                        new Game
                        {
                            Title = "Final Fantasy VII",
                            Description = "A groundbreaking JRPG that follows Cloud Strife and his allies in their fight against the Shinra Corporation.",
                            ImageUrl = "https://upload.wikimedia.org/wikipedia/en/c/c2/Final_Fantasy_VII_Box_Art.jpg",
                            TrailerUrl = "https://www.youtube.com/embed/Ru9zzFEdGWk",
                            VideoUrl = "https://www.youtube.com/embed/Ru9zzFEdGWk",
                            MetaScore = 92,
                            ReleaseDate = new DateTime(1997, 1, 31),
                            Platform = "PlayStation",
                            Genre = "RPG",
                            Developer = "Square",
                            Publisher = "Square",
                            Rating = "T"
                        },
                        new Game
                        {
                            Title = "Red Dead Redemption",
                            Description = "An open-world western action game following John Marston's quest for redemption in the dying American frontier.",
                            ImageUrl = "https://upload.wikimedia.org/wikipedia/en/a/a7/Red_Dead_Redemption.jpg",
                            TrailerUrl = "https://www.youtube.com/embed/PD24MkbHQrc",
                            VideoUrl = "https://www.youtube.com/embed/PD24MkbHQrc",
                            MetaScore = 95,
                            ReleaseDate = new DateTime(2010, 5, 18),
                            Platform = "PlayStation 3, Xbox 360",
                            Genre = "Action-Adventure",
                            Developer = "Rockstar San Diego",
                            Publisher = "Rockstar Games",
                            Rating = "M"
                        }
                    };

                    context.Games.AddRange(games);
                    context.SaveChanges();
                    logger.LogInformation("Seeded games successfully");
                }
                else
                {
                    logger.LogInformation("Games already exist in database");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error seeding games");
                throw;
            }
        }

        public static async Task SeedMoreClassicGamesAsync(ApplicationDbContext context)
        {
            if (!context.Games.Any(g => g.Title == "Chrono Trigger"))
            {
                var classicGames = new List<Game>
                {
                    new Game
                    {
                        Title = "Chrono Trigger",
                        Description = "A classic Japanese RPG about time travel, featuring a memorable cast of characters and multiple endings. Often cited as one of the greatest RPGs ever made.",
                        ImageUrl = "https://upload.wikimedia.org/wikipedia/en/a/a7/Chrono_Trigger.jpg",
                        MetaScore = 92,
                        ReleaseDate = new DateTime(1995, 3, 11),
                        Platform = "SNES, PlayStation, Nintendo DS, PC, Mobile",
                        Genre = "JRPG",
                        Developer = "Square",
                        Publisher = "Square",
                        Rating = "E10+"
                    },
                    new Game
                    {
                        Title = "Metal Gear Solid",
                        Description = "A stealth action game that revolutionized the genre, with complex storytelling and cinematic presentation.",
                        ImageUrl = "https://upload.wikimedia.org/wikipedia/en/3/33/Metal_Gear_Solid_cover_art.png",
                        MetaScore = 94,
                        ReleaseDate = new DateTime(1998, 9, 3),
                        Platform = "PlayStation",
                        Genre = "Stealth, Action",
                        Developer = "Konami",
                        Publisher = "Konami",
                        Rating = "M"
                    },
                    new Game
                    {
                        Title = "Castlevania: Symphony of the Night",
                        Description = "A groundbreaking metroidvania game featuring RPG elements, gorgeous 2D graphics, and an unforgettable soundtrack.",
                        ImageUrl = "https://upload.wikimedia.org/wikipedia/en/5/51/Castlevania_SOTN_PAL.jpg",
                        MetaScore = 93,
                        ReleaseDate = new DateTime(1997, 3, 20),
                        Platform = "PlayStation",
                        Genre = "Action, Adventure, RPG",
                        Developer = "Konami",
                        Publisher = "Konami",
                        Rating = "T"
                    },
                    new Game
                    {
                        Title = "Final Fantasy VI",
                        Description = "A beloved JRPG with a large cast of characters, steampunk setting, and one of gaming's most iconic villains.",
                        ImageUrl = "https://upload.wikimedia.org/wikipedia/en/0/05/Final_Fantasy_VI.jpg",
                        MetaScore = 92,
                        ReleaseDate = new DateTime(1994, 4, 2),
                        Platform = "SNES, PlayStation, GBA, PC, Mobile",
                        Genre = "JRPG",
                        Developer = "Square",
                        Publisher = "Square",
                        Rating = "T"
                    },
                    new Game
                    {
                        Title = "Super Metroid",
                        Description = "The definitive metroidvania game, featuring atmospheric world design and precise platforming gameplay.",
                        ImageUrl = "https://upload.wikimedia.org/wikipedia/en/e/e4/Smetroidbox.jpg",
                        MetaScore = 96,
                        ReleaseDate = new DateTime(1994, 3, 19),
                        Platform = "SNES",
                        Genre = "Action, Adventure",
                        Developer = "Nintendo",
                        Publisher = "Nintendo",
                        Rating = "E"
                    },
                    new Game
                    {
                        Title = "Deus Ex",
                        Description = "A groundbreaking immersive sim that combines FPS, RPG, and stealth gameplay with player choice and multiple solutions.",
                        ImageUrl = "https://upload.wikimedia.org/wikipedia/en/4/42/Dxcover.jpg",
                        MetaScore = 90,
                        ReleaseDate = new DateTime(2000, 6, 23),
                        Platform = "PC, PlayStation 2, Mac",
                        Genre = "Action, RPG",
                        Developer = "Ion Storm",
                        Publisher = "Eidos Interactive",
                        Rating = "M"
                    },
                    new Game
                    {
                        Title = "Resident Evil 2",
                        Description = "A survival horror classic featuring dual protagonists and a terrifying zombie outbreak in Raccoon City.",
                        ImageUrl = "https://upload.wikimedia.org/wikipedia/en/2/29/Resident_Evil_2_Cover.png",
                        MetaScore = 89,
                        ReleaseDate = new DateTime(1998, 1, 21),
                        Platform = "PlayStation",
                        Genre = "Survival Horror",
                        Developer = "Capcom",
                        Publisher = "Capcom",
                        Rating = "M"
                    },
                    new Game
                    {
                        Title = "Half-Life",
                        Description = "A revolutionary FPS that seamlessly integrated storytelling with gameplay, featuring minimal cutscenes and immersive world-building.",
                        ImageUrl = "https://upload.wikimedia.org/wikipedia/en/f/fa/Half-Life_Cover_Art.jpg",
                        MetaScore = 96,
                        ReleaseDate = new DateTime(1998, 11, 19),
                        Platform = "PC, PlayStation 2",
                        Genre = "FPS",
                        Developer = "Valve",
                        Publisher = "Sierra Entertainment",
                        Rating = "M"
                    },
                    new Game
                    {
                        Title = "Star Fox 64",
                        Description = "An on-rails shooter featuring branching paths, memorable characters, and iconic voice acting.",
                        ImageUrl = "https://upload.wikimedia.org/wikipedia/en/9/9d/Sf64_box.jpg",
                        MetaScore = 91,
                        ReleaseDate = new DateTime(1997, 4, 27),
                        Platform = "Nintendo 64",
                        Genre = "Shooter",
                        Developer = "Nintendo",
                        Publisher = "Nintendo",
                        Rating = "E"
                    },
                    new Game
                    {
                        Title = "Planescape: Torment",
                        Description = "A philosophically rich RPG with exceptional writing and unique setting, focusing on narrative over combat.",
                        ImageUrl = "https://upload.wikimedia.org/wikipedia/en/8/81/Planescape-torment-box.jpg",
                        MetaScore = 91,
                        ReleaseDate = new DateTime(1999, 12, 12),
                        Platform = "PC",
                        Genre = "RPG",
                        Developer = "Black Isle Studios",
                        Publisher = "Interplay Entertainment",
                        Rating = "T"
                    }
                };
                
                await context.Games.AddRangeAsync(classicGames);
            }
        }

        public static async Task SeedMoreModernGamesAsync(ApplicationDbContext context)
        {
            if (!context.Games.Any(g => g.Title == "Astro Bot"))
            {
                var newGames = new List<Game>
                {
                    new Game
                    {
                        Title = "Astro Bot",
                        Description = "A charming and innovative platformer that showcases the best of PlayStation technology.",
                        ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202401/2415/1c3c4f8f6bc5ebcf8750ae13e1134490f9924da4cb8d8fb6.jpg",
                        TrailerUrl = "https://www.youtube.com/embed/LPfhY0-TwY4",
                        Screenshots = new List<string>
                        {
                            "https://assets.reedpopcdn.com/astro-bot-rescue-mission-review-a-mini-marvel-comes-to-psvr-1538472227149.jpg",
                            "https://assets.reedpopcdn.com/astro-bot-rescue-mission-review-a-mini-marvel-comes-to-psvr-1538472227149.jpg",
                            "https://assets.reedpopcdn.com/astro-bot-rescue-mission-review-a-mini-marvel-comes-to-psvr-1538472227149.jpg"
                        },
                        MetaScore = 95,
                        ReleaseDate = new DateTime(2025, 3, 18),
                        Platform = "PS5",
                        Genre = "Platformer",
                        Developer = "Team Asobi",
                        Publisher = "Sony Interactive Entertainment",
                        Rating = "E"
                    },
                    new Game
                    {
                        Title = "Resident Evil 9",
                        Description = "The next mainline entry in Capcom's legendary survival horror series.",
                        ImageUrl = "https://cdn.wccftech.com/wp-content/uploads/2024/01/resident-evil-9-scaled.jpg",
                        TrailerUrl = "https://www.youtube.com/embed/Ffox_C_DWpU",
                        Screenshots = new List<string>
                        {
                            "https://cdn.wccftech.com/wp-content/uploads/2024/01/resident-evil-9-scaled.jpg",
                            "https://cdn.wccftech.com/wp-content/uploads/2024/01/resident-evil-9-scaled.jpg",
                            "https://cdn.wccftech.com/wp-content/uploads/2024/01/resident-evil-9-scaled.jpg"
                        },
                        MetaScore = 88,
                        ReleaseDate = new DateTime(2025, 3, 13),
                        Platform = "PC, PS5, Xbox Series X",
                        Genre = "Survival Horror",
                        Developer = "Capcom",
                        Publisher = "Capcom",
                        Rating = "M"
                    },
                    new Game
                    {
                        Title = "SoulCalibur",
                        Description = "This is a tale of souls and swords, transcending the world and all its history, told for all eternity...",
                        ImageUrl = "https://upload.wikimedia.org/wikipedia/en/8/89/Soul_Calibur_flyer.jpg",
                        TrailerUrl = "https://www.youtube.com/embed/RtF80UCgarY",
                        Screenshots = new List<string>
                        {
                            "https://www.mobygames.com/images/shots/l/436890-soulcalibur-dreamcast-screenshot-mitsurugi-vs-taki.jpg",
                            "https://www.mobygames.com/images/shots/l/436891-soulcalibur-dreamcast-screenshot-sophitia-vs-siegfried.png",
                            "https://www.mobygames.com/images/shots/l/436892-soulcalibur-dreamcast-screenshot-voldo-vs-rock.jpg"
                        },
                        MetaScore = 98,
                        ReleaseDate = new DateTime(1999, 9, 8),
                        Platform = "Dreamcast",
                        Genre = "Fighting",
                        Developer = "Namco",
                        Publisher = "Namco",
                        Rating = "T"
                    },
                    new Game
                    {
                        Title = "Warhammer 40,000: Space Marine 2",
                        Description = "Take on the role of a superhuman Space Marine and face the threats to humanity in this brutal action game.",
                        ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/2183900/header.jpg",
                        MetaScore = 86,
                        ReleaseDate = new DateTime(2025, 3, 8),
                        Platform = "PC, PS5, Xbox Series X",
                        Genre = "Action",
                        Developer = "Saber Interactive",
                        Publisher = "Focus Entertainment",
                        Rating = "M"
                    },
                    new Game
                    {
                        Title = "Assassin's Creed Shadows",
                        Description = "The next chapter in the Assassin's Creed saga, featuring a new historical setting and enhanced stealth mechanics.",
                        ImageUrl = "https://cdn.wccftech.com/wp-content/uploads/2024/01/assassins-creed-codename-red.jpg",
                        MetaScore = 89,
                        ReleaseDate = new DateTime(2025, 3, 23),
                        Platform = "PC, PS5, Xbox Series X",
                        Genre = "Action RPG",
                        Developer = "Ubisoft",
                        Publisher = "Ubisoft",
                        Rating = "M"
                    },
                    new Game
                    {
                        Title = "Marvel vs. Capcom Fighting Collection",
                        Description = "A comprehensive collection of the legendary crossover fighting game series with modern enhancements.",
                        ImageUrl = "https://cdn.cloudflare.steamstatic.com/steam/apps/2446040/header.jpg",
                        MetaScore = 87,
                        ReleaseDate = new DateTime(2025, 3, 28),
                        Platform = "PC, PS5, Xbox Series X, Nintendo Switch",
                        Genre = "Fighting",
                        Developer = "Capcom",
                        Publisher = "Capcom",
                        Rating = "T"
                    }
                };

                await context.Games.AddRangeAsync(newGames);
                await context.SaveChangesAsync();
            }
        }
    }
} 