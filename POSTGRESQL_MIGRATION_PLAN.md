# PostgreSQL Support Implementation Plan - EntityGen

## 📋 Executive Summary

Add PostgreSQL database support to EntityGen using the **Factory Pattern** for database abstraction, allowing the project to work with both SQL Server and PostgreSQL seamlessly.

---

## 🎯 Objectives

1. ✅ Implement Factory Pattern for database abstraction
2. ✅ Add PostgreSQL support alongside SQL Server
3. ✅ Maintain backward compatibility with existing SQL Server code
4. ✅ Allow runtime database selection via configuration
5. ✅ Support all existing CRUD operations for PostgreSQL

---

## 🏗️ Architecture Overview

### Current Architecture (Single Database)

```
┌─────────────────┐
│   Program.cs    │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│   DAConexion    │ ◄────── Hardcoded to SQL Server
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  SQL Server     │
└─────────────────┘
```

### Target Architecture (Multi-Database with Factory Pattern)

```
┌─────────────────┐
│   Program.cs    │
└────────┬────────┘
         │
         ▼
┌─────────────────────┐
│  IDatabaseFactory   │
└────────┬────────────┘
         │
    ┌────┴────┐
    ▼         ▼
┌──────────┐ ┌──────────┐
│SqlServer │ │PostgreSQL│
│ Factory  │ │ Factory  │
└─────┬────┘ └─────┬────┘
      │            │
      ▼            ▼
┌──────────┐ ┌──────────┐
│SqlServer │ │PostgreSQL│
│Conexion  │ │Conexion  │
└──────────┘ └──────────┘
     │            │
     └────┬───────┘
          ▼
   ┌──────────────┐
   │ IDatabase    │
   │  (Interface) │
   └──────────────┘
```

---

## 📐 Design Patterns

### 1. Factory Pattern

Creates database connections without specifying the exact class of object that will be created.

### 2. Abstract Factory Pattern

Provides an interface for creating families of related objects (Connection, Command, DataAdapter) without specifying their concrete classes.

### 3. Strategy Pattern

Different database implementations can be selected at runtime.

---

## 🔧 Core Components

### 1. Database Enum

```csharp
/// <summary>
/// Supported database types
/// </summary>
public enum DatabaseType
{
    SqlServer,
    PostgreSQL
}
```

### 2. IDatabase Interface

```csharp
/// <summary>
/// Abstract interface for database operations
/// </summary>
public interface IDatabase : IDisposable
{
    string ConnectionString { get; }
    IDbConnection Connection { get; }
    string InfoMessage { get; }

    void Open();
    void Close();

    IDbCommand CreateCommand(string commandText);
    IDataReader ExecuteReader(string commandText);
    int ExecuteNonQuery(string commandText);
    object ExecuteScalar(string commandText);

    DataTable ExecuteQuery(string commandText);
    DataSet ExecuteQueryDataSet(string commandText);

    DataTable GetSchema(string collectionName);
}
```

### 3. IDatabaseFactory Interface

```csharp
/// <summary>
/// Factory interface for creating database connections
/// </summary>
public interface IDatabaseFactory
{
    IDatabase CreateDatabase(string connectionString);
    DatabaseType DatabaseType { get; }
}
```

### 4. Concrete Implementations

#### 4.1 SqlServerFactory

```csharp
/// <summary>
/// Factory for SQL Server databases
/// </summary>
public class SqlServerFactory : IDatabaseFactory
{
    public DatabaseType DatabaseType => DatabaseType.SqlServer;

    public IDatabase CreateDatabase(string connectionString)
    {
        return new SqlServerDatabase(connectionString);
    }
}
```

#### 4.2 PostgreSqlFactory

```csharp>
/// <summary>
/// Factory for PostgreSQL databases
/// </summary>
public class PostgreSqlFactory : IDatabaseFactory
{
    public DatabaseType DatabaseType => DatabaseType.PostgreSQL;

    public IDatabase CreateDatabase(string connectionString)
    {
        return new PostgreSqlDatabase(connectionString);
    }
}
```

### 5. Database Factory Manager

