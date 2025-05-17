using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using WebGame.Models;
using WebGame.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using WebGame.Data;
using WebGame.Services;
using WebGame.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Configure database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Use only one DbContext registration method - use the pooled version for better performance
builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.CommandTimeout(60);
        sqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(10), null);
        sqlOptions.MinBatchSize(5);
        sqlOptions.MaxBatchSize(100);
        sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
    
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
    options.EnableDetailedErrors(builder.Environment.IsDevelopment());
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
}, poolSize: 128); // Optimize pool size for better performance

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Thêm xác thực Google
builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "GOOGLE_CLIENT_ID";
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "GOOGLE_CLIENT_SECRET";
        googleOptions.CallbackPath = "/signin-google";
    });

// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(WebGame.Services.MappingProfile));

// Configure Memory Cache with optimized settings
builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 1024; // Limit cache size to 1 GB
    options.ExpirationScanFrequency = TimeSpan.FromMinutes(5); // Scan for expired items every 5 minutes
    options.CompactionPercentage = 0.1; // 10% compaction
});

// Add Response Caching for better performance
builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 64 * 1024 * 1024; // 64MB
    options.UseCaseSensitivePaths = false;
});

// Add response compression for better network performance
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.MimeTypes = new[] {
        "text/plain",
        "text/html",
        "text/css",
        "application/javascript",
        "application/json",
        "image/svg+xml",
        "application/xml"
    };
});

// Register Lazy<T> for deferred service initialization
builder.Services.AddTransient(typeof(Lazy<>), typeof(LazyService<>));

// Add Output Caching for better performance
builder.Services.AddOutputCache(options => {
    // Cache home page for 5 minutes
    options.AddPolicy("HomeCache", builder => 
        builder.Expire(TimeSpan.FromMinutes(5))
               .SetVaryByQuery("page", "category")
               .Tag("home"));
               
    // Cache game list for 10 minutes
    options.AddPolicy("GameListCache", builder => 
        builder.Expire(TimeSpan.FromMinutes(10))
               .SetVaryByQuery("page", "genre", "platform", "sort")
               .Tag("games"));
});

// Configure Swagger/OpenAPI
// builder.Services.AddSwaggerGen(c =>
// {
//     c.SwaggerDoc("v1", new OpenApiInfo { Title = "Game Reviews API", Version = "v1" });
// });

// Configure Identity options
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

// Add Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("Admin"));
});

builder.Services.AddRazorPages();

builder.Services.AddDistributedMemoryCache(); // Cần thiết để lưu trữ session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20); // Reduced from 30 to 20 minutes for better resource usage
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Strict; // Enhance security
});

builder.Services.AddTransient<IGameImageService, GameImageService>();
builder.Services.AddTransient<INewsPostRepository, EFNewsPostRepository>();

// Configure cookie policy
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// Tăng cường logging trong Development
if (builder.Environment.IsDevelopment())
{
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
    builder.Logging.AddDebug();
    builder.Logging.SetMinimumLevel(LogLevel.Information);
}

var app = builder.Build();

// Initialize the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        logger.LogInformation("Checking database connection...");
        bool canConnect = false;
        try
        {
            // Test if we can connect to the database
            canConnect = context.Database.CanConnect();
            logger.LogInformation($"Database connection status: {canConnect}");
        }
        catch (Exception connEx)
        {
            logger.LogError(connEx, "Error connecting to database");
            // Continue execution even if connection fails initially
        }
        
        logger.LogInformation("Attempting to migrate database...");
        try
        {
            // Apply migrations instead of EnsureCreated for more robustness
            context.Database.Migrate();
            logger.LogInformation("Database migration completed successfully");
        }
        catch (Exception migrateEx)
        {
            logger.LogError(migrateEx, "Error during database migration. Attempting to ensure created instead.");
            try 
            {
                // Fallback to EnsureCreated if migrations fail
                context.Database.EnsureCreated();
                logger.LogInformation("Database created successfully using EnsureCreated");
            }
            catch (Exception createEx)
            {
                logger.LogError(createEx, "Failed to create database. Application may not function correctly.");
            }
        }
        
        // Check if Games table exists and has data
        try
        {
            bool hasGames = context.Games.Any();
            logger.LogInformation($"Games table exists and has data: {hasGames}");
            
            if (!hasGames)
            {
                logger.LogInformation("Seeding initial game data");
                try 
                {
                    var gameLogger = services.GetRequiredService<ILogger<GameController>>();
                    var gameImageService = services.GetRequiredService<IGameImageService>();
                    var gameController = new GameController(context, gameLogger, gameImageService);
                    
                    // Gọi phương thức SeedSampleData không cần đợi kết quả IActionResult
                    await gameController.SeedSampleData();
                    logger.LogInformation("Seed data completed successfully");
                }
                catch (Exception seedEx)
                {
                    logger.LogError(seedEx, "Error occurred while seeding game data");
                }
            }
        }
        catch (Exception tableEx)
        {
            logger.LogError(tableEx, "Error checking Games table. Table may not exist yet.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}

// Call Initialize on SeedData to create the PublishDate column and set values
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Calling SeedData.Initialize to initialize the database");
        WebGame.Data.SeedData.Initialize(services);
        
        // Thêm khoảng cách để kiểm tra xem có dữ liệu trò chơi không
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            int gameCount = context.Games.Count();
            logger.LogInformation($"Database initialized with {gameCount} games");
            
            // Nếu vẫn chưa có game, thử gọi trực tiếp các phương thức seed
            if (gameCount < 30)
            {
                logger.LogWarning("Game count is low, attempting to seed more games...");
                try
                {
                    await WebGame.Data.SeedData.SeedMoreClassicGamesAsync(context);
                    await WebGame.Data.SeedData.SeedMoreModernGamesAsync(context);
                    await context.SaveChangesAsync();
                    logger.LogInformation($"After additional seeding: {context.Games.Count()} games in database");
                }
                catch (Exception seedMoreEx)
                {
                    logger.LogError(seedMoreEx, "Error seeding additional games");
                }
            }
        }
        catch (Exception countEx)
        {
            logger.LogError(countEx, "Error counting games. Table may not exist yet.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Add response compression and caching
app.UseResponseCompression();
app.UseResponseCaching();
app.UseOutputCache();

app.UseRouting();

// Add these lines for Identity
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

// Configure endpoints
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
    
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Add this line to map Identity pages
app.MapRazorPages();

// Disable Swagger to prevent port binding issues
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Game Reviews API v1"));
// }

app.Run();
