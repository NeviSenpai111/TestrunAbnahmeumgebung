using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestrunAbnahmeumgebung.Models;

namespace TestrunAbnahmeumgebung.Services
{
    public class DbChecker : IDbChecker
    {
        private readonly ILogger<DbChecker> _logger;

        public DbChecker(ILogger<DbChecker> logger)
        {
            _logger = logger;
        }

        public async Task<bool> ExistsAsync(TestDbContext? context, CancellationToken ct = default)
        {
            if (context == null)
            {
                _logger.LogWarning("DbContext is null when checking DB existence.");
                return false;
            }

            try
            {
                var provider = context.Database.ProviderName ?? string.Empty;
                if (provider.Contains("InMemory", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation("Detected InMemory provider - treating DB as existing for test purposes.");
                    return true;
                }

                // Try a simple CanConnect if provider supports it
                if (context.Database.CanConnect())
                {
                    // We cannot confidently determine DB name here, but connection works
                    _logger.LogInformation("DbContext can connect to database.");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while checking DB existence via DbContext.");
                return false;
            }
        }

        public async Task<bool> ExistsAsync(string connectionString, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                _logger.LogWarning("Empty connection string passed to ExistsAsync.");
                return false;
            }

            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString);

                // Extract database name
                var dbName = builder.InitialCatalog;
                if (string.IsNullOrWhiteSpace(dbName))
                {
                    _logger.LogWarning("Connection string does not contain Initial Catalog / Database name.");
                    return false;
                }

                // Point to master DB for existence check
                builder.InitialCatalog = "master";

                // Ensure short timeout so health checks are fast
                if (builder.ConnectTimeout == 0)
                {
                    builder.ConnectTimeout = 5;
                }

                using (var conn = new SqlConnection(builder.ConnectionString))
                {
                    await conn.OpenAsync(ct);

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SELECT COUNT(1) FROM sys.databases WHERE name = @name";
                        cmd.Parameters.Add(new SqlParameter("@name", dbName));

                        var result = await cmd.ExecuteScalarAsync(ct);
                        if (result != null && int.TryParse(result.ToString(), out int count))
                        {
                            return count > 0;
                        }

                        return false;
                    }
                }
            }
            catch (SqlException ex)
            {
                _logger.LogWarning(ex, "SqlException while checking DB existence.");
                return false;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("DB existence check cancelled/timeout.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while checking DB existence.");
                return false;
            }
        }
    }
}

