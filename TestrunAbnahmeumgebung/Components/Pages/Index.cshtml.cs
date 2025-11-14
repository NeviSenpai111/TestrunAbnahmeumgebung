using System.Data.Entity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TestrunAbnahmeumgebung.Components.Pages;

public class IndexModel : PageModel
{
    private readonly TestDbContext _db;

    public List<Ping>? Pings { get; private set; }

    public IndexModel(TestDbContext db)
    {
        _db = db;
    }

    public async Task OnGetAsync()
    {
        // Create a record (optional) then read
        _db.Pings.Add(new Ping { Timestamp = DateTime.UtcNow });
        await _db.SaveChangesAsync();

        Pings = await _db.Pings.OrderByDescending(p => p.Id).Take(10).ToListAsync();
    }
}