namespace EntityGen;

/// <summary>
/// Central factory manager for creating database connections
/// Implements the Factory pattern for database abstraction
/// </summary>
public static class DatabaseFactoryManager
{
    private static readonly Dictionary<DatabaseType, IDatabaseFactory> _factories;

    static DatabaseFactoryManager()
    {
        _factories = new Dictionary<DatabaseType, IDatabaseFactory>
        {
            { DatabaseType.SqlServer, new SqlServerFactory() },
            { DatabaseType.PostgreSQL, new PostgreSqlFactory() }
        };
    }

    /// <summary>
    /// Creates a database instance for the specified database type
    /// </summary>
    /// <param name="databaseType">The type of database to create</param>
    /// <param name="connectionString">The connection string for the database</param>
    /// <returns>A database instance</returns>
    /// <exception cref="NotSupportedException">Thrown when database type is not supported</exception>
    public static IDatabase CreateDatabase(DatabaseType databaseType, string connectionString)
    {
        if (!_factories.TryGetValue(databaseType, out var factory))
        {
            throw new NotSupportedException($"Database type '{databaseType}' is not supported.");
        }

        return factory.CreateDatabase(connectionString);
    }

    /// <summary>
    /// Registers a custom database factory
    /// Allows for extending the system with additional database types
    /// </summary>
    /// <param name="databaseType">The database type</param>
    /// <param name="factory">The factory to create instances of that database type</param>
    public static void RegisterFactory(DatabaseType databaseType, IDatabaseFactory factory)
    {
        _factories[databaseType] = factory;
    }

    /// <summary>
    /// Gets all supported database types
    /// </summary>
    /// <returns>An array of supported database types</returns>
    public static DatabaseType[] GetSupportedDatabaseTypes()
    {
        return _factories.Keys.ToArray();
    }

    /// <summary>
    /// Checks if a database type is supported
    /// </summary>
    /// <param name="databaseType">The database type to check</param>
    /// <returns>True if the database type is supported, false otherwise</returns>
    public static bool IsSupported(DatabaseType databaseType)
    {
        return _factories.ContainsKey(databaseType);
    }
}
