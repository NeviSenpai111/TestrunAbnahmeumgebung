using Microsoft.EntityFrameworkCore;

namespace TestrunAbnahmeumgebung.Models
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options) { }

        public DbSet<Ping> Pings { get; set; } = null!;
    }
}

