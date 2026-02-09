# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build and Run Commands

### Build
```bash
# From the SqlV2 directory
dotnet clean
dotnet restore
dotnet build --configuration Release
```

### Run
```bash
# From the SqlV2 directory
dotnet run
```

### Test database connectivity
```bash
# Run the application - it will attempt to connect to the configured database
# and generate C# classes from the database schema
dotnet run
```

## Architecture Overview

EntityGen is a code generation tool that reads database schemas (SQL Server or PostgreSQL) and generates C# POCO classes with data access attributes.

### Core Architecture: Factory Pattern for Multi-Database Support

The application uses an Abstract Factory pattern to support multiple database types:

```
DatabaseFactoryManager (central registry)
    ├── IDatabaseFactory interface
    ├── SqlServerFactory → SqlServerDatabase (Microsoft.Data.SqlClient)
    └── PostgreSqlFactory → PostgreSqlDatabase (Npgsql)
```

**Key Design Decision**: All database-specific implementations are hidden behind the `IDatabase` interface. The `DAConexion` wrapper maintains backward compatibility with legacy code while internally using the factory pattern.

### Database Abstraction Layer

- **IDatabase interface**: Defines all database operations (Open, Close, ExecuteQuery, GetSchema, transactions)
- **IDatabaseFactory interface**: Factory for creating database instances
- **DatabaseFactoryManager**: Static registry of all available database factories with `RegisterFactory()` for extensibility
- **DatabaseSettings**: Configuration class loaded from appsettings.json

### Configuration System

**IMPORTANT**: The project uses `appsettings.json` (not App.config). Configuration is loaded via Microsoft.Extensions.Configuration:

```json
{
  "DatabaseSettings": {
    "DatabaseType": "SqlServer",  // or "PostgreSQL"
    "ConnectionStrings": {
      "SqlServer": "...",
      "PostgreSQL": "..."
    }
  }
}
```

Three ways to instantiate DAConexion:
1. `new DAConexion()` - Reads from appsettings.json
2. `new DAConexion(connectionString)` - Defaults to SqlServer for backward compatibility
3. `new DAConexion(DatabaseType.PostgreSQL, connectionString)` - Explicit database type

### Schema Retrieval Differences

**SQL Server**: Uses native `SqlConnection.GetSchema(collectionName)` API
**PostgreSQL**: Does NOT support GetSchema. Custom queries are implemented in PostgreSqlDatabase:
- `QueryPostgreSqlTables()` - Queries information_schema.tables
- `QueryPostgreSqlColumns()` - Queries information_schema.columns
- `QueryPostgreSqlViews()` - Queries information_schema.views

All return DataTables with matching column names for consistency.

### Code Generation Flow

Program.cs orchestrates the generation:
1. Connects to database via DAConexion
2. Retrieves all tables via `GetSchema("Tables")`
3. For each table:
   - Retrieves primary keys from CONSTRAINT_COLUMN_USAGE
   - Retrieves column metadata via `SP_Columns` (SQL Server) or equivalent query
   - Maps SQL types to .NET types (Program.cs:GetNetType)
   - Generates partial class with attributes
   - Writes to PartialClass/ directory

### Type Mapping System

SQL-to-.NET type mapping is centralized in `Program.cs:GetNetType()` method. When adding support for new database types, extend this method.

### Key Implementation Details

- **Nullable handling**: Non-string types get `?` suffix when `IS_NULLABLE == "YES"`
- **Primary key detection**: Determined by querying CONSTRAINT_COLUMN_USAGE for PRIMARY KEY constraints
- **Namespace**: All generated classes use `EntityGen` namespace
- **Base class**: Generated classes inherit from `DASqlBaseV3<T>`
- **Attributes**: Custom `[DAAttributes]` mark ORM-like properties (IsIdentity, IsKeyForDelete, etc.)

### Package Dependencies

- **Microsoft.Data.SqlClient** v6.0.0 - SQL Server ADO.NET provider
- **Npgsql** v8.0.3 - PostgreSQL ADO.NET provider
- **Microsoft.Extensions.Configuration*** v10.0.0 - Configuration system

### Extending to New Database Types

To add support for a new database (e.g., MySQL):

1. Add enum value to `DatabaseType.cs`
2. Create `MySqlDatabase.cs` implementing `IDatabase`
3. Create `MySqlFactory.cs` implementing `IDatabaseFactory`
4. Register in `DatabaseFactoryManager` static constructor
5. Add NuGet package (e.g., MySqlConnector)
6. Implement custom schema queries if database doesn't support GetSchema
7. Update `Program.cs:GetNetType()` for MySQL-specific type mappings
8. Add connection string to appsettings.json

### Known Limitations

- **SP_Columns dependency**: Program.cs uses SQL Server `SP_Columns` stored procedure. PostgreSQL users must create equivalent procedure or modify Program.cs to use standard queries.
- **Hardcoded paths**: Some paths in legacy code may still reference Windows-style absolute paths
- **Generated file location**: Classes are written to PartialClass/ directory (configured in Program.cs)
