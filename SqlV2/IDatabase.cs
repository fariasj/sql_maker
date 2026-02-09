using System.Data;

namespace EntityGen;

/// <summary>
/// Abstract interface for database operations
/// Provides database-agnostic access to different database systems
/// </summary>
public interface IDatabase : IDisposable
{
    /// <summary>
    /// Gets the connection string for this database
    /// </summary>
    string ConnectionString { get; }

    /// <summary>
    /// Gets the underlying database connection
    /// </summary>
    IDbConnection Connection { get; }

    /// <summary>
    /// Gets or sets informational messages from the database
    /// </summary>
    string InfoMessage { get; set; }

    /// <summary>
    /// Opens the database connection
    /// </summary>
    void Open();

    /// <summary>
    /// Closes the database connection
    /// </summary>
    void Close();

    /// <summary>
    /// Creates a command with the specified command text
    /// </summary>
    /// <param name="commandText">The SQL command text</param>
    /// <returns>A database command</returns>
    IDbCommand CreateCommand(string commandText);

    /// <summary>
    /// Executes a query and returns a data reader
    /// </summary>
    /// <param name="commandText">The SQL command text</param>
    /// <returns>A data reader</returns>
    IDataReader ExecuteReader(string commandText);

    /// <summary>
    /// Executes a non-query SQL command and returns the number of rows affected
    /// </summary>
    /// <param name="commandText">The SQL command text</param>
    /// <returns>Number of rows affected</returns>
    int ExecuteNonQuery(string commandText);

    /// <summary>
    /// Executes a query and returns the first column of the first row
    /// </summary>
    /// <param name="commandText">The SQL command text</param>
    /// <returns>The first column of the first row as an object</returns>
    object ExecuteScalar(string commandText);

    /// <summary>
    /// Executes a query and returns the results as a DataTable
    /// </summary>
    /// <param name="commandText">The SQL command text</param>
    /// <returns>A DataTable containing the query results</returns>
    DataTable ExecuteQuery(string commandText);

    /// <summary>
    /// Executes a query and returns the results as a DataSet
    /// </summary>
    /// <param name="commandText">The SQL command text</param>
    /// <returns>A DataSet containing the query results</returns>
    DataSet ExecuteQueryDataSet(string commandText);

    /// <summary>
    /// Retrieves schema information for the specified collection
    /// </summary>
    /// <param name="collectionName">The name of the schema collection to retrieve</param>
    /// <returns>A DataTable containing schema information</returns>
    DataTable GetSchema(string collectionName);
}
