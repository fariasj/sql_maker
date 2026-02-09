using System;
using System.Reflection;
using System.ComponentModel;

namespace SqlV2;

//[Serializable]
//public struct KeyValuePair<K, V>
//{
//    public K Key { get; set; }
//    public V Value { get; set; }
//}

public enum DASqlType
{
    Table = 1,
    Procedure = 2
}

public enum DACondicional
{
    [Description(">")]
    MayorQue = 1,
    [Description("<")]
    MenorQue = 2,
    [Description("<>")]
    Entre = 3,
    [Description("><")]
    MayorQueYMenorQue = 4
}

public enum DATransactionType
{
    Borrar = 4,
    Consultar = 3,
    Alta = 1,
    Actualizar = 2,
    ConsultarColeccion = 5,
    BorrarColeccion = 6,
    Copiar = 7,
    ConsultaColeccionPorusuario
}

public class DAConstantes
{
    public static string CurrentUser
    {
        get
        {
            // Cross-platform solution: works on Linux, macOS, and Windows
            return Environment.UserName;
        }
    }

    public static string CurrentVersion
    {
        get
        {
            //return Assembly.LoadFrom(HostingEnvironment.MapPath("~/bin/" + Assembly.GetExecutingAssembly().FullName().Name + ".dll")).GetName().Version.ToString();
            return Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";
        }
    }
}

[System.AttributeUsage(AttributeTargets.Property)]
public class DAAttributes : System.Attribute
{
    public bool IsSqlParameter { get; set; }
    public string SqlParameterName { get; set; } = string.Empty;
    public string SqlColumnName { get; set; } = string.Empty;

    public bool IsKeyForSelect { get; set; }
    public bool IsKeyForUpdate { get; set; }
    public bool IsKeyForDelete { get; set; }
    public bool IsIdentity { get; set; }
    public bool IsNullable { get; set; }
}

[AttributeUsage(AttributeTargets.Class)]
public class DAClassAttributes : System.Attribute
{
    public string SqlTableName { get; set; } = string.Empty;
    public DASqlType SqlType { get; set; }
    public string SqlSchema { get; set; } = string.Empty;
}
