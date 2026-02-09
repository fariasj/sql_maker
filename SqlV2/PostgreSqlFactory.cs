using System.Data;

namespace EntityGen;

/// <summary>
/// Factory for PostgreSQL databases
/// </summary>
public class PostgreSqlFactory : IDatabaseFactory
{
    /// <summary>
    /// Gets the database type this factory creates
    /// </summary>
    public DatabaseType DatabaseType => DatabaseType.PostgreSQL;

    /// <summary>
    /// Creates a PostgreSQL database instance
    /// </summary>
    /// <param name="connectionString">The connection string for PostgreSQL</param>
    /// <returns>A PostgreSQL database instance</returns>
    public IDatabase CreateDatabase(string connectionString)
    {
        return new PostgreSqlDatabase(connectionString);
    }
}
