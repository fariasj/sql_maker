namespace EntityGen;

/// <summary>
/// Factory interface for creating database connections
/// Implements the Abstract Factory pattern for database creation
/// </summary>
public interface IDatabaseFactory
{
    /// <summary>
    /// Gets the database type this factory creates
    /// </summary>
    DatabaseType DatabaseType { get; }

    /// <summary>
    /// Creates a database instance with the specified connection string
    /// </summary>
    /// <param name="connectionString">The connection string for the database</param>
    /// <returns>A database instance</returns>
    IDatabase CreateDatabase(string connectionString);
}
