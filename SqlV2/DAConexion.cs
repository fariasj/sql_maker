using System.Diagnostics;
using System.Data.Common;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace EntityGen;

public delegate void DAConexionEventHandler(object sender, DAConexionEventArgs e);

public class DAConexionEventArgs : EventArgs
{
    //public string _msg;
    public string Cmd { get; set; } = string.Empty;
    public SqlConnection? ConnectionString { get; set; }
    //public string CurrentMethod;

    public DAConexionEventArgs(string sqlcmd, SqlConnection? sqlConnection)
    {
        StackTrace trace = new StackTrace();
        StackFrame frame = new StackFrame(2);
        var method = frame.GetMethod();

        //_msg = msg;

        Cmd = "";

        if (method?.DeclaringType?.Name != "DAConexion")
        {
            Cmd += DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + "|" + DAConstantes.CurrentUser + "|" + method?.DeclaringType + " - " + method?.Name + "\t| " + sqlcmd;
        }

        //var s =
        ConnectionString = sqlConnection;
    }
}

public class DAConexion : IDisposable
{
    private void Cnx_OnExecuteQuery(object sender, DAConexionEventArgs e)
    {
        Debug.WriteLineIf(Debugger.IsAttached && !e.Cmd.IsNullOrEmpty(), e.Cmd);
    }

    private string cadconexion = string.Empty;
    private SqlConnection? sqlConexion;

    public string InfoMessage { get; set; } = string.Empty;

    public event DAConexionEventHandler? OnExecuteQuery;

    public SqlConnection? Connection
    {
        get { return sqlConexion; }
    }

    private SqlTransaction? sqlTransaction;
    private SqlCommand? sqlCommand;
    private bool disposed = false;

    public DAConexion()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        cadconexion = configuration.GetConnectionString("cnxDefault")
            ?? throw new Exception("Connection string 'cnxDefault' not found in appsettings.json");

        OnExecuteQuery += Cnx_OnExecuteQuery;