```csharp
/// <summary>
/// Central factory manager for creating database connections
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

    public static IDatabase CreateDatabase(DatabaseType databaseType, string connectionString)
    {
        if (!_factories.TryGetValue(databaseType, out var factory))
        {
            throw new NotSupportedException($"Database type '{databaseType}' is not supported.");
        }

        return factory.CreateDatabase(connectionString);
    }
}
```

### 6. Concrete Database Classes

#### 6.1 SqlServerDatabase

```csharp
/// <summary>
/// SQL Server implementation of IDatabase
/// </summary>
public class SqlServerDatabase : IDatabase
{
    private SqlConnection? _connection;

    public string ConnectionString { get; }
    public IDbConnection Connection => _connection ?? throw new InvalidOperationException("Connection not initialized");
    public string InfoMessage { get; private set; } = string.Empty;

    public SqlServerDatabase(string connectionString)
    {
        ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public void Open()
    {
        _connection = new SqlConnection(ConnectionString);
        _connection.InfoMessage += OnInfoMessage;
        _connection.Open();
    }

    public DataTable GetSchema(string collectionName)
    {
        return _connection?.GetSchema(collectionName) ?? new DataTable();
    }

    // ... other interface implementations
}
```

#### 6.2 PostgreSqlDatabase

```csharp
/// <summary>
/// PostgreSQL implementation of IDatabase
/// </summary>
public class PostgreSqlDatabase : IDatabase
{
    private NpgsqlConnection? _connection;

    public string ConnectionString { get; }
    public IDbConnection Connection => _connection ?? throw new InvalidOperationException("Connection not initialized");
    public string InfoMessage { get; private set; } = string.Empty;

    public PostgreSqlDatabase(string connectionString)
    {
        ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public void Open()
    {
        _connection = new NpgsqlConnection(ConnectionString);
        _connection.Open();
    }

    public DataTable GetSchema(string collectionName)
    {
        // PostgreSQL doesn't support GetSchema like SQL Server
        // Need to implement custom schema retrieval
        return GetPostgreSqlSchema(collectionName);
    }

    private DataTable GetPostgreSqlSchema(string collectionName)
    {
        var table = new DataTable();

        switch (collectionName.ToLower())
        {
            case "tables":
                return QueryPostgreSqlTables();
            case "columns":
                return QueryPostgreSqlColumns();
            default:
                throw new NotSupportedException($"Schema collection '{collectionName}' is not supported for PostgreSQL.");
        }
    }

    private DataTable QueryPostgreSqlTables()
    {
        const string query = @"
            SELECT
                table_schema AS ""SchemaName"",
                table_name AS ""TableName"",
                table_type AS ""TableType""
            FROM information_schema.tables
            WHERE table_schema NOT IN ('pg_catalog', 'information_schema')
            ORDER BY table_schema, table_name";

        return ExecuteQuery(query);
    }

    private DataTable QueryPostgreSqlColumns()
    {
        const string query = @"
            SELECT
                table_schema AS ""SchemaName"",
                table_name AS ""TableName"",
                column_name AS ""ColumnName"",
                data_type AS ""TypeName"",
                character_maximum_length AS ""Length"",
                is_nullable AS ""IsNullable"",
                column_default AS ""DefaultValue""
            FROM information_schema.columns
            WHERE table_schema NOT IN ('pg_catalog', 'information_schema')
            ORDER BY table_schema, table_name, ordinal_position";

        return ExecuteQuery(query);
    }

    // ... other interface implementations
}
```

---

## 📦 NuGet Packages Required

### Current Packages (SQL Server)
- ✅ `Microsoft.Data.SqlClient` (v6.0.0)

### New Packages (PostgreSQL)
- ✅ `Npgsql` (v8.0.0) - PostgreSQL ADO.NET Data Provider
- ✅ `Npgsql.EntityFrameworkCore.PostgreSQL` (v8.0.0) - Optional: For EF Core support

### Updated Packages
- ✅ `Microsoft.Extensions.Configuration` (v10.0.0) - Already present
- ✅ `Microsoft.Extensions.Configuration.Json` (v10.0.0) - Already present
- ✅ `Microsoft.Extensions.Configuration.Binder` (v10.0.0) - Already present

---

## 🗄️ Database-Specific Implementations

### SQL Server vs PostgreSQL Mapping

