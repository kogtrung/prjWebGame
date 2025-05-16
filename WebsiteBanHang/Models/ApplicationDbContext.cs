namespace WebGame.Models
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using WebGame.Models;
    using WebGame.Data;
    using System;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
            // Set recommended performance options
            this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            this.ChangeTracker.AutoDetectChangesEnabled = false;
        }
        
        public DbSet<GameCategory> GameCategories { get; set; }
        public DbSet<NewsPost> NewsPosts { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<GamePlatform> GamePlatforms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

          
                
            // NewsPosts
            modelBuilder.Entity<NewsPost>()
                .HasIndex(n => n.CreatedAt);
                
            modelBuilder.Entity<NewsPost>()
                .HasIndex(n => n.GameCategoryId);
                
            modelBuilder.Entity<NewsPost>()
                .HasIndex(n => n.Title);
                
            // Games
            modelBuilder.Entity<Game>()
                .HasIndex(g => g.Title);
                
            modelBuilder.Entity<Game>()
                .HasIndex(g => g.ReleaseDate);
                
            // Reviews
            modelBuilder.Entity<Review>()
                .HasIndex(r => r.GameId);
                
            modelBuilder.Entity<Review>()
                .HasIndex(r => r.UserId);
                
            // Configure relationships
            modelBuilder.Entity<NewsPost>()
                .HasOne(n => n.GameCategory)
                .WithMany()
                .HasForeignKey(n => n.GameCategoryId)
                .OnDelete(DeleteBehavior.SetNull);
                
            
                
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Game)
                .WithMany(g => g.Reviews)
                .HasForeignKey(r => r.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure GamePlatform relationships
            modelBuilder.Entity<GamePlatform>()
                .HasOne(gp => gp.Game)
                .WithMany(g => g.GamePlatforms)
                .HasForeignKey(gp => gp.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GamePlatform>()
                .HasOne(gp => gp.Platform)
                .WithMany(p => p.GamePlatforms)
                .HasForeignKey(gp => gp.PlatformId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed data in correct order
            // 1. Seed GameCategories first (required by NewsPost)
            GameCategorySeeder.SeedGameCategories(modelBuilder);
            
            // 2. Seed Platforms (required by GamePlatforms)
            PlatformSeeder.SeedPlatforms(modelBuilder);
            
            // 3. Seed Games
            GameSeeder.SeedGames(modelBuilder);
            
            // 4. Seed GamePlatforms (requires both Games and Platforms)
            GamePlatformSeeder.SeedGamePlatforms(modelBuilder);
        }
    }
}