        CrearDbConnection();
    }

    public DAConexion(string connectionString)
    {
        cadconexion = connectionString;
        OnExecuteQuery += Cnx_OnExecuteQuery;

        CrearDbConnection();
    }

    public bool BeginTransaction()
    {
        if (sqlTransaction == null)
        {
            sqlTransaction = sqlConexion?.BeginTransaction();
            if (sqlCommand != null)
            {
                sqlCommand.Transaction = sqlTransaction;
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public void CommitTransaction()
    {
        // No se pregunta si es null porque la idea es que ocurra un error si se utiliza mal
        if (sqlTransaction != null)
        {
            sqlTransaction.Commit();
            sqlTransaction.Dispose();
            sqlTransaction = null;
        }

        if (sqlCommand != null)
        {
            sqlCommand.Transaction = null;
        }
    }

    public void RollbackTransaction()
    {
        // No se pregnta si es null porque la idea es que ocurra un error si se utiliza mal
        if (sqlTransaction != null)
        {
            sqlTransaction.Rollback();

            sqlTransaction.Dispose();
            sqlTransaction = null;

            if (sqlCommand != null)
            {
                sqlCommand.Transaction = null;
            }
        }
    }

    public string ExecuteScalar(string cmdSql)
    {
        OnExecuteQuery?.Invoke(this, new DAConexionEventArgs(cmdSql, this.Connection!));

        return ExecuteQuery(cmdSql).Rows[0][0]?.ToString() ?? string.Empty;
    }

    public DataRow ExecuteAndGetFirstRow(string cmdSql)
    {
        OnExecuteQuery?.Invoke(this, new DAConexionEventArgs(cmdSql, this.Connection!));

        return ExecuteQuery(cmdSql).Rows[0];
    }

    public DataSet ExecuteQueryDataSet(string cmdSql)
    {
        OnExecuteQuery?.Invoke(this, new DAConexionEventArgs(cmdSql, this.Connection!));

        DbDataAdapter? dbDataAdapter;
        DataSet dtTable = new DataSet();

        dtTable.Clear();

        if (sqlCommand != null)
        {
            sqlCommand.CommandText = cmdSql;
            dbDataAdapter = new SqlDataAdapter();
            dbDataAdapter.SelectCommand = sqlCommand;

            try
            {
                dbDataAdapter.Fill(dtTable);
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message + Environment.NewLine + cmdSql);
                throw new Exception(ex.Message + Environment.NewLine);
            }
        }
        else
        {
            throw new Exception("SQL Command not initialized");
        }

        return dtTable;
    }

    public DataTable ExecuteQuery(string cmdSql)
    {
        OnExecuteQuery?.Invoke(this, new DAConexionEventArgs(cmdSql, this.Connection!));

        return ExecuteQuery(cmdSql, false);
    }

    public DataTable ExecuteQuery(string cmdSql, bool throwEmptyData)
    {
        OnExecuteQuery?.Invoke(this, new DAConexionEventArgs(cmdSql, this.Connection!));

        DbDataAdapter? dbDataAdapter;
        DataTable dtTable = new DataTable();

        dtTable.Clear();

        if (sqlCommand != null)
        {
            sqlCommand.CommandText = cmdSql;
            dbDataAdapter = new SqlDataAdapter();
            dbDataAdapter.SelectCommand = sqlCommand;

            try
            {
                dbDataAdapter.Fill(dtTable);
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message + Environment.NewLine + cmdSql);
                throw new Exception(ex.Message + Environment.NewLine);
            }

            if (throwEmptyData)
            {
                if (dtTable.Rows.Count <= 0)
                {
                    throw new Exception("No se encontró información");
                }
            }
        }
        else
        {
            throw new Exception("SQL Command not initialized");
        }

        return dtTable;
    }

    private void CrearDbConnection()
    {
        sqlConexion = new SqlConnection();
        sqlConexion.ConnectionString = cadconexion;

        sqlConexion.InfoMessage += SqlConexion_InfoMessage;

        try
        {
            sqlConexion.Open();
            sqlCommand = (SqlCommand?)CrearDbCommand(0);
        }
        catch (Exception ex)
        {
            if (sqlCommand != null)
            {
                sqlCommand.Dispose();
            }

            if (sqlConexion != null)
            {
                sqlConexion.Dispose();
            }

            throw new Exception(ex.Message);
        }
    }

    private void SqlConexion_InfoMessage(object? sender, SqlInfoMessageEventArgs e)
    {
        InfoMessage = e.Message + "\n" + e.Source + "\n" + e.Errors;
        //throw new NotImplementedException();
    }

    private DbCommand? CrearDbCommand(int timeOut)
    {
        if (sqlConexion == null) return null;

        DbCommand dbCommand;
        string commandTimeout = timeOut.ToString();

        if ((timeOut <= 0))
        {
            commandTimeout = "50";
        }

        dbCommand = sqlConexion.CreateCommand();
        dbCommand.CommandTimeout = int.Parse(commandTimeout);
        dbCommand.Transaction = sqlTransaction;

        return dbCommand;
    }

    public int ExecuteNonQuery(string cmdSql)
    {
        OnExecuteQuery?.Invoke(this, new DAConexionEventArgs(cmdSql, this.Connection!));

        if (sqlCommand != null)
        {
            sqlCommand.CommandText = cmdSql;

            try
            {
                return sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message + Environment.NewLine + cmdSql);
                throw new Exception(ex.Message + Environment.NewLine);
            }
        }
        else
        {
            throw new Exception("SQL Command not initialized");
        }
    }


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
        if (!this.disposed)
        {
            if (disposing)
            {
                if (sqlTransaction != null)
                {
                    sqlTransaction.Dispose();
                }

                if (sqlCommand != null)
                {
                    sqlCommand.Dispose();
                }

                if (sqlConexion != null)
                {
                    sqlConexion.Dispose();
                }
            }
        }

        disposed = true;
    }

    #endregion
}
