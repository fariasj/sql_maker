using System.Data;
using Microsoft.Data.SqlClient;

namespace EntityGen;

/// <summary>
/// Factory for SQL Server databases
/// </summary>
public class SqlServerFactory : IDatabaseFactory
{
    /// <summary>
    /// Gets the database type this factory creates
    /// </summary>
    public DatabaseType DatabaseType => DatabaseType.SqlServer;

    /// <summary>
    /// Creates a SQL Server database instance
    /// </summary>
    /// <param name="connectionString">The connection string for SQL Server</param>
    /// <returns>A SQL Server database instance</returns>
    public IDatabase CreateDatabase(string connectionString)
    {
        return new SqlServerDatabase(connectionString);
    }
}
