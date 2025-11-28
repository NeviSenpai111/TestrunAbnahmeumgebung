using System.Threading;
using System.Threading.Tasks;
using TestrunAbnahmeumgebung.Models;

namespace TestrunAbnahmeumgebung.Services
{
    public interface IDbChecker
    {
        /// <summary>
        /// Prüft asynchron, ob die Datenbank, die im ConnectionString angegeben ist, auf dem Server existiert.
        /// Erwartet einen SQL Server Connection String.
        /// </summary>
        Task<bool> ExistsAsync(string connectionString, CancellationToken ct = default);

        /// <summary>
        /// Prüft anhand eines DbContext (z.B. TestDbContext). Bei InMemory-Providern wird true zurückgegeben.
        /// </summary>
        Task<bool> ExistsAsync(TestDbContext? context, CancellationToken ct = default);
    }
}

