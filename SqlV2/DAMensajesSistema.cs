using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data;


public class DAMensajesSistema
{
    public enum TipoMensaje
    {
        Informativo = 1,
        Error = 2
    }

    [Serializable]
    public class RegistroMensaje
    {
        public TipoMensaje TipoMensaje;
        public string TextoMensaje;
        public string Contexto;
        public DateTime Fecha;

        public RegistroMensaje()
        {
            TextoMensaje = "";
            Contexto = "";
            Fecha = DateTime.Now;
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private List<RegistroMensaje> mensajes;

    public DAMensajesSistema()
    {
        mensajes = new List<RegistroMensaje>();
    }

    public void Inicializar()
    {
        mensajes.Clear();
    }

    public DataTable TablaMensajes
    {
        get
        {
            DataTable dTable = new DataTable("MensajesSistema");
            DataRow dRow;

            dTable.Columns.Add("Tipo Mensaje");
            dTable.Columns.Add("Texto Mensaje");
            dTable.Columns.Add("Contexto");
            dTable.Columns.Add("Fecha");

            foreach (RegistroMensaje mensaje in mensajes)
            {
                dRow = dTable.NewRow();

                dRow["Tipo Mensaje"] = mensaje.TipoMensaje;
                dRow["Texto Mensaje"] = mensaje.TextoMensaje;
                dRow["Contexto"] = mensaje.Contexto;
                dRow["Fecha"] = mensaje.Fecha;

                dTable.Rows.Add(dRow);
            }

            return dTable;
        }
    }

    public List<RegistroMensaje> ListaMensajes
    {
        get
        {
            return mensajes;
        }
        set
        {
            mensajes = value;
        }
    }

    public void AdicionarMensajes(DAMensajesSistema fuente)
    {
        mensajes.InsertRange(mensajes.Count, fuente.mensajes);
    }

    public void RegistrarMensaje(TipoMensaje tipoMensaje, string textoMensaje, string contexto)
    {
        RegistroMensaje registroMensaje;
        StackTrace stackTrace = new StackTrace(true);
        StackFrame[] stackFrames = stackTrace.GetFrames();

        registroMensaje = new RegistroMensaje();

        registroMensaje.TipoMensaje = tipoMensaje;
        registroMensaje.TextoMensaje = textoMensaje;

        if (String.IsNullOrEmpty(contexto.Trim()))
        {
            foreach (StackFrame stackFrame in stackFrames)
            {
                if (stackFrame.GetMethod().Module.Name.Contains("RH."))
                {
                    if (registroMensaje.Contexto != string.Empty)
                    {
                        registroMensaje.Contexto += " -> ";
                    }
                    registroMensaje.Contexto += stackFrame.GetMethod().Module.Name + "." + stackFrame.GetMethod().Name + " (" + stackFrame.GetFileLineNumber().ToString() + ")";
                }
                else
                {
                    break;
                }
            }
        }
        else
        {
            registroMensaje.Contexto = contexto;
        }

        registroMensaje.Fecha = DateTime.Now;

        mensajes.Add(registroMensaje);
    }

    public void RegistrarMensaje(TipoMensaje tipoMensaje, string textoMensaje)
    {
        RegistrarMensaje(tipoMensaje, textoMensaje, "");
    }

    public void RegistrarMensaje(string textoMensaje)
    {
        RegistrarMensaje(TipoMensaje.Error, textoMensaje);
    }

    public void RegistrarMensaje(string textoMensaje, string contexto)
    {
        RegistrarMensaje(TipoMensaje.Error, textoMensaje, contexto);
    }

    public string ObtenerMensajes(string saltoLinea, string formatoFecha)
    {
        string str = string.Empty;

        for (int i = mensajes.Count - 1; i <= mensajes.Count; i--)
        {
            if (i < 0)
            {
                break;
            }

            str += "- " + (!string.IsNullOrEmpty(formatoFecha) ? mensajes[i].Fecha.ToString(formatoFecha) + "| " : "") +
                           mensajes[i].TextoMensaje + saltoLinea;
        }

        //foreach (var oX in mensajes)
        //{
        //    str += "- " + (!string.IsNullOrEmpty(formatoFecha) ? oX.Fecha.ToString(formatoFecha) + "| " : "") +
        //                    oX.TextoMensaje + saltoLinea;
        //}

        return str;
    }

    public string UltimoMensaje()
    {
        if (mensajes.Count > 0)
        {
            return mensajes[mensajes.Count - 1].TextoMensaje;
        }
        else
        {
            return string.Empty;
        }
    }
}

