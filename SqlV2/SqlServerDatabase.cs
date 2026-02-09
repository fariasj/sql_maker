using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace EntityGen;

/// <summary>
/// SQL Server implementation of IDatabase
/// Wraps Microsoft.Data.SqlClient functionality
/// </summary>
public class SqlServerDatabase : IDatabase
{
    private SqlConnection? _connection;
    private SqlTransaction? _transaction;
    private bool _disposed;

    /// <summary>
    /// Gets the connection string
    /// </summary>
    public string ConnectionString { get; }

    /// <summary>
    /// Gets the underlying SQL Server connection
    /// </summary>
    public IDbConnection Connection => _connection ?? throw new InvalidOperationException("Connection not initialized");

    /// <summary>
    /// Gets or sets informational messages from the database
    /// </summary>
    public string InfoMessage { get; set; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the SqlServerDatabase class
    /// </summary>
    /// <param name="connectionString">The SQL Server connection string</param>
    public SqlServerDatabase(string connectionString)
    {
        ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    /// <summary>
    /// Opens the database connection
    /// </summary>
    public void Open()
    {
        _connection = new SqlConnection(ConnectionString);
        _connection.InfoMessage += OnInfoMessage;
        _connection.Open();
    }

    /// <summary>
    /// Closes the database connection
    /// </summary>
    public void Close()
    {
        if (_connection != null)
        {
            _connection.Close();
        }
    }

    /// <summary>
    /// Creates a SQL command with the specified text
    /// </summary>
    public IDbCommand CreateCommand(string commandText)
    {
        var command = _connection?.CreateCommand();
        if (command != null)
        {
            command.CommandText = commandText;
            command.Transaction = _transaction;
            command.CommandType = CommandType.Text;
            command.CommandTimeout = 50;
        }
        return command ?? throw new InvalidOperationException("Cannot create command: connection is not initialized");
    }

    /// <summary>
    /// Executes a query and returns a data reader
    /// </summary>
    public IDataReader ExecuteReader(string commandText)
    {
        var command = CreateCommand(commandText);
        return command.ExecuteReader();
    }

    /// <summary>
    /// Executes a non-query command
    /// </summary>
    public int ExecuteNonQuery(string commandText)
    {
        var command = CreateCommand(commandText);
        return command.ExecuteNonQuery();
    }

    /// <summary>
    /// Executes a query and returns the first value
    /// </summary>
    public object ExecuteScalar(string commandText)
    {
        var command = CreateCommand(commandText);
        return command.ExecuteScalar() ?? string.Empty;
    }

    /// <summary>
    /// Executes a query and returns a DataTable
    /// </summary>
    public DataTable ExecuteQuery(string commandText)
    {
        var command = CreateCommand(commandText);
        var adapter = new SqlDataAdapter((SqlCommand)command);
        var table = new DataTable();
        adapter.Fill(table);
        return table;
    }

    /// <summary>
    /// Executes a query and returns a DataSet
    /// </summary>
    public DataSet ExecuteQueryDataSet(string commandText)
    {
        var command = CreateCommand(commandText);
        var adapter = new SqlDataAdapter((SqlCommand)command);
        var dataSet = new DataSet();
        adapter.Fill(dataSet);
        return dataSet;
    }

    /// <summary>
    /// Retrieves schema information from SQL Server
    /// </summary>
    public DataTable GetSchema(string collectionName)
    {
        return _connection?.GetSchema(collectionName) ?? new DataTable();
    }

    /// <summary>
    /// Begins a database transaction
    /// </summary>
    public void BeginTransaction()
    {
        if (_transaction == null && _connection != null)
        {
            _transaction = _connection.BeginTransaction();
        }
    }

    /// <summary>
    /// Commits the current transaction
    /// </summary>
    public void CommitTransaction()
    {
        if (_transaction != null)
        {
            _transaction.Commit();
            _transaction.Dispose();
            _transaction = null;
        }
    }

    /// <summary>
    /// Rolls back the current transaction
    /// </summary>
    public void RollbackTransaction()
    {
        if (_transaction != null)
        {
            _transaction.Rollback();
            _transaction.Dispose();
            _transaction = null;
        }
    }

    /// <summary>
    /// Handles InfoMessage events from SQL Server
    /// </summary>
    private void OnInfoMessage(object? sender, SqlInfoMessageEventArgs e)
    {
        InfoMessage = e.Message + "\n" + e.Source + "\n" + e.Errors;
    }

    /// <summary>
    /// Disposes the database connection
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes resources
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _connection?.Dispose();
            }
            _disposed = true;
        }
    }
}
