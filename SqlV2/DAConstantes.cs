using System.Reflection;
using System.ComponentModel;

namespace EntityGen;

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
    Delete = 4,
    Select = 3,
    Add = 1,
    Update = 2,
    SelectColection = 5,
    DeleteColection = 6,
    Copy = 7,
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
