using System.Diagnostics;
using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Configuration;

namespace EntityGen;

public delegate void DAConexionEventHandler(object sender, DAConexionEventArgs e);

public class DAConexionEventArgs : EventArgs
{
    public string Cmd { get; set; } = string.Empty;
    public IDbConnection? ConnectionString { get; set; }

    public DAConexionEventArgs(string sqlcmd, IDbConnection? dbConnection)
    {
        StackTrace trace = new StackTrace();
        StackFrame frame = new StackFrame(2);
        var method = frame.GetMethod();

        Cmd = "";

        if (method?.DeclaringType?.Name != "DAConexion")
        {
            Cmd += DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + "|" + DAConstantes.CurrentUser + "|" + method?.DeclaringType + " - " + method?.Name + "\t| " + sqlcmd;
        }

        ConnectionString = dbConnection;
    }
}

/// <summary>
/// Database connection wrapper using Factory Pattern
/// Supports both SQL Server and PostgreSQL through IDatabase interface
/// </summary>
public class DAConexion : IDisposable
{
    private void Cnx_OnExecuteQuery(object sender, DAConexionEventArgs e)
    {
        Debug.WriteLineIf(Debugger.IsAttached && !e.Cmd.IsNullOrEmpty(), e.Cmd);
    }

    private readonly IDatabase _database;
    private bool disposed = false;

    public string InfoMessage
    {
        get => _database.InfoMessage;
        set
        {
            _database.InfoMessage = value;
        }
    }

    public event DAConexionEventHandler? OnExecuteQuery;

    /// <summary>
    /// Gets the underlying database connection (for backward compatibility)
    /// </summary>
    public IDbConnection? Connection => _database.Connection;

    /// <summary>
    /// Default constructor - reads database type and connection string from appsettings.json
    /// </summary>
    public DAConexion()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var dbSettings = configuration.GetSection("DatabaseSettings")
            .Get<DatabaseSettings>();

        if (dbSettings == null)
        {
            throw new Exception("DatabaseSettings section not found in appsettings.json");
        }

        var connectionString = dbSettings.GetConnectionString();

        _database = DatabaseFactoryManager.CreateDatabase(dbSettings.DatabaseType, connectionString);
        _database.Open();

        OnExecuteQuery += Cnx_OnExecuteQuery;
    }

    /// <summary>
    /// Constructor with explicit connection string (defaults to SQL Server for backward compatibility)
    /// </summary>
    /// <param name="connectionString">The connection string</param>
    public DAConexion(string connectionString)
    {
        // Default to SQL Server for backward compatibility
        _database = DatabaseFactoryManager.CreateDatabase(DatabaseType.SqlServer, connectionString);
        _database.Open();

        OnExecuteQuery += Cnx_OnExecuteQuery;
    }

    /// <summary>
    /// Constructor with explicit database type and connection string
    /// </summary>
    /// <param name="databaseType">The type of database (SqlServer or PostgreSQL)</param>
    /// <param name="connectionString">The connection string</param>
    public DAConexion(DatabaseType databaseType, string connectionString)
    {
        _database = DatabaseFactoryManager.CreateDatabase(databaseType, connectionString);
        _database.Open();

        OnExecuteQuery += Cnx_OnExecuteQuery;
    }

    /// <summary>
    /// Begins a database transaction
    /// </summary>
    public bool BeginTransaction()
    {
        if (_database is SqlServerDatabase sqlServerDb)
        {
            sqlServerDb.BeginTransaction();
            return true;
        }
        else if (_database is PostgreSqlDatabase postgreSqlDb)
        {
            postgreSqlDb.BeginTransaction();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Commits the current transaction
    /// </summary>
    public void CommitTransaction()
    {
        if (_database is SqlServerDatabase sqlServerDb)
        {
            sqlServerDb.CommitTransaction();
        }
        else if (_database is PostgreSqlDatabase postgreSqlDb)
        {
            postgreSqlDb.CommitTransaction();
        }
    }

    /// <summary>
    /// Rolls back the current transaction
    /// </summary>
    public void RollbackTransaction()
    {
        if (_database is SqlServerDatabase sqlServerDb)
        {
            sqlServerDb.RollbackTransaction();
        }
        else if (_database is PostgreSqlDatabase postgreSqlDb)
        {
            postgreSqlDb.RollbackTransaction();
        }
    }

    /// <summary>
    /// Executes a query and returns the first column of the first row
    /// </summary>
    public string ExecuteScalar(string cmdSql)
    {
        OnExecuteQuery?.Invoke(this, new DAConexionEventArgs(cmdSql, _database.Connection));

        return _database.ExecuteScalar(cmdSql)?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Executes a query and returns the first row
    /// </summary>
    public DataRow ExecuteAndGetFirstRow(string cmdSql)
    {
        OnExecuteQuery?.Invoke(this, new DAConexionEventArgs(cmdSql, _database.Connection));

        return ExecuteQuery(cmdSql).Rows[0];
    }

    /// <summary>
    /// Executes a query and returns a DataSet
    /// </summary>
    public DataSet ExecuteQueryDataSet(string cmdSql)
    {
        OnExecuteQuery?.Invoke(this, new DAConexionEventArgs(cmdSql, _database.Connection));

        DataSet dtTable = new DataSet();
        dtTable.Clear();

        try
        {
            var resultTable = _database.ExecuteQuery(cmdSql);
            dtTable.Tables.Add(resultTable);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message + Environment.NewLine);
        }

        return dtTable;
    }

    /// <summary>
    /// Executes a query and returns a DataTable
    /// </summary>
    public DataTable ExecuteQuery(string cmdSql)
    {
        OnExecuteQuery?.Invoke(this, new DAConexionEventArgs(cmdSql, _database.Connection));

        return ExecuteQuery(cmdSql, false);
    }

    /// <summary>
    /// Executes a query and returns a DataTable with optional empty data check
    /// </summary>
    public DataTable ExecuteQuery(string cmdSql, bool throwEmptyData)
    {
        OnExecuteQuery?.Invoke(this, new DAConexionEventArgs(cmdSql, _database.Connection));

        DataTable dtTable = new DataTable();

        try
        {
            dtTable = _database.ExecuteQuery(cmdSql);

            if (throwEmptyData)
            {
                if (dtTable.Rows.Count <= 0)
                {
                    throw new Exception("No data found");
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message + Environment.NewLine);
        }

        return dtTable;
    }

    /// <summary>
    /// Executes a non-query SQL command (INSERT, UPDATE, DELETE)
    /// </summary>
    public int ExecuteNonQuery(string cmdSql)
    {
        OnExecuteQuery?.Invoke(this, new DAConexionEventArgs(cmdSql, _database.Connection));

        try
        {
            return _database.ExecuteNonQuery(cmdSql);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message + Environment.NewLine);
        }
    }

    /// <summary>
    /// Gets schema information from the database
    /// </summary>
    public DataTable GetSchema(string collectionName)
    {
        return _database.GetSchema(collectionName);
    }

    /// <summary>
    /// Exposes the underlying IDatabase for advanced scenarios
    /// </summary>
    public IDatabase Database => _database;

    #region IDisposable Members

    ~DAConexion()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _database?.Dispose();
            }

            disposed = true;
        }
    }

    #endregion
}
