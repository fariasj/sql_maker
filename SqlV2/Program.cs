using System.Data;
using System.Text;

namespace EntityGen;

class Program
{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var nameSpace = "EntityGen";
            var tableName = "123";
            var cnxString = @"Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=demo";
            var pathOfClass = @".\PartialClass\";

            //Pagos
            using (var cnx = new DAConexion(DatabaseType.PostgreSQL, cnxString))
            {
                var cmdSql = " SELECT * " +
                             " FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE " +
                             "   JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS " +
                             "   ON CONSTRAINT_COLUMN_USAGE.CONSTRAINT_NAME = TABLE_CONSTRAINTS.CONSTRAINT_NAME " +
                             "   WHERE CONSTRAINT_TYPE = 'PRIMARY KEY'";

                var xKeys = cnx.ExecuteQuery(cmdSql);
                var x = cnx.GetSchema("Tables");

                foreach (DataRow z in x.Rows)
                {
                    if (!Directory.Exists(pathOfClass))
                    {
                        Directory.CreateDirectory(pathOfClass);
                    }

                    tableName = z[1].ToString();
                    var sqlSchema = z[0].ToString();

                    // Generic query using information_schema (works with SQL Server and PostgreSQL)
                    cmdSql = @"SELECT
                                column_name AS COLUMN_NAME,
                                data_type AS TYPE_NAME,
                                character_maximum_length AS LENGTH,
                                is_nullable AS IS_NULLABLE,
                                column_default AS DEFAULT_VALUE,
                                ordinal_position AS ORDINAL_POSITION
                               FROM
                                information_schema.columns
                               WHERE
                                table_schema = '" + sqlSchema + @"'
                                AND table_name = '" + tableName + @"'
                               ORDER BY
                                ordinal_position";

                    var keysOfTable = (from DataRow dt in xKeys.Rows
                                       where dt.GetValue<string>("TABLE_NAME") == tableName
                                       select dt["COLUMN_NAME"]).ToList();

                    //cmdSql = "SP_Columns " + tableName;
                    var t = cnx.ExecuteQuery(cmdSql);


                    var stringClass = "using System;\n\n";

                    stringClass += "namespace " + nameSpace + "\n" +
                                  "{\n" +
                                  "\t[DAClassAttributes( " + 
                                  " SqlType = DASqlType.Table " + "," +
                                  " SqlSchema = \"" + sqlSchema + "\"" +
                                  ")]\n" +
                                  "\tpublic partial class " + tableName.ToTitleCase() + " : DASqlBaseV3<" + tableName.ToTitleCase() + ">\n" +
                                  "\t{\n";

                    stringClass += "\t\t//" + DAUtileriasSistema.RepeatCharacter(25, '*') + "\n";
                    stringClass += "\t\t//Archivo generado automaticamente por una utilidad de Abraham Farías." +
                                   "\n\t\t//No modificar el archivo a mano." +
                                   "\n\t\t//Fecha generación: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "\n";
                    stringClass += "\t\t//" + DAUtileriasSistema.RepeatCharacter(25, '*') + "\n\n";

                    stringClass += "\t\tpublic " + tableName.ToTitleCase() + "()\n\t\t{}\n\n";

                    foreach (DataRow y in t.Rows)
                    {
                        var sqlTypeName = y.GetValue<string>("TYPE_NAME");

                        var isNullable = y.GetValue<string>("IS_NULLABLE") == "YES";

                        var columnName = y.GetValue<string>("COLUMN_NAME");
                        var isIdentity = keysOfTable.Contains(columnName); //(sqlTypeName.Split(' ').Count() > 1 ? true : false);

                        var columnType = GetNetType(sqlTypeName);

                        if (columnType != "string" && isNullable)
                        {
                            columnType += "?";
                        }

                        string attributeName = "\t\t[DAAttributes(" +
                                                    (isIdentity ? "IsKeyForDelete = " + isIdentity.ToString().ToLower() + ", " : "") +
                                                    (isIdentity ? "IsIdentity = " + isIdentity.ToString().ToLower() + ", " : "") +
                                                    (isIdentity ? "IsKeyForUpdate = " + isIdentity.ToString().ToLower() + ", " : "") +
                                                    (isIdentity ? "IsKeyForSelect = " + isIdentity.ToString().ToLower() + ", " : "") +
                                                    (isNullable ? "IsNullable = " + "true, " : "") +
                                                    "IsSqlParameter = true, " +
                                                    "SqlColumnName = \"" + columnName + "\")]";

                        stringClass += attributeName + "\n";
                        stringClass += "\t\tpublic " + columnType + " " + columnName.ToTitleCase() + " { get; set; }" + "\n\n";
                    }

                    stringClass += "\t}\n}";

                    File.WriteAllText(pathOfClass + tableName.ToTitleCase() + ".cs", stringClass, Encoding.UTF8);

                    Console.WriteLine(stringClass);
                    //Console.ReadLine();
                    Console.Clear();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlType"></param>
        /// <returns></returns>
        static string GetNetType(string sqlType)
        {
            string netType = "";

            switch (sqlType.ToLower())
            {
                case "bigint": netType = "Int64"; break;
                case "binary": netType = "Byte[]"; break;
                case "bit": netType = "bool"; break;
                case "char": netType = "string"; break;
                case "date": netType = "DateTime"; break;
                case "datetime": netType = "DateTime"; break;
                case "datetime2": netType = "DateTime"; break;
                case "datetimeoffset": netType = "DateTimeOffset"; break;
                case "decimal": netType = "decimal"; break;
                case "filestream": netType = "Byte[]"; break;
                case "float": netType = "double"; break;
                case "image": netType = "Byte[]"; break;
                case "int identity": netType = "int"; break;
                case "int": netType = "int"; break;
                case "money": netType = "decimal"; break;
                case "nchar": netType = "string"; break;
                case "ntext": netType = "string"; break;
                case "numeric": netType = "decimal"; break;
                case "nvarchar": netType = "string"; break;
                case "real": netType = "Single"; break;
                case "rowversion": netType = "Byte[]"; break;
                case "smalldatetime": netType = "DateTime"; break;
                case "smallint": netType = "Int16"; break;
                case "smallmoney": netType = "decimal"; break;
                case "sql_variant": netType = "Object"; break;
                case "text": netType = "string"; break;
                case "time": netType = "TimeSpan"; break;
                case "timestamp": netType = "Byte[]"; break;
                case "tinyint": netType = "Byte"; break;
                case "uniqueidentifier": netType = "Guid"; break;
                case "varbinary": netType = "Byte[]"; break;
                case "varchar": netType = "string"; break;
                case "xml": netType = "XDocument"; break;
                case "sysname": netType = "string"; break;

                // PostgreSQL specific types
                case "json": netType = "string"; break;
                case "jsonb": netType = "string"; break;
                case "bytea": netType = "Byte[]"; break;
                case "uuid": netType = "Guid"; break;
                case "interval": netType = "TimeSpan"; break;
                case "cidr": netType = "string"; break;
                case "inet": netType = "string"; break;
                case "macaddr": netType = "string"; break;
                case "macaddr8": netType = "string"; break;
                case "pg_lsn": netType = "string"; break;
                case "tsquery": netType = "string"; break;
                case "tsvector": netType = "string"; break;
                case "txid_snapshot": netType = "string"; break;
                case "point": netType = "string"; break;
                case "line": netType = "string"; break;
                case "lseg": netType = "string"; break;
                case "box": netType = "string"; break;
                case "path": netType = "string"; break;
                case "polygon": netType = "string"; break;
                case "circle": netType = "string"; break;

                default: netType = sqlType; break;
            }

            return netType;
        }
    }
