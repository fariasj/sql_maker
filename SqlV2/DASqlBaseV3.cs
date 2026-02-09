using System.Text.Json.Serialization;

namespace EntityGen;

public delegate void DASqlErrorEventHandler(object sender, DASqlErrorEventArgs e);

public class DASqlErrorEventArgs : EventArgs
{
    public string MsgError { get; private set; } = string.Empty;
    public string SqlCmd { get; private set; } = string.Empty;
    public string SqlConexion { get; private set; } = string.Empty;
    public DATransactionType TipoOperacion { get; private set; }

    public DASqlErrorEventArgs(string msgError, string cmdSql, string cnxSql, DATransactionType tipoOperacion)
    {
        MsgError = msgError;
        SqlCmd = cmdSql;
        SqlConexion = cnxSql;
        TipoOperacion = tipoOperacion;
    }
}

public delegate void DASqlOkEventHandler(object sender, DASqlOkEventArgs e);

public class DASqlOkEventArgs : EventArgs
{
    public int Id { get; set; }
    public double TimeOfExecute { get; set; }
    public string SqlInfoMessage { get; set; } = string.Empty;
    public DATransactionType TipoOperacion { get; private set; }

    public DASqlOkEventArgs(int id, double timeOfExecute, string sqlMessage, DATransactionType tipoOperacion)
    {
        Id = id;
        TimeOfExecute = timeOfExecute;
        SqlInfoMessage = sqlMessage;
        TipoOperacion = tipoOperacion;
    }
}

public abstract class DASqlBaseV3<T> where T : DASqlBaseV3<T>
{
    [JsonIgnore]
    public DAMensajesSistema MensajesSistema { get; }

    public event DASqlErrorEventHandler? OnSqlError;
    public event DASqlOkEventHandler? OnSqlOk;

    protected string SP_NAME = string.Empty;
    protected int Id;

    private string infoSqlMessage = string.Empty;

    public DASqlBaseV3()
    {
        MensajesSistema = new DAMensajesSistema();
    }

    protected void sqlError(string sqlError, string cmdSql, string cnxSql, DATransactionType tipoOperacion)
    {
        OnSqlError?.Invoke(this, new DASqlErrorEventArgs(sqlError, cmdSql, cnxSql, tipoOperacion));
    }

    protected void sqlOk(double timeOfExecute, string infoMessage, DATransactionType tipoAfectacion)
    {
        OnSqlOk?.Invoke(this, new DASqlOkEventArgs(Id, timeOfExecute, infoMessage, tipoAfectacion));
    }

    private bool Afectar(DATransactionType tipoAfectacion, DAConexion cnx)
    {
        var startDate = DateTime.Now;

        var _accion = (int)tipoAfectacion;
        var huboErrores = false;
        var cmdSql = "";
        var localTransaction = false;

        try
        {
            var obj = this.GetType();
            var xT = obj.GetCustomAttributes(typeof(DAClassAttributes), false);

            localTransaction = cnx.BeginTransaction();

            DASqlType objetoDestino = DASqlType.Table;

            if (xT.Count() > 0)
            {
                objetoDestino = ((DAClassAttributes)xT[0]).SqlType;
            }

            switch (objetoDestino)
            {
                case DASqlType.Procedure:
                    cmdSql = SP_NAME + " " +
                                    "@Accion = " + _accion + ", " +
                                    DAUtileriasSistema.ObjectToSqlString(this, ',', '@');
                    break;

                case DASqlType.Table:
                    cmdSql = DAUtileriasSistema.ObjectToSqlString(tipoAfectacion, this);
                    break;
            }

            if (MensajesSistema.ListaMensajes.Count > 0)
            {
                throw new Exception("La información capturada no es válida.");
            }

            switch (tipoAfectacion)
            {
                case DATransactionType.Add:
                    var id = cnx.ExecuteScalar(cmdSql);
                    Id = id.Parse<int>();
                    break;

                case DATransactionType.Delete:
                case DATransactionType.DeleteColection:
                case DATransactionType.Update:
                    cnx.ExecuteNonQuery(cmdSql);
                    break;
            }

            if (localTransaction)
            {
                cnx.CommitTransaction();
            }

            var timeOfExecute = DateTime.Now.Subtract(startDate).TotalMilliseconds;

            OnSqlOk?.Invoke(this, new DASqlOkEventArgs(Id, timeOfExecute, cnx.InfoMessage, tipoAfectacion));
        }
        catch (Exception ex)
        {
            if (localTransaction)
            {
                cnx.RollbackTransaction();
            }

            if (!ex.Message.IsNullOrEmpty())
            {
                MensajesSistema.RegistrarMensaje(ex.Message.Replace("'", "~"));
            }

            huboErrores = true;

            MensajesSistema.RegistrarMensaje("No se pudo ejecutar la accion [" + tipoAfectacion.ToString().ToUpper() + "].");

            OnSqlError?.Invoke(this, new DASqlErrorEventArgs(ex.Message.Replace("'", "~"), cmdSql, cnx.Connection?.ConnectionString ?? string.Empty, tipoAfectacion));
        }

        return !huboErrores;
    }

    public virtual bool Guardar(DAConexion cnx)
    {
        return Afectar(DATransactionType.Add, cnx);
    }

    public virtual bool Modificar(DAConexion cnx)
    {
        return Afectar(DATransactionType.Update, cnx);
    }

    public virtual bool Borrar(DAConexion cnx)
    {
        return Afectar(DATransactionType.Delete, cnx);
    }

    public virtual bool BorrarColeccion(DAConexion cnx)
    {
        return Afectar(DATransactionType.DeleteColection, cnx);
    }