| Feature | SQL Server | PostgreSQL |
|---------|-----------|------------|
| **Connection Class** | `SqlConnection` | `NpgsqlConnection` |
| **Command Class** | `SqlCommand` | `NpgsqlCommand` |
| **DataAdapter Class** | `SqlDataAdapter` | `NpgsqlDataAdapter` |
| **Parameter Prefix** | `@` | `@` or `$` |
| **String Concatenation** | `+` | `\|\|` |
| **TOP clause** | `SELECT TOP 10` | `SELECT ... LIMIT 10` |
| **Auto-increment** | `IDENTITY` | `SERIAL` / `SEQUENCE` |
| **GetSchema** | ✅ Supported | ❌ Custom implementation needed |
| **Information Schema** | `INFORMATION_SCHEMA` | `information_schema` (standard) |
| **System Tables** | `sys.tables`, `sys.columns` | `pg_tables`, `pg_attribute` |

---

## 📝 Configuration Changes

### appsettings.json Update

```json
{
  "DatabaseSettings": {
    "DatabaseType": "SqlServer",
    "ConnectionStrings": {
      "SqlServer": "Server=localhost;Database=netTV;User Id=sa;Password=sql.2014",
      "PostgreSQL": "Host=localhost;Database=nettv;Username=postgres;Password=postgres"
    }
  },
  "AppSettings": {
    // Additional configurations
  }
}
```

### Database Configuration Class

```csharp
/// <summary>
/// Database configuration settings
/// </summary>
public class DatabaseSettings
{
    public DatabaseType DatabaseType { get; set; }
    public Dictionary<string, string> ConnectionStrings { get; set; } = new();
}
```

---

## 🔄 Migration Strategy

### Phase 1: Foundation (Week 1)

**Tasks:**
1. Create `IDatabase` interface
2. Create `IDatabaseFactory` interface
3. Implement `DatabaseType` enum
4. Implement `DatabaseFactoryManager`
5. Add `Npgsql` NuGet package
6. Unit tests for factory pattern

**Deliverables:**
- Interface definitions
- Factory manager
- Basic unit tests

---

### Phase 2: SQL Server Implementation (Week 1)

**Tasks:**
1. Create `SqlServerFactory` class
2. Create `SqlServerDatabase` class implementing `IDatabase`
3. Refactor existing `DAConexion` to use `IDatabase`
4. Maintain backward compatibility
5. Test with existing SQL Server databases

**Deliverables:**
- Working SQL Server implementation
- Migration of existing code
- Test suite passing

---

### Phase 3: PostgreSQL Implementation (Week 2)

**Tasks:**
1. Create `PostgreSqlFactory` class
2. Create `PostgreSqlDatabase` class implementing `IDatabase`
3. Implement PostgreSQL-specific schema queries
4. Handle data type mapping between SQL Server and PostgreSQL
5. Test CRUD operations

**Deliverables:**
- Working PostgreSQL implementation
- Schema queries working
- CRUD operations tested

---

### Phase 4: Program.cs Updates (Week 2)

**Tasks:**
1. Update `Program.cs` to use `DatabaseFactoryManager`
2. Add database type selection from configuration
3. Support both databases in class generation
4. Generate database-specific code when needed
5. Handle database-specific syntax in generated code

**Deliverables:**
- Updated `Program.cs`
- Configuration loading
- Generated code works for both databases

---

### Phase 5: Testing & Documentation (Week 3)

**Tasks:**
1. Create integration tests for SQL Server
2. Create integration tests for PostgreSQL
3. Update README.md with PostgreSQL instructions
4. Create migration guide
5. Performance testing

**Deliverables:**
- Test suite for both databases
- Updated documentation
- Performance benchmarks

---

## 💻 Code Examples

### Example 1: Using the Factory Pattern

```csharp
/// <summary>
/// Example: Creating database connections using Factory Pattern
/// </summary>
public class ExampleUsage
{
    public void QueryData(DatabaseType databaseType, string connectionString)
    {
        // Create database using factory
        using var database = DatabaseFactoryManager.CreateDatabase(databaseType, connectionString);
        database.Open();

        // Execute query - works the same for both databases
        var result = database.ExecuteQuery("SELECT * FROM Customers");

        foreach (DataRow row in result.Rows)
        {
            Console.WriteLine($"Customer: {row["Name"]}");
        }
    }
}
```

