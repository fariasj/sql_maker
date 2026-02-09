# SqlV2 - SQL Server to C# Class Generator

A .NET code generation tool that reads SQL Server database schemas and automatically generates C# POCO classes with data access attributes.

## Features

- Connects to SQL Server databases
- Reads table schemas and column information
- Generates C# partial classes from database tables
- Automatically detects primary keys and identity columns
- Maps SQL Server data types to .NET types
- Generates custom attributes for ORM-like functionality
- Supports nullable types for appropriate columns

## Requirements

- .NET Framework 4.5 or higher
- Visual Studio 2015 or later (or MSBuild)
- SQL Server database access
- Newtonsoft.Json (currently referenced from external path - needs NuGet package)

## Configuration

Before running the application, update the following values in `Program.cs`:

```csharp
// Line 17-20
var nameSpace = "SqlV2";           // Namespace for generated classes
var cnxString = @"Server=localhost;Database=netTV;User Id=sa;Password=sql.2014";
var pathOfClass = @"E:\My Documents\...\PartialClass\";  // Output directory
```

### Alternative: App.config Connection String

You can also configure a named connection string in `App.config`:

```xml
<connectionStrings>
  <add name="cnxDefault" connectionString="Server=localhost;Database=YourDatabase;..." />
</connectionStrings>
```

## Building

### Using Visual Studio
1. Open `SqlV2.sln`
2. Fix the Newtonsoft.Json reference (install via NuGet)
3. Build solution (F6)

### Using MSBuild (command line)
```bash
msbuild SqlV2.sln /p:Configuration=Release
```

## Running

1. Ensure configuration is set correctly
2. Run the executable:
   - From Visual Studio: F5
   - From command line: `bin\Debug\SqlV2.exe`

The application will:
- Connect to the specified SQL Server database
- Iterate through all tables
- Generate a C# class file for each table in the `PartialClass` directory
- Display the generated code in the console

## Project Structure

```
SqlV2/
├── Program.cs              # Main entry point - class generation logic
├── DAConexion.cs           # SQL Server connection wrapper
├── DASqlBaseV3.cs          # Base class for generated entities
├── DAExtensions.cs         # Extension methods
├── DAUtileriasSistema.cs   # Utility functions
├── DAMensajesSistema.cs    # System messages
├── DAConstantes.cs         # Constants
├── Animal.cs               # Example/Animal class
├── PartialClass/           # Generated classes (output directory)
│   ├── Pagos.cs
│   └── Encuestas.cs
└── Class/                  # Additional classes
```

## Generated Class Example

For a table named `Pagos`, the tool generates:

```csharp
using System;

namespace SqlV2
{
    [DAClassAttributes(SqlType = DASqlType.Table, SqlSchema = "dbo")]
    public partial class Pagos : DASqlBaseV3<Pagos>
    {
        //*************************
        //Archivo generado automaticamente por una utilidad de Abraham Farías.
        //No modificar el archivo a mano.
        //Fecha generación: 08/02/2026 21:33:00
        //*************************

        public Pagos()
        { }

        [DAAttributes(IsIdentity = true, IsKeyForDelete = true,
                      IsKeyForUpdate = true, IsKeyForSelect = true,
                      IsSqlParameter = true, SqlColumnName = "PagoId")]
        public int PagoId { get; set; }

        [DAAttributes(IsNullable = true, IsSqlParameter = true,
                      SqlColumnName = "Monto")]
        public decimal? Monto { get; set; }
    }
}
```

## SQL Type to .NET Type Mapping

| SQL Type | .NET Type |
|----------|-----------|
| bigint | Int64 |
| int | int |
| smallint | Int16 |
| tinyint | Byte |
| bit | bool |
| decimal, money, numeric | decimal |
| float | double |
| real | Single |
| char, nchar, varchar, nvarchar, text, ntext | string |
| datetime, datetime2, smalldatetime | DateTime |
| date | DateTime |
| time | TimeSpan |
| uniqueidentifier | Guid |
| binary, varbinary, image, timestamp, rowversion | Byte[] |
| xml | XDocument |

## Known Issues

- **Newtonsoft.Json reference**: Currently points to an absolute path that won't work across machines. Fix by installing via NuGet:
  ```bash
  Install-Package Newtonsoft.Json
  ```

- **Hardcoded paths**: Connection strings and output paths are hardcoded in `Program.cs`

- **.NET Framework version**: Project uses .NET Framework 4.5. Consider upgrading to .NET 6/7/8 for modern features

## Database Stored Procedure

The application relies on `SP_Columns` stored procedure which returns column metadata for a given table and schema:

```sql
EXEC SP_Columns @TableName, @Schema
```

Ensure this procedure exists in your database or provide an alternative method to query column metadata.

## License

This project appears to be an internal utility. Please verify licensing before redistribution.

## Author

Generated by a utility of Abraham Farías.
