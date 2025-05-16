using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebGame.Migrations
{
    /// <inheritdoc />
    public partial class Game4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Age = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VideoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrailerUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Screenshots = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetaScore = table.Column<int>(type: "int", nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Genre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Developer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Publisher = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserScore = table.Column<int>(type: "int", nullable: false),
                    ReviewCount = table.Column<int>(type: "int", nullable: false),
                    UserReviewCount = table.Column<int>(type: "int", nullable: false),
                    TrendingScore = table.Column<float>(type: "real", nullable: false),
                    Screenshot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rating = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MustPlay = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Platforms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Manufacturer = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Platforms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NewsPosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PublishDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    GameCategoryId = table.Column<int>(type: "int", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GameCategoryId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewsPosts_GameCategories_GameCategoryId",
                        column: x => x.GameCategoryId,
                        principalTable: "GameCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_NewsPosts_GameCategories_GameCategoryId1",
                        column: x => x.GameCategoryId1,
                        principalTable: "GameCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCriticReview = table.Column<bool>(type: "bit", nullable: false),
                    HelpfulCount = table.Column<int>(type: "int", nullable: false),
                    UnhelpfulCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GamePlatforms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    PlatformId = table.Column<int>(type: "int", nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamePlatforms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GamePlatforms_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GamePlatforms_Platforms_PlatformId",
                        column: x => x.PlatformId,
                        principalTable: "Platforms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "GameCategories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Action games focus on physical challenges, including hand–eye coordination and reaction-time.", "Action" },
                    { 2, "Adventure games focus on puzzle solving within a narrative framework.", "Adventure" },
                    { 3, "Role-playing games where players assume the roles of characters in a fictional setting.", "RPG" },
                    { 4, "Strategy games focus on skillful thinking and planning to achieve victory.", "Strategy" },
                    { 5, "Sports games simulate the practice of traditional physical sports.", "Sports" },
                    { 6, "Racing games involve the player participating in racing competitions.", "Racing" },
                    { 7, "Simulation games are designed to simulate real world activities.", "Simulation" },
                    { 8, "Fighting games emphasize one-on-one combat between characters.", "Fighting" }
                });

            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "Id", "Description", "Developer", "Genre", "ImageUrl", "MetaScore", "MustPlay", "Platform", "Publisher", "Rating", "ReleaseDate", "ReviewCount", "Screenshot", "Screenshots", "Title", "TrailerUrl", "TrendingScore", "UserReviewCount", "UserScore", "VideoUrl" },
                values: new object[,]
                {
                    { 1, "Baldur's Gate 3 is a role-playing game developed and published by Larian Studios. Choose from a wide range of D&D races and classes, or play as an origin character with a hand-crafted background.", "Larian Studios", "RPG", "/images/games/baldurs-gate-3.jpg", 96, false, "Not specified", "Larian Studios", "M", new DateTime(2023, 8, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, null, null, "Baldur's Gate 3", null, 0f, 0, 0, null },
                    { 2, "Marvel's Spider-Man 2 is an action-adventure game featuring both Peter Parker and Miles Morales.", "Insomniac Games", "Action-Adventure", "/images/games/spiderman2.jpg", 90, false, "Not specified", "Sony Interactive Entertainment", "T", new DateTime(2023, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, null, null, "Marvel's Spider-Man 2", null, 0f, 0, 0, null },
                    { 3, "Resident Evil 4 is a survival horror game developed and published by Capcom.", "Capcom", "Survival Horror", "/images/games/re4.jpg", 93, false, "Not specified", "Capcom", "M", new DateTime(2023, 3, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, null, null, "Resident Evil 4 Remake", null, 0f, 0, 0, null },
                    { 4, "The Legend of Zelda: Tears of the Kingdom is an action-adventure game developed and published by Nintendo, the sequel to Breath of the Wild.", "Nintendo", "Action-Adventure", "/images/games/zelda-totk.jpg", 96, false, "Not specified", "Nintendo", "E10+", new DateTime(2023, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, null, null, "The Legend of Zelda: Tears of the Kingdom", null, 0f, 0, 0, null },
                    { 5, "Final Fantasy XVI is an action role-playing game developed and published by Square Enix.", "Square Enix", "Action RPG", "/images/games/ff16.jpg", 87, false, "Not specified", "Square Enix", "M", new DateTime(2023, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, null, null, "Final Fantasy XVI", null, 0f, 0, 0, null },
                    { 6, "Star Wars Jedi: Survivor is an action-adventure game developed by Respawn Entertainment.", "Respawn Entertainment", "Action-Adventure", "/images/games/jedi-survivor.jpg", 85, false, "Not specified", "Electronic Arts", "T", new DateTime(2023, 4, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, null, null, "Star Wars Jedi: Survivor", null, 0f, 0, 0, null },
                    { 7, "Dead Space is a survival horror game developed by Motive Studio and published by Electronic Arts.", "Motive Studio", "Survival Horror", "/images/games/dead-space.jpg", 89, false, "Not specified", "Electronic Arts", "M", new DateTime(2023, 1, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, null, null, "Dead Space Remake", null, 0f, 0, 0, null },
                    { 8, "Hi-Fi Rush is a rhythm-action game developed by Tango Gameworks and published by Bethesda Softworks.", "Tango Gameworks", "Action Rhythm", "/images/games/hifi-rush.jpg", 88, false, "Not specified", "Bethesda Softworks", "T", new DateTime(2023, 1, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, null, null, "Hi-Fi Rush", null, 0f, 0, 0, null },
                    { 9, "Street Fighter 6 is a fighting game developed and published by Capcom.", "Capcom", "Fighting", "/images/games/sf6.jpg", 92, false, "Not specified", "Capcom", "T", new DateTime(2023, 6, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, null, null, "Street Fighter 6", null, 0f, 0, 0, null },
                    { 10, "Diablo IV is an action role-playing game developed and published by Blizzard Entertainment.", "Blizzard Entertainment", "Action RPG", "/images/games/diablo4.jpg", 88, false, "Not specified", "Blizzard Entertainment", "M", new DateTime(2023, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, null, null, "Diablo IV", null, 0f, 0, 0, null },
                    { 11, "Lies of P is a souls-like action RPG inspired by the story of Pinocchio.", "Neowiz Games", "Action RPG", "/images/games/lies-of-p.jpg", 84, false, "Not specified", "Neowiz Games", "M", new DateTime(2023, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, null, null, "Lies of P", null, 0f, 0, 0, null },
                    { 12, "Armored Core VI: Fires of Rubicon is an action game developed by FromSoftware.", "FromSoftware", "Action", "/images/games/armored-core-6.jpg", 87, false, "Not specified", "Bandai Namco", "T", new DateTime(2023, 8, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, null, null, "Armored Core VI", null, 0f, 0, 0, null },
                    { 13, "Sea of Stars is a turn-based RPG inspired by classic 16-bit RPGs.", "Sabotage Studio", "RPG", "/images/games/sea-of-stars.jpg", 89, false, "Not specified", "Sabotage Studio", "E10+", new DateTime(2023, 8, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, null, null, "Sea of Stars", null, 0f, 0, 0, null },
                    { 14, "Starfield is an action role-playing game developed by Bethesda Game Studios.", "Bethesda Game Studios", "Action RPG", "/images/games/starfield.jpg", 83, false, "Not specified", "Bethesda Softworks", "M", new DateTime(2023, 9, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, null, null, "Starfield", null, 0f, 0, 0, null },
                    { 15, "Alan Wake 2 is a survival horror game developed by Remedy Entertainment.", "Remedy Entertainment", "Survival Horror", "/images/games/alan-wake-2.jpg", 89, false, "Not specified", "Epic Games", "M", new DateTime(2023, 10, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, null, null, "Alan Wake 2", null, 0f, 0, 0, null },
                    { 16, "Super Mario Bros. Wonder is a 2D side-scrolling platform game developed by Nintendo.", "Nintendo", "Platform", "/images/games/mario-wonder.jpg", 92, false, "Not specified", "Nintendo", "E", new DateTime(2023, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, null, null, "Super Mario Bros. Wonder", null, 0f, 0, 0, null },
                    { 17, "Octopath Traveler II is a turn-based RPG featuring eight different characters with unique stories.", "Square Enix", "RPG", "/images/games/octopath2.jpg", 87, false, "Not specified", "Square Enix", "T", new DateTime(2023, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, null, null, "Octopath Traveler II", null, 0f, 0, 0, null },
                    { 18, "Assassin's Creed Mirage is an action-adventure game set in 9th century Baghdad.", "Ubisoft Bordeaux", "Action-Adventure", "/images/games/ac-mirage.jpg", 77, false, "Not specified", "Ubisoft", "M", new DateTime(2023, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, null, null, "Assassin's Creed Mirage", null, 0f, 0, 0, null },
                    { 19, "Mortal Kombat 1 is a fighting game developed by NetherRealm Studios.", "NetherRealm Studios", "Fighting", "/images/games/mk1.jpg", 84, false, "Not specified", "Warner Bros. Games", "M", new DateTime(2023, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, null, null, "Mortal Kombat 1", null, 0f, 0, 0, null },
                    { 20, "Atomic Heart is a first-person shooter set in an alternate reality Soviet Union.", "Mundfish", "Action FPS", "/images/games/atomic-heart.jpg", 75, false, "Not specified", "Focus Entertainment", "M", new DateTime(2023, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, null, null, "Atomic Heart", null, 0f, 0, 0, null },
                    { 21, "Forza Motorsport is a racing simulation game developed by Turn 10 Studios.", "Turn 10 Studios", "Racing", "/images/games/forza-motorsport.jpg", 86, false, "Not specified", "Xbox Game Studios", "E", new DateTime(2023, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, null, null, "Forza Motorsport", null, 0f, 0, 0, null },
                    { 22, "Wild Hearts is a hunting action game where you battle giant beasts using ancient technology.", "Omega Force", "Action RPG", "/images/games/wild-hearts.jpg", 76, false, "Not specified", "Electronic Arts", "T", new DateTime(2023, 2, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, null, null, "Wild Hearts", null, 0f, 0, 0, null },
                    { 23, "Remnant II is an action-shooter survival game set in a post-apocalyptic world.", "Gunfire Games", "Action", "/images/games/remnant2.jpg", 85, false, "Not specified", "Gearbox Publishing", "M", new DateTime(2023, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, null, null, "Remnant II", null, 0f, 0, 0, null },
                    { 24, "Pikmin 4 is a real-time strategy and puzzle game developed by Nintendo.", "Nintendo", "Strategy", "/images/games/pikmin4.jpg", 87, false, "Not specified", "Nintendo", "E10+", new DateTime(2023, 7, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, null, null, "Pikmin 4", null, 0f, 0, 0, null }
                });

            migrationBuilder.InsertData(
                table: "Platforms",
                columns: new[] { "Id", "Manufacturer", "Name", "ReleaseDate" },
                values: new object[,]
                {
                    { 1, "Sony", "PlayStation 5", new DateTime(2020, 11, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "Sony", "PlayStation 4", new DateTime(2013, 11, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "Sony", "PlayStation 3", new DateTime(2006, 11, 11, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "Microsoft", "Xbox Series X|S", new DateTime(2020, 11, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, "Microsoft", "Xbox One", new DateTime(2013, 11, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, "Microsoft", "Xbox 360", new DateTime(2005, 11, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, "Nintendo", "Nintendo Switch", new DateTime(2017, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 8, "Nintendo", "Nintendo Wii U", new DateTime(2012, 11, 18, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 9, "Nintendo", "Nintendo Wii", new DateTime(2006, 11, 19, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 10, "Nintendo", "Nintendo 3DS", new DateTime(2011, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 11, "Various", "PC", null },
                    { 12, "Nintendo", "Nintendo 64", new DateTime(1996, 6, 23, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 13, "Sony", "PlayStation", new DateTime(1994, 12, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 14, "Sony", "PlayStation 2", new DateTime(2000, 3, 4, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "GamePlatforms",
                columns: new[] { "Id", "GameId", "PlatformId", "ReleaseDate" },
                values: new object[,]
                {
                    { 14, 9, 2, new DateTime(2018, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 15, 9, 11, new DateTime(2022, 1, 14, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 16, 10, 2, new DateTime(2015, 5, 19, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 17, 10, 5, new DateTime(2015, 5, 19, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 18, 10, 11, new DateTime(2015, 5, 19, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 19, 10, 7, new DateTime(2019, 10, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 20, 11, 2, new DateTime(2020, 3, 31, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 21, 11, 7, new DateTime(2022, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 22, 12, 1, new DateTime(2022, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 23, 12, 2, new DateTime(2022, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 24, 12, 4, new DateTime(2022, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 25, 12, 5, new DateTime(2022, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 26, 12, 11, new DateTime(2022, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 27, 13, 2, new DateTime(2020, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 28, 13, 11, new DateTime(2021, 12, 16, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 29, 14, 1, new DateTime(2020, 12, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 30, 14, 2, new DateTime(2020, 12, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 31, 14, 4, new DateTime(2020, 12, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 32, 14, 5, new DateTime(2020, 12, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 33, 14, 11, new DateTime(2020, 12, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 34, 15, 1, new DateTime(2021, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 35, 15, 2, new DateTime(2020, 7, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 36, 16, 2, new DateTime(2017, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 37, 16, 11, new DateTime(2020, 8, 7, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 38, 17, 2, new DateTime(2019, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 39, 17, 5, new DateTime(2019, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 40, 17, 11, new DateTime(2019, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 41, 18, 2, new DateTime(2021, 8, 13, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 42, 18, 1, new DateTime(2021, 8, 13, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 43, 18, 5, new DateTime(2021, 8, 13, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 44, 18, 4, new DateTime(2021, 8, 13, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 45, 18, 7, new DateTime(2020, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 46, 18, 11, new DateTime(2020, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 47, 19, 2, new DateTime(2019, 11, 8, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 48, 19, 11, new DateTime(2020, 7, 14, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 49, 20, 1, new DateTime(2021, 3, 26, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 50, 20, 2, new DateTime(2021, 3, 26, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 51, 20, 4, new DateTime(2021, 3, 26, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 52, 20, 5, new DateTime(2021, 3, 26, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 53, 20, 11, new DateTime(2021, 3, 26, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 54, 21, 7, new DateTime(2021, 3, 26, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 55, 21, 11, new DateTime(2022, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 56, 21, 1, new DateTime(2023, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 57, 21, 2, new DateTime(2023, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 58, 21, 4, new DateTime(2023, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 59, 21, 5, new DateTime(2023, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 60, 22, 1, new DateTime(2021, 9, 14, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 61, 22, 11, new DateTime(2021, 9, 14, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 62, 22, 4, new DateTime(2022, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 63, 23, 1, new DateTime(2021, 6, 11, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 64, 23, 11, new DateTime(2023, 7, 26, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 65, 24, 1, new DateTime(2020, 11, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_GamePlatforms_GameId",
                table: "GamePlatforms",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GamePlatforms_PlatformId",
                table: "GamePlatforms",
                column: "PlatformId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_ReleaseDate",
                table: "Games",
                column: "ReleaseDate");

            migrationBuilder.CreateIndex(
                name: "IX_Games_Title",
                table: "Games",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_NewsPosts_CreatedAt",
                table: "NewsPosts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_NewsPosts_GameCategoryId",
                table: "NewsPosts",
                column: "GameCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsPosts_GameCategoryId1",
                table: "NewsPosts",
                column: "GameCategoryId1");

            migrationBuilder.CreateIndex(
                name: "IX_NewsPosts_Title",
                table: "NewsPosts",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_GameId",
                table: "Reviews",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "GamePlatforms");

            migrationBuilder.DropTable(
                name: "NewsPosts");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Platforms");

            migrationBuilder.DropTable(
                name: "GameCategories");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
