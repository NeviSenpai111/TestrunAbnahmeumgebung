using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Use your connection string name (as configured in App Service)
var connectionString = builder.Configuration.GetConnectionString("AppDb");

builder.Services.AddDbContext<TestDbContext>(options =>
    options.UseSqlServer(connectionString,
        sqlOptions => sqlOptions.EnableRetryOnFailure()));

// Optionally add Razor Pages
builder.Services.AddRazorPages();

var app = builder.Build();

// Map Razor Pages
app.MapRazorPages();

app.Run();


// Define your DbContext
public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options)
        : base(options) { }

    public DbSet<Ping> Pings { get; set; } = null!;
}

// Define a simple model
public class Ping
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
}