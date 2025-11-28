using Microsoft.EntityFrameworkCore;
using TestrunAbnahmeumgebung.Models;
using TestrunAbnahmeumgebung.Services;

var builder = WebApplication.CreateBuilder(args);

// Try to read connection string; if missing, fall back to InMemory for local/dev testing
var connectionString = builder.Configuration.GetConnectionString("AppDB");

if (string.IsNullOrWhiteSpace(connectionString))
{
    // Use InMemory DB for local testing so the app won't fail to start without a production DB
    builder.Services.AddDbContext<TestDbContext>(options =>
        options.UseInMemoryDatabase("TestDb_InMemory"));
}
else
{
    builder.Services.AddDbContext<TestDbContext>(options =>
        options.UseSqlServer(connectionString,
            sqlOptions => sqlOptions.EnableRetryOnFailure()));
}

// Register DB checker service
builder.Services.AddSingleton<IDbChecker, DbChecker>();

// Use MVC (Controllers + Views)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Try to create database/schema at startup to avoid runtime table-missing errors.
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<TestDbContext>();
        // This will create the database and schema for InMemory or create DB if allowed (SQL Server)
        db.Database.EnsureCreated();
        logger?.LogInformation("Database ensure/create executed.");
    }
    catch (Exception ex)
    {
        logger?.LogError(ex, "Database ensure/create failed on startup.");
    }
}

app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
