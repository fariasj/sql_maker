namespace EntityGen;

/// <summary>
/// Database configuration settings
/// Loaded from appsettings.json
/// </summary>
public class DatabaseSettings
{
    /// <summary>
    /// Gets or sets the database type to use (SqlServer or PostgreSQL)
    /// </summary>
    public DatabaseType DatabaseType { get; set; }

    /// <summary>
    /// Gets or sets the connection strings for different database types
    /// Key format: "SqlServer", "PostgreSQL"
    /// </summary>
    public Dictionary<string, string> ConnectionStrings { get; set; } = new();

    /// <summary>
    /// Gets the connection string for the configured database type
    /// </summary>
    /// <returns>The connection string</returns>
    /// <exception cref="InvalidOperationException">Thrown when connection string is not found</exception>
    public string GetConnectionString()
    {
        var key = DatabaseType.ToString();
        if (ConnectionStrings.TryGetValue(key, out var connectionString))
        {
            return connectionString;
        }

        throw new InvalidOperationException($"Connection string for '{DatabaseType}' not found in configuration.");
    }
}