### Example 2: Configuration-Based Selection

```csharp
/// <summary>
/// Example: Loading database from configuration
/// </summary>
public class ConfigurationExample
{
    public IDatabase CreateDatabaseFromConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var dbSettings = configuration.GetSection("DatabaseSettings")
            .Get<DatabaseSettings>();

        var connectionString = dbSettings.ConnectionStrings[dbSettings.DatabaseType.ToString()];

        return DatabaseFactoryManager.CreateDatabase(dbSettings.DatabaseType, connectionString);
    }
}
```

### Example 3: Updated DAConexion Class

```csharp
/// <summary>
/// Updated DAConexion using IDatabase interface
/// </summary>
public class DAConexion : IDisposable
{
    private readonly IDatabase _database;
    private bool _disposed;

    public DAConexion(DatabaseType databaseType, string connectionString)
    {
        _database = DatabaseFactoryManager.CreateDatabase(databaseType, connectionString);
        _database.Open();
    }

    // Default constructor - reads from configuration
    public DAConexion()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var dbSettings = configuration.GetSection("DatabaseSettings")
            .Get<DatabaseSettings>();

        var connectionString = dbSettings.ConnectionStrings[dbSettings.DatabaseType.ToString()];

        _database = DatabaseFactoryManager.CreateDatabase(dbSettings.DatabaseType, connectionString);
        _database.Open();
    }

    public DataTable ExecuteQuery(string cmdSql)
    {
        return _database.ExecuteQuery(cmdSql);
    }

    // ... rest of the implementation
}
```

---

## 🧪 Testing Strategy

### Unit Tests

```csharp
[TestClass]
public class DatabaseFactoryTests
{
    [TestMethod]
    public void CreateDatabase_SqlServer_ReturnsSqlServerDatabase()
    {
        // Arrange
        var connectionString = "Server=localhost;Database=test";

        // Act
        var database = DatabaseFactoryManager.CreateDatabase(DatabaseType.SqlServer, connectionString);

        // Assert
        Assert.IsInstanceOfType(database, typeof(SqlServerDatabase));
    }

    [TestMethod]
    public void CreateDatabase_PostgreSQL_ReturnsPostgreSQLDatabase()
    {
        // Arrange
        var connectionString = "Host=localhost;Database=test";

        // Act
        var database = DatabaseFactoryManager.CreateDatabase(DatabaseType.PostgreSQL, connectionString);

        // Assert
        Assert.IsInstanceOfType(database, typeof(PostgreSqlDatabase));
    }
}
```

### Integration Tests

```csharp
[TestClass]
public class DatabaseIntegrationTests
{
    [TestMethod]
    public void SqlServer_CRUD_Operations_Success()
    {
        // Test SQL Server CRUD operations
    }

    [TestMethod]
    public void PostgreSQL_CRUD_Operations_Success()
    {
        // Test PostgreSQL CRUD operations
    }

    [TestMethod]
    public void BothDatabases_SameSchema_ReturnsSameData()
    {
        // Verify both databases return consistent results
    }
}
```

---

## ⚠️ Breaking Changes & Considerations

### Breaking Changes

1. **DAConexion Constructor Change**
   - Old: `new DAConexion(string connectionString)`
   - New: `new DAConexion(DatabaseType databaseType, string connectionString)`
   - **Mitigation**: Keep old constructor for backward compatibility (reads from config)

2. **Generated Code Differences**
   - Database-specific attribute mappings may be needed
   - **Mitigation**: Add database type attribute to generated classes

3. **Connection String Format**
   - SQL Server and PostgreSQL use different formats
   - **Mitigation**: Support both in configuration

### Considerations

1. **Performance**
   - Factory pattern adds minimal overhead (one-time object creation)
   - Npgsql performance is comparable to SqlClient

2. **Feature Parity**
   - Some SQL Server features may not have direct PostgreSQL equivalents
   - Need to implement alternative approaches (e.g., GetSchema)

3. **Testing**
   - Need to test against both databases
   - Consider Docker containers for test databases

---

## 📊 Data Type Mapping

