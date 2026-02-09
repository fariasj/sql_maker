# SqlOrm - SQL Server Class Generator for .NET 10

**C# Class Generator from SQL Server for .NET 10 LTS**

This project automatically generates C# classes from SQL Server tables, simplifying data access layer development.

---

## 📋 Table of Contents

- [Features](#features)
- [Requirements](#requirements)
- [Installation](#installation)
- [Configuration](#configuration)
- [Usage](#usage)
- [Migration from .NET Framework 4.5](#migration-from-net-framework-45)
- [Project Structure](#project-structure)
- [Code Examples](#code-examples)

---

## ✨ Features

- ✅ **Automatic Generation**: Creates partial classes from SQL Server tables
- ✅ **Integrated CRUD**: Preconfigured methods for Create, Read, Update, and Delete operations
- ✅ **Cross-Platform**: Compatible with Linux, macOS, and Windows
- ✅ **.NET 10 LTS**: Latest LTS version of .NET
- ✅ **Modern C#:** Nullable reference types and file-scoped namespaces
- ✅ **JSON Serialization**: Integration with System.Text.Json
- ✅ **Transactions**: Full support for database transactions

---

## 📦 Requirements

### Required

- **.NET 10 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/10.0)
- **SQL Server** - Any version compatible with ADO.NET
- **Recommended IDE**: Visual Studio 2022, Visual Studio Code, or JetBrains Rider

### Optional

- Git for version control

---

## 🚀 Installation

### 1. Clone the Repository

```bash
git clone <repository-url>
cd sql_maker/SqlV2
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Build the Project

```bash
dotnet build --configuration Release
```

---

## ⚙️ Configuration

### Configuration File: `appsettings.json`

The project uses `appsettings.json` for configuration (replaces the old `App.config`):

```json
{
  "ConnectionStrings": {
    "cnxDefault": "Server=localhost;Database=netTV;User Id=sa;Password=sql.2014"
  },
  "AppSettings": {
    // Additional configurations as needed
  }
}
```

### Changing the Connection String

Edit `appsettings.json` and modify the `ConnectionStrings` section:

```json
{
  "ConnectionStrings": {
    "cnxDefault": "Server=your-server;Database=your-database;User Id=your-user;Password=your-password"
  }
}
```

---

## 🎯 Usage

### Generating Classes from SQL Server

The `Program.cs` program automatically generates partial classes:

```bash
dotnet run
```

This will generate files in the `PartialClass/` directory with the format:

```csharp
namespace SqlOrm;

[DAClassAttributes(SqlType = DASqlType.Table)]
public partial class TableName : DASqlBaseV3<TableName>
{
    [DAAttributes(IsKeyForDelete = true, IsIdentity = true, IsKeyForUpdate = true, IsKeyForSelect = true, IsSqlParameter = true, SqlColumnName = "id")]
    public int Id { get; set; }

    [DAAttributes(IsSqlParameter = true, SqlColumnName = "Name")]
    public string Name { get; set; } = string.Empty;
}
```

---

## 🔄 Migration from .NET Framework 4.5

This project was successfully migrated from .NET Framework 4.5 to .NET 10 LTS.

### Key Changes

#### ✅ API Updates

| Before (.NET Framework 4.5) | After (.NET 10) |
|----------------------------|-------------------|
| `System.Data.SqlClient` | `Microsoft.Data.SqlClient` |
| `System.Configuration` | `Microsoft.Extensions.Configuration` |
| `App.config` | `appsettings.json` |
| `Newtonsoft.Json` | `System.Text.Json` |
| `System.Runtime.Serialization.Formatters.Binary` | `System.Text.Json` |
| `WindowsIdentity.GetCurrent().Name` | `Environment.UserName` |

#### ✅ Code Modernization

- **File-scoped namespaces**
- **Nullable reference types** enabled (`<Nullable>enable</Nullable>`)
- **Implicit using statements** (`<ImplicitUsings>enable</ImplicitUsings>`)
- **SDK-style project format**

#### ✅ Cross-Platform Features

- ✅ Removed Windows-only dependencies
- ✅ Compatible with Linux, macOS, and Windows
- ✅ No System.Web dependencies

### Breaking Changes

⚠️ **BinaryFormatter**: If you have data serialized with `BinaryFormatter`, you will need to migrate it before using this version. The serialization format changed to JSON.

---

## 📁 Project Structure

```
SqlV2/
├── Class/                    # Custom partial classes
│   ├── Pagos.cs
│   └── ...
├── PartialClass/             # Auto-generated classes
│   ├── Encuestas.cs
│   ├── Respuestas.cs
│   └── ...
├── Properties/               # Project configuration
├── DAConexion.cs             # SQL connection management
├── DAExtensions.cs           # Extension methods
├── DAUtileriasSistema.cs     # System utilities
├── DASqlBaseV3.cs           # Base for CRUD operations
├── DAMensajesSistema.cs      # Messaging system
├── DAConstantes.cs          # Constants and attributes
├── Program.cs                # Entry point
├── SqlV2.csproj             # Project file
├── appsettings.json         # Configuration
└── README.md                # This file
```

---

## 💡 Code Examples

### 1. Database Connection

```csharp
using (var cnx = new DAConexion())
{
    // Connection opens automatically
    var result = cnx.ExecuteQuery("SELECT * FROM Table");

    foreach (DataRow row in result.Rows)
    {
        Console.WriteLine(row["Column"]);
    }
}
```

### 2. Insert a Record (Create)

```csharp
using (var cnx = new DAConexion())
{
    var newRecord = new Pagos
    {
        IdCliente = 123,
        IdLista = 456,
        Fecha = DateTime.Now,
        Flag = 1
    };

    if (newRecord.Guardar(cnx))
    {
        Console.WriteLine($"Record saved with ID: {newRecord.Id}");
    }
}
```

### 3. Query a Record

```csharp
using (var cnx = new DAConexion())
{
    var pago = new Pagos { Id = 1 };

    if (pago.Consultar(cnx))
    {
        Console.WriteLine($"Client: {pago.IdCliente}");
        Console.WriteLine($"Date: {pago.Fecha}");
    }
}
```

### 4. Update a Record

```csharp
using (var cnx = new DAConexion())
{
    var pago = new Pagos { Id = 1 };

    if (pago.Consultar(cnx))
    {
        pago.Fecha = DateTime.Now;
        pago.Modificar(cnx);
    }
}
```

### 5. Delete a Record

```csharp
using (var cnx = new DAConexion())
{
    var pago = new Pagos { Id = 1 };

    if (pago.Borrar(cnx))
    {
        Console.WriteLine("Record deleted");
    }
}
```

### 6. Query Multiple Records

```csharp
using (var cnx = new DAConexion())
{
    var encuesta = new Encuestas();
    var lista = encuesta.ConsultarColeccion(cnx);

    foreach (var item in lista)
    {
        Console.WriteLine($"Survey: {item.Encuesta}");
    }
}
```

### 7. Using Transactions

```csharp
using (var cnx = new DAConexion())
{
    // Transactions are handled automatically in Guardar/Modificar/Borrar
    var pago = new Pagos
    {
        IdCliente = 123,
        IdLista = 456,
        Fecha = DateTime.Now,
        Flag = 1
    };

    if (pago.Guardar(cnx))
    {
        Console.WriteLine("Successfully saved with transaction");
    }
    // Automatically rolls back on error
}
```

---

## 🔧 Build and Run

### Debug Mode

```bash
dotnet build
dotnet run
```

### Release Mode

```bash
dotnet build --configuration Release
dotnet run --configuration Release
```

### Publish as Executable

```bash
# Windows
dotnet publish -c Release -r win-x64 --self-contained

# Linux
dotnet publish -c Release -r linux-x64 --self-contained

# macOS
dotnet publish -c Release -r osx-x64 --self-contained
```

---

## 🛠️ Troubleshooting

### Error: "Connection string 'cnxDefault' not found"

**Solution**: Verify that `appsettings.json` exists and has the connection configured:

```json
{
  "ConnectionStrings": {
    "cnxDefault": "your-connection-string"
  }
}
```

### Error: "Cannot connect to SQL Server"

**Solution**: Verify:
1. SQL Server is running
2. The connection string is correct
3. The firewall allows connections on port 1433

### Nullable Reference Types Warnings

**Note**: These warnings are normal when enabling nullable reference types in existing code. They will be resolved gradually.

---

## 📚 References and Resources

- [.NET 10 Documentation](https://docs.microsoft.com/dotnet/core/)
- [Microsoft.Data.SqlClient](https://docs.microsoft.com/sql/connect/ado-net/introduction-microsoft-data-sqlclient-namespace)
- [System.Text.Json](https://docs.microsoft.com/dotnet/standard/serialization/system-text-json-overview)
- [Nullable Reference Types](https://docs.microsoft.com/dotnet/csharp/nullable-references)

---

## 📝 Release Notes

### Version 2.0 - Migration to .NET 10 LTS

**Date**: February 2025

**Changes**:
- ✅ Migrated from .NET Framework 4.5 to .NET 10 LTS
- ✅ Project converted to SDK-style format
- ✅ Replaced obsolete APIs
- ✅ Full cross-platform support
- ✅ Namespace updated: `SqlV2` → `SqlOrm`
- ✅ Modernization with C# 10 features

---

## 👥 Authors

- **Abraham Farías** - Original author of class generation utilities

---

## 🤝 Contributing

Contributions are welcome. Please:
1. Fork the project
2. Create a branch for your feature
3. Commit your changes
4. Push to the branch
5. Open a Pull Request

---

## 📧 Contact

For questions or support, please open an issue in the repository.

---

**SqlOrm** - Simplifying data access layer development in .NET 10