    public virtual bool Consultar(DAConexion cnx)
    {
        T? _obj;
        var startDate = DateTime.Now;

        int _accion = (int)DATransactionType.Select;
        string cmdSql = "";

        try
        {
            var obj = this.GetType();
            var xT = obj.GetCustomAttributes(typeof(DAClassAttributes), false);
            DASqlType objetoDestino = DASqlType.Table;

            if (xT.Count() > 0)
            {
                objetoDestino = ((DAClassAttributes)xT[0]).SqlType;
            }

            switch (objetoDestino)
            {
                case DASqlType.Procedure:
                    cmdSql = SP_NAME + " " +
                                    "@Accion = " + _accion + ", " +
                                    DAUtileriasSistema.ObjectToSqlString(this, ',', '@');
                    break;

                case DASqlType.Table:
                    cmdSql = DAUtileriasSistema.ObjectToSqlString(DATransactionType.Select, this);
                    break;
            }

            var dt = cnx.ExecuteQuery(cmdSql);

            DAUtileriasSistema.TableToObject(out _obj, dt);
            if (_obj != null)
            {
                DAUtileriasSistema.CopyProperties<T>(_obj, this);
            }

            var timeOfExecute = DateTime.Now.Subtract(startDate).TotalMilliseconds;

            OnSqlOk?.Invoke(this, new DASqlOkEventArgs(Id, timeOfExecute, cnx.InfoMessage, DATransactionType.Select));

            return true;
        }
        catch (Exception ex)
        {
            if (!ex.Message.IsNullOrEmpty())
            {
                MensajesSistema.RegistrarMensaje(ex.Message.Replace("'", "~"));
            }

            MensajesSistema.RegistrarMensaje("No se pudo ejecutar la accion [" + DATransactionType.Select.ToString().ToUpper() + "].");

            //MensajesSistema.RegistrarMensaje("No se pudo ejecutar la accion '" + DATransactionType.Consultar.ToString().ToUpper() + "'\n" +
            //                    "Error: " + ex.Message);

            OnSqlError?.Invoke(this, new DASqlErrorEventArgs(ex.Message, cmdSql, cnx.Connection?.ConnectionString ?? string.Empty, DATransactionType.Select));

            //_obj = default(T);
            return false;
        }
    }

    //[Obsolete]
    //public virtual DataTable ConsultarColeccion(DAConexion cnx)
    //{
    //    T _obj;
    //    var startDate = DateTime.Now;

    //    int _accion = (int)DATransactionType.ConsultarColeccion;

    //    string cmdSql = SP_NAME + " " +
    //                    "@Accion = " + _accion + ", " +
    //                    DAUtileriasSistema.ObjectToSqlString(this, ',', '@');

    //    try
    //    {
    //        var dt = cnx.ExecuteQuery(cmdSql);
    //        var timeOfExecute = DateTime.Now.Subtract(startDate).TotalMilliseconds;

    //        OnSqlOk?.Invoke(this, new DASqlOkEventArgs(Id, timeOfExecute, cnx.InfoMessage, DATransactionType.ConsultarColeccion));

    //        return dt;
    //    }
    //    catch (Exception ex)
    //    {
    //        MensajesSistema.RegistrarMensaje("No se pudo ejecutar la acción '" + DATransactionType.Consultar.ToString().ToUpper() + "'\n" +
    //                            "Error: " + ex.Message);

    //        OnSqlError?.Invoke(this, new DASqlErrorEventArgs(ex.Message, cmdSql, cnx.Connection.ConnectionString, DATransactionType.ConsultarColeccion));

    //        throw new Exception(ex.Message);
    //    }
    //}

    public virtual List<T> ConsultarColeccion(DAConexion cnx)
    {
        //T _obj;
        var startDate = DateTime.Now;

        int _accion = (int)DATransactionType.SelectColection;

        var cmdSql = "";
        var obj = this.GetType();
        var xT = obj.GetCustomAttributes(typeof(DAClassAttributes), false);
        DASqlType objetoDestino = DASqlType.Table;

        if (xT.Count() > 0)
        {
            objetoDestino = ((DAClassAttributes)xT[0]).SqlType;
        }

        switch (objetoDestino)
        {
            case DASqlType.Procedure:
                cmdSql = SP_NAME + " " +
                                "@Accion = " + _accion + ", " +
                                DAUtileriasSistema.ObjectToSqlString(this, ',', '@');
                break;

            case DASqlType.Table:
                cmdSql = DAUtileriasSistema.ObjectToSqlString(DATransactionType.SelectColection, this);
                break;
        }

        //string cmdSql = SP_NAME + " " +
        //                "@Accion = " + _accion + ", " +
        //                DAUtileriasSistema.ObjectToSqlString(this, ',', '@');

        //cmdSql = "Enc2_Preguntas @Accion = 5, @idencuesta = 759";

        try
        {
            var dt = cnx.ExecuteQuery(cmdSql);
            var objDestino = new List<T>();

            DAUtileriasSistema.TableToObject<List<T>>(out objDestino, dt);

            var timeOfExecute = DateTime.Now.Subtract(startDate).TotalMilliseconds;

            OnSqlOk?.Invoke(this, new DASqlOkEventArgs(Id, timeOfExecute, cnx.InfoMessage, DATransactionType.SelectColection));

            return objDestino;
        }
        catch (Exception ex)
        {
            MensajesSistema.RegistrarMensaje("No se pudo ejecutar la acción '" + DATransactionType.Select.ToString().ToUpper() + "'\n" +
                                "Error: " + ex.Message);

            OnSqlError?.Invoke(this, new DASqlErrorEventArgs(ex.Message, cmdSql, cnx.Connection?.ConnectionString ?? string.Empty, DATransactionType.SelectColection));

            throw new Exception(ex.Message);
        }
    }


    //public abstract IEnumerable<T> ConsultarColeccion(DAConexion cnx);
}
