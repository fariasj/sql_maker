# SqlOrm - SQL Server Class Generator for .NET 10

**Generador de clases C# desde SQL Server para .NET 10 LTS**

Este proyecto genera automáticamente clases de C# a partir de las tablas de SQL Server, simplificando el desarrollo de la capa de acceso a datos.

---

## 📋 Tabla de Contenidos

- [Características](#características)
- [Requisitos](#requisitos)
- [Instalación](#instalación)
- [Configuración](#configuración)
- [Uso](#uso)
- [Migración desde .NET Framework 4.5](#migración-desde-net-framework-45)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Ejemplos de Código](#ejemplos-de-código)

---

## ✨ Características

- ✅ **Generación Automática**: Crea clases parciales desde tablas de SQL Server
- ✅ **CRUD Integrado**: Métodos preconfigurados para Alta, Baja, Cambio y Consulta
- ✅ **Cross-Platform**: Compatible con Linux, macOS y Windows
- ✅ **.NET 10 LTS**: Última versión LTS de .NET
- ✅ **Modern C#:** Tipos de referencia anulables y namespaces con ámbito de archivo
- ✅ **Serialización JSON**: Integración con System.Text.Json
- ✅ **Transacciones**: Soporte completo para transacciones de base de datos

---

## 📦 Requisitos

### Necesario

- **.NET 10 SDK** - [Descargar aquí](https://dotnet.microsoft.com/download/dotnet/10.0)
- **SQL Server** - Cualquier versión compatible con ADO.NET
- **IDE Recomendado**: Visual Studio 2022, Visual Studio Code, or JetBrains Rider

### Opcional

- Git para control de versiones

---

## 🚀 Instalación

### 1. Clonar el Repositorio

```bash
git clone <repository-url>
cd sql_maker/SqlV2
```

### 2. Restaurar Dependencias

```bash
dotnet restore
```

### 3. Compilar el Proyecto

```bash
dotnet build --configuration Release
```

---

## ⚙️ Configuración

### Archivo de Configuración: `appsettings.json`

El proyecto utiliza `appsettings.json` para la configuración (reemplaza el antiguo `App.config`):

```json
{
  "ConnectionStrings": {
    "cnxDefault": "Server=localhost;Database=netTV;User Id=sa;Password=sql.2014"
  },
  "AppSettings": {
    // Configuraciones adicionales según necesites
  }
}
```

### Cambiar la Cadena de Conexión

Edita `appsettings.json` y modifica la sección `ConnectionStrings`:

```json
{
  "ConnectionStrings": {
    "cnxDefault": "Server=tu-servidor;Database=tu-base-de-datos;User Id=tu-usuario;Password=tu-contraseña"
  }
}
```

---

## 🎯 Uso

### Generar Clases desde SQL Server

El programa `Program.cs` genera automáticamente clases parciales:

```bash
dotnet run
```

Esto generará archivos en el directorio `PartialClass/` con formato:

```csharp
namespace SqlOrm;

[DAClassAttributes(SqlType = DASqlType.Table)]
public partial class NombreTabla : DASqlBaseV3<NombreTabla>
{
    [DAAttributes(IsKeyForDelete = true, IsIdentity = true, IsKeyForUpdate = true, IsKeyForSelect = true, IsSqlParameter = true, SqlColumnName = "id")]
    public int Id { get; set; }

    [DAAttributes(IsSqlParameter = true, SqlColumnName = "Nombre")]
    public string Nombre { get; set; } = string.Empty;
}
```

---

## 🔄 Migración desde .NET Framework 4.5

Este proyecto fue migrado exitosamente de .NET Framework 4.5 a .NET 10 LTS.

### Cambios Principales

#### ✅ Actualizaciones de APIs

| Antes (.NET Framework 4.5) | Después (.NET 10) |
|----------------------------|-------------------|
| `System.Data.SqlClient` | `Microsoft.Data.SqlClient` |
| `System.Configuration` | `Microsoft.Extensions.Configuration` |
| `App.config` | `appsettings.json` |
| `Newtonsoft.Json` | `System.Text.Json` |
| `System.Runtime.Serialization.Formatters.Binary` | `System.Text.Json` |
| `WindowsIdentity.GetCurrent().Name` | `Environment.UserName` |

#### ✅ Modernización del Código

- **Namespaces con ámbito de archivo** (file-scoped namespaces)
- **Tipos de referencia anulables** habilitados (`<Nullable>enable</Nullable>`)
- **Using statements implícitos** (`<ImplicitUsings>enable</ImplicitUsings>`)
- **Formato de proyecto SDK-style**

#### ✅ Características Cross-Platform

- ✅ Eliminadas dependencias de Windows-only
- ✅ Compatible con Linux, macOS y Windows
- ✅ Sin dependencias de System.Web

### Breaking Changes

⚠️ **BinaryFormatter**: Si tenías datos serializados con `BinaryFormatter`, necesitarás migrarlos antes de usar esta versión. El formato de serialización cambió a JSON.

---

## 📁 Estructura del Proyecto

```
SqlV2/
├── Class/                    # Clases parciales personalizadas
│   ├── Pagos.cs
│   └── ...
├── PartialClass/             # Clases generadas automáticamente
│   ├── Encuestas.cs
│   ├── Respuestas.cs
│   └── ...
├── Properties/               # Configuración del proyecto
├── DAConexion.cs             # Gestión de conexiones SQL
├── DAExtensions.cs           # Métodos de extensión
├── DAUtileriasSistema.cs     # Utilidades del sistema
├── DASqlBaseV3.cs           # Base para operaciones CRUD
├── DAMensajesSistema.cs      # Sistema de mensajes
├── DAConstantes.cs          # Constantes y atributos
├── Program.cs                # Punto de entrada
├── SqlV2.csproj             # Archivo de proyecto
├── appsettings.json         # Configuración
└── README.md                # Este archivo
```

---

## 💡 Ejemplos de Código

### 1. Conexión a la Base de Datos

```csharp
using (var cnx = new DAConexion())
{
    // La conexión se abre automáticamente
    var resultado = cnx.ExecuteQuery("SELECT * FROM Tabla");

    foreach (DataRow row in resultado.Rows)
    {
        Console.WriteLine(row["Columna"]);
    }
}
```

### 2. Insertar un Registro (Alta)

```csharp
using (var cnx = new DAConexion())
{
    var nuevoRegistro = new Pagos
    {
        IdCliente = 123,
        IdLista = 456,
        Fecha = DateTime.Now,
        Flag = 1
    };

    if (nuevoRegistro.Guardar(cnx))
    {
        Console.WriteLine($"Registro guardado con ID: {nuevoRegistro.Id}");
    }
}
```

### 3. Consultar un Registro

```csharp
using (var cnx = new DAConexion())
{
    var pago = new Pagos { Id = 1 };

    if (pago.Consultar(cnx))
    {
        Console.WriteLine($"Cliente: {pago.IdCliente}");
        Console.WriteLine($"Fecha: {pago.Fecha}");
    }
}
```

### 4. Actualizar un Registro

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

### 5. Eliminar un Registro

```csharp
using (var cnx = new DAConexion())
{
    var pago = new Pagos { Id = 1 };

    if (pago.Borrar(cnx))
    {
        Console.WriteLine("Registro eliminado");
    }
}
```

### 6. Consultar Múltiples Registros

```csharp
using (var cnx = new DAConexion())
{
    var encuesta = new Encuestas();
    var lista = encuesta.ConsultarColeccion(cnx);

    foreach (var item in lista)
    {
        Console.WriteLine($"Encuesta: {item.Encuesta}");
    }
}
```

### 7. Uso de Transacciones

```csharp
using (var cnx = new DAConexion())
{
    // La transacción se maneja automáticamente en Guardar/Modificar/Borrar
    var pago = new Pagos
    {
        IdCliente = 123,
        IdLista = 456,
        Fecha = DateTime.Now,
        Flag = 1
    };

    if (pago.Guardar(cnx))
    {
        Console.WriteLine("Guardado exitosamente con transacción");
    }
    // Si ocurre error, hace rollback automáticamente
}
```

---

## 🔧 Compilación y Ejecución

### Modo Debug

```bash
dotnet build
dotnet run
```

### Modo Release

```bash
dotnet build --configuration Release
dotnet run --configuration Release
```

### Publicar como Ejecutable

```bash
# Windows
dotnet publish -c Release -r win-x64 --self-contained

# Linux
dotnet publish -c Release -r linux-x64 --self-contained

# macOS
dotnet publish -c Release -r osx-x64 --self-contained
```

---

## 🛠️ Solución de Problemas

### Error: "Connection string 'cnxDefault' not found"

**Solución**: Verifica que `appsettings.json` exista y tenga la conexión configurada:

```json
{
  "ConnectionStrings": {
    "cnxDefault": "tu-cadena-de-conexion"
  }
}
```

### Error: "Cannot connect to SQL Server"

**Solución**: Verifica:
1. SQL Server está ejecutándose
2. La cadena de conexión es correcta
3. El firewall permite conexiones al puerto 1433

### Warnings de Nullable Reference Types

**Nota**: Estos warnings son normales al habilitar tipos de referencia anulables en código existente. Se irán resolviendo gradualmente.

---

## 📚 Referencias y Recursos

- [Documentación de .NET 10](https://docs.microsoft.com/dotnet/core/)
- [Microsoft.Data.SqlClient](https://docs.microsoft.com/sql/connect/ado-net/introduction-microsoft-data-sqlclient-namespace)
- [System.Text.Json](https://docs.microsoft.com/dotnet/standard/serialization/system-text-json-overview)
- [Tipos de referencia anulables](https://docs.microsoft.com/dotnet/csharp/nullable-references)

---

## 📝 Notas de Versión

### Versión 2.0 - Migración a .NET 10 LTS

**Fecha**: Febrero 2025

**Cambios**:
- ✅ Migrado de .NET Framework 4.5 a .NET 10 LTS
- ✅ Proyecto convertido a formato SDK-style
- ✅ Reemplazadas APIs obsoletas
- ✅ Soporte multiplataforma completo
- ✅ Namespace actualizado: `SqlV2` → `SqlOrm`
- ✅ Modernización con características de C# 10

---

## 👥 Autores

- **Abraham Farías** - Autor original de las utilidades de generación de clases

---

## 🤝 Contribuciones

Las contribuciones son bienvenidas. Por favor:
1. Fork el proyecto
2. Crea una rama para tu feature
3. Commit tus cambios
4. Push a la rama
5. Abre un Pull Request

---

## 📧 Contacto

Para preguntas o soporte, por favor abre un issue en el repositorio.

---

**SqlOrm** - Simplificando el desarrollo de la capa de datos en .NET 10
