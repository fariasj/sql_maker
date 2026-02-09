using System.Data;
using System.Data.Common;
using Npgsql;

namespace EntityGen;

/// <summary>
/// PostgreSQL implementation of IDatabase
/// Wraps Npgsql functionality for PostgreSQL databases
/// </summary>
public class PostgreSqlDatabase : IDatabase
{
    private NpgsqlConnection? _connection;
    private NpgsqlTransaction? _transaction;
    private bool _disposed;

    /// <summary>
    /// Gets the connection string
    /// </summary>
    public string ConnectionString { get; }

    /// <summary>
    /// Gets the underlying PostgreSQL connection
    /// </summary>
    public IDbConnection Connection => _connection ?? throw new InvalidOperationException("Connection not initialized");

    /// <summary>
    /// Gets informational messages from the database
    /// </summary>
    public string InfoMessage { get; private set; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the PostgreSqlDatabase class
    /// </summary>
    /// <param name="connectionString">The PostgreSQL connection string</param>
    public PostgreSqlDatabase(string connectionString)
    {
        ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    /// <summary>
    /// Opens the database connection
    /// </summary>
    public void Open()
    {
        _connection = new NpgsqlConnection(ConnectionString);
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
    /// Creates a PostgreSQL command with the specified text
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
        var adapter = new NpgsqlDataAdapter((NpgsqlCommand)command);
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
        var adapter = new NpgsqlDataAdapter((NpgsqlCommand)command);
        var dataSet = new DataSet();
        adapter.Fill(dataSet);
        return dataSet;
    }

    /// <summary>
    /// Retrieves schema information from PostgreSQL
    /// PostgreSQL doesn't support GetSchema like SQL Server, so we implement custom queries
    /// </summary>
    public DataTable GetSchema(string collectionName)
    {
        return collectionName.ToLowerInvariant() switch
        {
            "tables" => QueryPostgreSqlTables(),
            "columns" => QueryPostgreSqlColumns(),
            "views" => QueryPostgreSqlViews(),
            _ => new DataTable()
        };
    }

    /// <summary>
    /// Queries PostgreSQL for table information
    /// </summary>
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

    /// <summary>
    /// Queries PostgreSQL for column information
    /// </summary>
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
                column_default AS ""DefaultValue"",
                ordinal_position AS ""OrdinalPosition""
            FROM information_schema.columns
            WHERE table_schema NOT IN ('pg_catalog', 'information_schema')
            ORDER BY table_schema, table_name, ordinal_position";

        return ExecuteQuery(query);
    }

    /// <summary>
    /// Queries PostgreSQL for view information
    /// </summary>
    private DataTable QueryPostgreSqlViews()
    {
        const string query = @"
            SELECT
                table_schema AS ""SchemaName"",
                table_name AS ""ViewName"",
                view_definition AS ""Definition""
            FROM information_schema.views
            WHERE table_schema NOT IN ('pg_catalog', 'information_schema')
            ORDER BY table_schema, table_name";

        return ExecuteQuery(query);
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