### SQL Server to PostgreSQL Type Mapping

| SQL Server | PostgreSQL | C# Type |
|------------|-----------|---------|
| `int` | `integer` | `int` |
| `bigint` | `bigint` | `long` |
| `varchar(n)` | `varchar(n)` | `string` |
| `nvarchar(n)` | `varchar(n)` | `string` |
| `datetime` | `timestamp` | `DateTime` |
| `datetime2` | `timestamp` | `DateTime` |
| `bit` | `boolean` | `bool` |
| `decimal` | `numeric` | `decimal` |
| `float` | `double precision` | `double` |
| `uniqueidentifier` | `uuid` | `Guid` |
| `varbinary` | `bytea` | `byte[]` |
| `text` | `text` | `string` |

---

## 🚀 Implementation Checklist

### Phase 1: Foundation
- [ ] Create `IDatabase` interface
- [ ] Create `IDatabaseFactory` interface
- [ ] Create `DatabaseType` enum
- [ ] Create `DatabaseFactoryManager`
- [ ] Create `DatabaseSettings` configuration class
- [ ] Add `Npgsql` NuGet package
- [ ] Write unit tests for factory pattern

### Phase 2: SQL Server Implementation
- [ ] Create `SqlServerFactory` class
- [ ] Create `SqlServerDatabase` class
- [ ] Implement all `IDatabase` members
- [ ] Refactor `DAConexion` to use `IDatabase`
- [ ] Maintain backward compatibility
- [ ] Test with existing SQL Server databases

### Phase 3: PostgreSQL Implementation
- [ ] Create `PostgreSqlFactory` class
- [ ] Create `PostgreSqlDatabase` class
- [ ] Implement all `IDatabase` members
- [ ] Implement PostgreSQL schema queries
- [ ] Handle data type mappings
- [ ] Test CRUD operations
- [ ] Test with PostgreSQL database

### Phase 4: Integration
- [ ] Update `Program.cs` for factory pattern
- [ ] Update `appsettings.json` configuration
- [ ] Update class generation code
- [ ] Test class generation for both databases
- [ ] Update generated code templates if needed

### Phase 5: Documentation & Testing
- [ ] Update README.md with PostgreSQL support
- [ ] Create migration guide
- [ ] Create integration tests
- [ ] Performance testing
- [ ] Code review and refinement

---

## 📖 References

### PostgreSQL Resources
- [Npgsql Documentation](https://www.npgsql.org/doc/)
- [PostgreSQL Type Mapping](https://www.npgsql.org/doc/types/index.html)
- [PostgreSQL SQL Syntax](https://www.postgresql.org/docs/current/sql-syntax.html)

### Design Patterns
- [Factory Pattern](https://refactoring.guru/design-patterns/factory-method)
- [Abstract Factory Pattern](https://refactoring.guru/design-patterns/abstract-factory)
- [Strategy Pattern](https://refactoring.guru/design-patterns/strategy)

---

## 📅 Timeline Estimate

- **Phase 1**: 2-3 days
- **Phase 2**: 2-3 days
- **Phase 3**: 3-4 days
- **Phase 4**: 2-3 days
- **Phase 5**: 2-3 days

**Total Estimate**: 2-3 weeks for complete implementation

---

## 🎯 Success Criteria

1. ✅ EntityGen works with both SQL Server and PostgreSQL
2. ✅ No breaking changes for existing SQL Server users
3. ✅ All CRUD operations work on both databases
4. ✅ Class generation works for both databases
5. ✅ Performance is comparable to current implementation
6. ✅ Code is maintainable and extensible
7. ✅ Documentation is complete and accurate

---

## 🔄 Future Enhancements

1. **MySQL Support** - Add MySQL using MySQL Connector/NET
2. **SQLite Support** - Add SQLite for local development/testing
3. **Connection Pooling** - Implement connection pooling for better performance
4. **Async Operations** - Add async versions of all database operations
5. **Bulk Operations** - Implement bulk insert/update for both databases
6. **Database Migrations** - Add automated migration support
7. **Schema Synchronization** - Tools to sync schemas between databases

---

**Document Version**: 1.0
**Date**: February 2025
**Status**: Draft - Ready for Review
