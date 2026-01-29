using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Configuration;
using Newtonsoft.Json;

public static class DAUtileriasSistema
{
    public static object GetPropertyValue(object objSource, string propertyName)
    {
        try
        {
            return objSource.GetType().GetProperty(propertyName).GetValue(objSource, null);
        }
        catch
        {
            throw new Exception("No se encontro la propiedad [ " + propertyName + " ] en el objeto " + objSource.ToString());
        }
    }

    //[Obsolete]
    //public static string ObjectToSqlString(DADatabases forDatabase, object objSource, char separadorCampos, char predecesorIdentificador, params string[] optional)
    //{
    //    string spName = "";
    //    string dataBaseName = "For" + forDatabase.ToString().ToUpper();
    //    string inDatabase = "In" + forDatabase.ToString().ToUpper();
    //    string strDummy = string.Empty;
    //    int countDummy = 0;

    //    bool ty = false;

    //    var objProperties = objSource.GetType().GetProperties();

    //    foreach (PropertyInfo oP in objProperties)
    //    {
    //        object parameterValue = oP.GetValue(objSource, null);

    //        var isSqlParameter = GetAttributeValue<DAAttributes, bool>(objSource.GetType(), oP.Name, "IsSqlParameter");

    //        var dBase = GetAttributeValue<DAAttributes, bool>(objSource.GetType(), oP.Name, dataBaseName);

    //        if (oP.Name == "SP")
    //        {
    //            spName = oP.GetValue(objSource, null).ToString();
    //        }

    //        if (!dBase)
    //        {
    //            continue;
    //        }

    //        if (isSqlParameter)
    //        {
    //            ty = true;

    //            countDummy++;
    //            var parameterName = GetAttributeValue<DAAttributes, string>(objSource.GetType(), oP.Name, "SqlParameterName");

    //            try
    //            {
    //                var customName = GetAttributeValue<DACustomSqlNames, string>(objSource.GetType(), oP.Name, inDatabase);
    //                parameterName = customName;
    //            }
    //            catch
    //            {
    //                /* Cuando la propiedad no contiene atributos 'DASpNames' */
    //            }

    //            if (string.IsNullOrEmpty(parameterName))
    //            {
    //                parameterName = oP.Name;
    //            }

    //            switch (oP.PropertyType.Name)
    //            {
    //                case "Char":
    //                case "String":
    //                    parameterValue = (Convert.ToString(parameterValue)).ToSqlString();
    //                    break;

    //                case "Int16":
    //                case "Int32":
    //                case "Int64":
    //                    parameterValue = (Convert.ToString(parameterValue)).ToSqlNumber();
    //                    break;

    //                case "Datetime":
    //                    parameterValue = (Convert.ToDateTime(parameterValue)).ToSqlDateTime();
    //                    break;

    //                case "XmlDocument":
    //                    parameterValue = (((XmlDocument)parameterValue).OuterXml).ToSqlString();
    //                    break;
    //            }

    //            strDummy += predecesorIdentificador + parameterName + " = " + parameterValue +
    //                (oP == objProperties.LastOrDefault() ? "" : separadorCampos.ToString() + " ") +
    //                (countDummy == 5 ? Environment.NewLine : "");

    //            if (countDummy >= 5) { countDummy = 0; }
    //        }
    //    }

    //    strDummy = strDummy.TrimEnd();

    //    if (strDummy.EndsWith(separadorCampos.ToString()))
    //    {
    //        strDummy = strDummy.Substring(0, strDummy.Length - separadorCampos.ToString().Length);
    //    }

    //    foreach (string str in optional)
    //    {
    //        strDummy += "," + str;
    //    }

    //    strDummy = spName + " " + strDummy;
    //    return ty ? strDummy : string.Empty;
    //}

    public static void CopyProperties<T>(this T source, object destination)
    {
        if (source == null || destination == null)
        {
            throw new Exception("Source or/and Destination Objects are null");
        }

        // Getting the Types of the objects
        Type typeDest = destination.GetType();
        Type typeSrc = source.GetType();

        // Iterate the Properties of the source instance and  
        // populate them from their desination counterparts  
        PropertyInfo[] srcProps = typeSrc.GetProperties();
        foreach (PropertyInfo srcProp in srcProps)
        {
            if (!srcProp.CanRead)
            {
                continue;
            }

            PropertyInfo targetProperty = typeDest.GetProperty(srcProp.Name);
            if (targetProperty == null)
            {
                continue;
            }
            if (!targetProperty.CanWrite)
            {
                continue;
            }
            if (targetProperty.GetSetMethod(true) != null && targetProperty.GetSetMethod(true).IsPrivate)
            {
                continue;
            }
            if ((targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0)
            {
                continue;
            }
            if (!targetProperty.PropertyType.IsAssignableFrom(srcProp.PropertyType))
            {
                continue;
            }
            // Passed all tests, lets set the value
            targetProperty.SetValue(destination, srcProp.GetValue(source, null), null);
        }
    }

    public static string ObjectToSqlString(DATransactionType typeOfTransaction, object objSource)
    {
        string strDummy = string.Empty;
        int countDummy = 0;
        var tableName = "";
        var type = "";

        var values = new Dictionary<string, string>();
        Dictionary<string, string> keyDeleteValues =  new Dictionary<string, string>();
        Dictionary<string, string> keyUpdateValues  = new Dictionary<string, string>();
        Dictionary<string, string> keySelectValues = new Dictionary<string, string>();

        var objProperties = objSource.GetType().GetProperties();

        try
        {
            tableName = ((DAClassAttributes)objSource.GetType().GetCustomAttribute(typeof(DAClassAttributes))).SqlTableName;

            if (tableName == null)
            {
                throw new Exception();
            }
        }
        catch
        {
            tableName = objSource.GetType().Name;
        }

        foreach (PropertyInfo oP in objProperties)
        {
            object parameterValue = oP.GetValue(objSource, null);

            var isSqlParameter = GetAttributeValue<DAAttributes, bool>(objSource.GetType(), oP.Name, "IsSqlParameter");

            if (isSqlParameter)
            {
                countDummy++;
                var parameterName = GetAttributeValue<DAAttributes, string>(objSource.GetType(), oP.Name, "SqlParameterName");
                var columnName = GetAttributeValue<DAAttributes, string>(objSource.GetType(), oP.Name, "SqlColumnName");

                var isKeySelect = GetAttributeValue<DAAttributes, bool>(objSource.GetType(), oP.Name, "IsKeyForSelect");
                var isKeyDelete = GetAttributeValue<DAAttributes, bool>(objSource.GetType(), oP.Name, "IsKeyForDelete");
                var isKeyUpdate = GetAttributeValue<DAAttributes, bool>(objSource.GetType(), oP.Name, "IsKeyForUpdate");
                var isIdentity = GetAttributeValue<DAAttributes, bool>(objSource.GetType(), oP.Name, "IsIdentity");
                var isNullable = GetAttributeValue<DAAttributes, bool>(objSource.GetType(), oP.Name, "IsNullable");

                switch (typeOfTransaction)
                {
                    case DATransactionType.Borrar:
                        isKeyUpdate = false;
                       // keyDeleteValues = new Dictionary<string, string>();
                        break;

                    case DATransactionType.Actualizar:
                        isKeyDelete = false;
                       //keyUpdateValues = new Dictionary<string, string>();
                        break;

                    case DATransactionType.Consultar:
                        
                        break;
                }

                if (string.IsNullOrEmpty(parameterName))
                {
                    parameterName = oP.Name;
                }

                if (!columnName.IsNullOrEmpty())
                {
                    parameterName = columnName;
                }

                if (typeOfTransaction == DATransactionType.Alta || typeOfTransaction == DATransactionType.Actualizar)
                {
                    if ((!isNullable) && (parameterValue == null))
                    {
                        var mensajesSistema = (DAMensajesSistema)objSource.GetType().GetProperty("MensajesSistema").GetValue(objSource);
                        var mensaje = "[" + parameterName.ToTitleCase() + "] no fue especificado y es requerido.";

                        mensajesSistema.RegistrarMensaje(DAMensajesSistema.TipoMensaje.Error, mensaje);
                    }
                }

                type = oP.PropertyType.Name;

                if (oP.PropertyType.GenericTypeArguments.Count() > 0)
                {
                    type = oP.PropertyType.GenericTypeArguments[0].Name;
                }

                switch (type)
                {
                    case "Char":
                    case "String":
                        parameterValue = (Convert.ToString(parameterValue)).ToSqlString();

                        if (isNullable && parameterValue.ToString().IsNullOrEmpty())
                        {
                            parameterValue = "Null";
                        }

                        if (parameterValue.ToString().Replace("'", "").IsNullOrEmpty())
                        {
                            continue;
                        }

                        break;

                    case "Int16":
                    case "Int32":
                    case "Int64":
                        parameterValue = (Convert.ToString(parameterValue)).ToSqlNumber();

                        if (isNullable && parameterValue.ToString().IsNullOrEmpty())
                        {
                            parameterValue = "Null";
                        }

                        break;

                    case "DateTime":
                        //if ((DateTime)parameterValue == default(DateTime))
                        //{
                        //    parameterValue = new DateTime(1970, 1, 1);
                        //}

                        if (!isNullable)
                        {
                            parameterValue = (Convert.ToDateTime(parameterValue)).ToSqlDateTime();
                        }
                        else
                        {
                            if (parameterValue == null)
                            {
                                parameterValue = "Null";
                            }

                            //parameterValue = null;
                        }
                        
                        //if (parameterValue == new DateTime(1970, 1, 1))
                        //    continue;

                        break;

                    case "XmlDocument":
                        parameterValue = (((XmlDocument)parameterValue).OuterXml).ToSqlString();
                        break;

                    case "Byte[]":
                        //parameterValue = Dau

                        break;

                    default:
                        parameterValue = "Null";
                        break;
                }

                if (typeOfTransaction != DATransactionType.Alta)
                {
                    bool isKey = false;
                    if (isKeyDelete)
                    {
                        keyDeleteValues.Add(parameterName, parameterValue.ToString());
                        isKey = true;
                    }

                    if (isKeyUpdate)
                    {
                        keyUpdateValues.Add(parameterName, parameterValue.ToString());
                        isKey = true;
                    }

                    if (isKeySelect)
                    {
                        keySelectValues.Add(parameterName, parameterValue.ToString());
                    }
                    
                    if (!isKey)
                    {
                        values.Add(parameterName, parameterValue.ToString());
                    }
                }
                else
                {
                    if (!isIdentity)
                    {
                        values.Add(parameterName, parameterValue.ToString());
                    }
                }

                //strDummy += predecesorIdentificador + parameterName + " = " + parameterValue +
                //    (oP == objProperties.LastOrDefault() ? "" : separadorCampos.ToString() + " ");
                //+ (countDummy == 5 ? Environment.NewLine : "");

                if (countDummy >= 5) { countDummy = 0; }
            }
        }

        var contador = 0;
        switch (typeOfTransaction)
        {
            case DATransactionType.Alta:
                strDummy = "Insert into " + tableName + "\t";
                var stColumns = "";
                var stValues = "";

                foreach (var x in values)
                {
                    contador++;

                    stColumns += x.Key + (contador < values.Count ? ", " : "");
                    stValues += x.Value + (contador < values.Count ? ", " : "");
                }

                stColumns = "(" + stColumns + ")";
                stValues = "(" + stValues + ")";

                strDummy = strDummy + stColumns + "\nValues \t" + stValues;
                break;

            case DATransactionType.BorrarColeccion:
            case DATransactionType.Borrar:
                strDummy = "Delete From " + tableName + "\n";
                //int contador = 0;

                if (keyDeleteValues.Count > 0)
                {
                    foreach (var x in keyDeleteValues)
                    {
                        if (contador == 0)
                        {
                            strDummy += "Where\t" + x.Key + " = " + x.Value + "\n";
                        }
                        else
                        {
                            strDummy += "And\t" + x.Key + " = " + x.Value + "\n";
                        }

                        contador++;
                    }
                }

                break;

            case DATransactionType.Actualizar:

                if (keyUpdateValues.Count <= 0)
                {
                    throw new Exception("No se ha definido una llave condicional para el la accion 'Update'.");
                }

                strDummy = "Update\t" + tableName + "\nSet\t";
                var strValues = "";
                var conditionalValues = "";

                foreach (var x in values)
                {
                    contador++;
                    strValues += x.Key + " = " + x.Value + (contador < values.Count ? ",\n\t" : "");
                }

                contador = 0;
                foreach (var x in keyUpdateValues)
                {
                    if (contador == 0)
                    {
                        conditionalValues += "Where\t" + x.Key + " = " + x.Value + "\n";
                    }
                    else
                    {
                        conditionalValues += "And\t" + x.Key + " = " + x.Value + "\n";
                    }

                    contador++;
                }

                strDummy += strValues + "\n" + conditionalValues;

                break;

            case DATransactionType.ConsultarColeccion:
            case DATransactionType.Consultar:
                
                strDummy = "Select * From " + tableName + "\n\t";

                if (keySelectValues.Count > 0)
                {
                    foreach (var x in keySelectValues)
                    {
                        if (contador == 0)
                        {
                            strDummy += "Where\t" + x.Key + " = " + x.Value + "\n";
                        }
                        else
                        {
                            strDummy += "And\t" + x.Key + " = " + x.Value + "\n";
                        }

                        contador++;
                    }
                }

                strDummy += "\n\t";


                break;
        }

        strDummy = strDummy.TrimEnd();

        return strDummy;
    }

    public static string ObjectToSqlString(object objSource, char separadorCampos, char predecesorIdentificador)
    {
        string strDummy = string.Empty;
        int countDummy = 0;

        var objProperties = objSource.GetType().GetProperties();

        foreach (PropertyInfo oP in objProperties)
        {
            object parameterValue = oP.GetValue(objSource, null);

            var isSqlParameter = GetAttributeValue<DAAttributes, bool>(objSource.GetType(), oP.Name, "IsSqlParameter");

            if (isSqlParameter)
            {
                countDummy++;
                var parameterName = GetAttributeValue<DAAttributes, string>(objSource.GetType(), oP.Name, "SqlParameterName");

                if (string.IsNullOrEmpty(parameterName))
                {
                    parameterName = oP.Name;
                }

                switch (oP.PropertyType.Name)
                {
                    case "Char":
                    case "String":
                        parameterValue = (Convert.ToString(parameterValue)).ToSqlString();

                        if (parameterValue.ToString().Replace("'", "").IsNullOrEmpty())
                            continue;
                        break;

                    case "Int16":
                    case "Int32":
                    case "Int64":
                        parameterValue = (Convert.ToString(parameterValue)).ToSqlNumber();
                        break;

                    case "DateTime":
                        if ((DateTime)parameterValue == default(DateTime))
                        {
                            parameterValue = new DateTime(1970, 1, 1);
                        }

                        parameterValue = (Convert.ToDateTime(parameterValue)).ToSqlDateTime();

                        //if (parameterValue == new DateTime(1970, 1, 1))
                        //    continue;

                        break;

                    case "XmlDocument":
                        parameterValue = (((XmlDocument)parameterValue).OuterXml).ToSqlString();
                        break;

                    default:

                        parameterValue = "Null";

                        break;
                }

                strDummy += predecesorIdentificador + parameterName + " = " + parameterValue +
                    (oP == objProperties.LastOrDefault() ? "" : separadorCampos.ToString() + " ");
                //+ (countDummy == 5 ? Environment.NewLine : "");

                if (countDummy >= 5) { countDummy = 0; }
            }
        }

        strDummy = strDummy.TrimEnd();

        if (strDummy.EndsWith(separadorCampos.ToString()))
        {
            strDummy = strDummy.Substring(0, strDummy.Length - separadorCampos.ToString().Length);
        }

        return strDummy;
    }

    public static attributeType GetAttributeValue<classType, attributeType>(Type objectType, string propertyName, string attributePropertyName)
    {
        var propertyInfo = objectType.GetProperty(propertyName);

        if (propertyInfo != null)
        {
            if (Attribute.IsDefined(propertyInfo, typeof(classType)))
            {
                var attributeInstance = Attribute.GetCustomAttribute(propertyInfo, typeof(classType));

                if (attributeInstance != null)
                {
                    foreach (PropertyInfo info in typeof(classType).GetProperties())
                    {
                        if (info.CanRead && String.Compare(info.Name, attributePropertyName, StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            return (attributeType)info.GetValue(attributeInstance, null);
                        }
                    }
                }
            }
        }

        return Activator.CreateInstance<attributeType>();
    }

    public static bool IntToBool(int base2Number, int number)
    {
        return Convert.ToBoolean((base2Number & (int)(Math.Pow(2, number))) / (int)Math.Pow(2, number));
    }

    public static int BitToInt(int bitNumber)
    {
        return (int)Math.Pow(2, bitNumber);
    }

    public static void TableToObject<T>(out T objDestino, DataTable dtResultado)
    {
        if (dtResultado.Rows.Count <= 0)
        {
            throw new Exception("No se encontraron registros");
        }

        objDestino = Activator.CreateInstance<T>();
        var isList = false;
        var typeOfElement = objDestino.GetType();

        if (objDestino.IsGenericList())
        {
            typeOfElement = objDestino.GetType().GetGenericArguments().Single();
            isList = true;
        }

        foreach (DataRow oR in dtResultado.Rows)
        {
            var xDestino = Activator.CreateInstance(typeOfElement);

            foreach (DataColumn oC in dtResultado.Columns)
            {
                PropertyInfo oP = null;
                var columnMap = "";

                foreach (PropertyInfo oPInfo in xDestino.GetType().GetProperties())
                {
                    try
                    {
                        var oF = GetAttributeValue<DAAttributes, string>(objDestino.GetType(), oPInfo.Name, "SqlColumnName");

                        if (oF == oC.ColumnName)
                        {
                            columnMap = oF;
                            oP = oPInfo;
                            break;
                        }
                    }
                    catch
                    { }
                }

                if (oP == null)
                {
                    oP = objDestino.GetType().GetProperty(oC.ColumnName);
                }

                if (oP != null)
                {
                    if (string.IsNullOrEmpty(columnMap))
                    {
                        columnMap = oC.ColumnName;
                    }

                    object obj = new object();
                    switch (oP.PropertyType.Name)
                    {
                        case "String":
                            obj = Convert.ToString(dtResultado.Rows[dtResultado.Rows.IndexOf(oR)][columnMap]).TrimEnd();
                            break;

                        case "Int16":
                        case "Int32":
                            obj = Convert.ToInt32((dtResultado.Rows[dtResultado.Rows.IndexOf(oR)][columnMap].ToString().IsNullOrEmpty() ? 0 :
                                dtResultado.Rows[dtResultado.Rows.IndexOf(oR)][columnMap]));
                            break;

                        case "Int64":
                            obj = Convert.ToInt64(dtResultado.Rows[dtResultado.Rows.IndexOf(oR)][columnMap]);
                            break;

                        case "DateTime":
                            if (dtResultado.Rows[dtResultado.Rows.IndexOf(oR)][columnMap] != DBNull.Value)
                            {
                                obj = DateTime.Parse(dtResultado.Rows[dtResultado.Rows.IndexOf(oR)][columnMap].ToString());
                            }
                            else
                            {
                                obj = default(DateTime);
                            }

                            break;

                        default:
                            continue;
                            //throw new Exception("La propiedad contiene un tipo no identificado. \n" + oP.PropertyType.Name);
                    }

                    oP.SetValue(xDestino, obj, null);
                }
            }

            if (isList)
            {
                objDestino.GetType().GetMethod("Add").Invoke(objDestino, new[] { xDestino });
            }
            else
            {
                objDestino = (T)xDestino;
            }

        }
    }


    public static string CadenaParaXml(string origStr, int limitChars, bool esXml)
    {
        string newStr = origStr;

        /* True:
		 * Se trata de un string que contiene una definición de un xml
		 * y por lo tanto sólo necesita que se reemplacen las letras y otros caracteres especiales
		 * pero no los caracteres <, >, ", \t, \r, etc.
		*/

        if ((limitChars > 0) && (origStr.Length > limitChars))
        {
            newStr = newStr.Substring(0, limitChars);  // los campos Title, Prompt y Name tienen ciertos límites
        }

        newStr = newStr.Replace("&", "&amp;");
        newStr = newStr.Replace("'", "&apos;");
        newStr = newStr.Replace("á", "&#225;");
        newStr = newStr.Replace("é", "&#233;");
        newStr = newStr.Replace("í", "&#237;");
        newStr = newStr.Replace("ó", "&#243;");
        newStr = newStr.Replace("ú", "&#250;");
        newStr = newStr.Replace("Á", "&#193;");
        newStr = newStr.Replace("É", "&#201;");
        newStr = newStr.Replace("Í", "&#205;");
        newStr = newStr.Replace("Ó", "&#211;");
        newStr = newStr.Replace("Ú", "&#218;");
        newStr = newStr.Replace("ñ", "&#241;");
        newStr = newStr.Replace("Ñ", "&#209;");
        newStr = newStr.Replace("ü", "&#252;");
        newStr = newStr.Replace("Ü", "&#220;");
        newStr = newStr.Replace("¿", "&#191;");
        newStr = newStr.Replace("¡", "&#161;");
        newStr = newStr.Replace(",", "&#044;");

        if (!esXml)
        {
            newStr = newStr.Replace("\"", "&quot;");
            newStr = newStr.Replace("'", "&apos;");
            newStr = newStr.Replace("<", "&lt;");
            newStr = newStr.Replace(">", "&gt;");
            newStr = newStr.Replace("\t", "&#09;");
            newStr = newStr.Replace("\r", "&#10;");
        }

        return (newStr);
    }

    public static string StringToHex(string origStr)
    {
        string str = origStr;
        char[] charValues = str.ToCharArray();
        string hexOutput = "";

        foreach (char _eachChar in charValues)
        {
            int value = Convert.ToInt32(_eachChar);
            hexOutput += String.Format("{0:X}", value);
        }

        return hexOutput.ToLower();
    }

    public static byte[] StringToBytes(string str)
    {
        byte[] bytes = new byte[str.Length * sizeof(char)];
        System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
        return bytes;
    }

    public static string BytesToString(byte[] bytes)
    {
        char[] chars = new char[bytes.Length / sizeof(char)];
        System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
        return new string(chars);
    }

    public static string HextToString(string hexValue)
    {
        StringBuilder oSb = new StringBuilder();

        for (int i = 0; i <= hexValue.Length - 2; i += 2)
        {
            oSb.Append(Convert.ToString(Convert.ToChar(Int32.Parse(hexValue.Substring(i, 2),
            System.Globalization.NumberStyles.HexNumber))));
        }

        return oSb.ToString();
    }

    public static string HexToOctal(string hexValue)
    {
        //First hex string to byte array
        var binaryval = Convert.ToString(Convert.ToInt32(hexValue, 16), 8);
        return binaryval;
    }

    public static string RepeatCharacter(int totalLenght, char caracter)
    {
        return caracter.ToString().PadRight(totalLenght, caracter);
    }

    public static string TransformXml(string stringXslt, string stringXml)
    {
        string text = string.Empty;

        //var stringXslt = string.Concat(File.ReadAllLines(fullPathXLST));
        //var stringXml = string.Concat(File.ReadAllLines(fullPathXml));

        using (StringReader srt = new StringReader(stringXslt)) // xslInput is a string that contains xsl
        using (StringReader sri = new StringReader(stringXml)) // xmlInput is a string that contains xml
        {
            using (XmlReader xrt = XmlReader.Create(srt))
            using (XmlReader xri = XmlReader.Create(sri))
            {
                XslCompiledTransform xslt = new XslCompiledTransform();
                xslt.Load(xrt);

                using (StringWriter sw = new StringWriter())
                using (XmlWriter xwo = XmlWriter.Create(sw, xslt.OutputSettings)) // use OutputSettings of xsl, so it can be output as HTML
                {
                    xslt.Transform(xri, xwo);
                    text = sw.ToString();
                }
            }
        }

        return text;
    }

    public static List<T> GetConstantValues<T>(Type type)
    {
        FieldInfo[] fields = type.GetFields(BindingFlags.Public
            | BindingFlags.Static
            | BindingFlags.FlattenHierarchy);

        return (fields.Where(fieldInfo => fieldInfo.IsLiteral
                && !fieldInfo.IsInitOnly
                && fieldInfo.FieldType == typeof(T)).Select(fi => (T)fi.GetRawConstantValue())).ToList();
    }

    public static List<Type> GetClassInAssembly(Assembly oAssembly)
    {
        return (from oC in oAssembly.GetTypes() select oC).ToList<Type>();
    }

    public static List<Type> GetClassInAssembly(string assemblyName)
    {
        Assembly oAssembly = Assembly.Load(assemblyName);
        return GetClassInAssembly(oAssembly);
    }

    public static T NextValue<T>(IEnumerable collection, T startValue)
    {
        object returnValue;

        try
        {
            var index = collection.OfType<T>().ToList().IndexOf(startValue);

            returnValue = collection.OfType<T>().ToList()[index + 1];
        }
        catch
        {
            returnValue = collection.OfType<T>().ToList().LastOrDefault();
        }

        return (T)returnValue;
    }

    public static void PrintProperties(object obj)
    {
        PrintProperties(obj, 0);
    }

    public static void PrintProperties(object obj, int indent)
    {
        if (obj == null) return;

        string indentString = new string(' ', indent);
        Type objType = obj.GetType();
        PropertyInfo[] properties = objType.GetProperties();

        foreach (PropertyInfo property in properties)
        {
            object propValue = property.GetValue(obj, null);

            if (property.PropertyType.Assembly == objType.Assembly)
            {
                Debug.WriteLine(string.Format("{0}{1}:", indentString, property.Name));
                PrintProperties(propValue, indent + 2);
            }
            else
            {
                Debug.WriteLine(string.Format("{0}{1}: {2}", indentString, property.Name, propValue));
            }
        }
    }

    public static string GetMethodInfo()
    {
        var oSt = new StackTrace();
        var oTrace = NextValue<StackFrame>(oSt.GetFrames().ToList(), oSt.GetFrames().FirstOrDefault());
        var sttParams = string.Empty;

        oTrace.GetMethod().GetParameters().ToList().ForEach(oX => sttParams += oX.ParameterType + " " + oX.Name + ", ");
        sttParams = sttParams.Substring(0, sttParams.Length - 2);

        return oTrace.GetMethod().Name + "(" + sttParams + ")";
    }

    public static string IndentXMLString(string xml)
    {
        string outXml = string.Empty;
        MemoryStream ms = new MemoryStream();
        XmlTextWriter xtw = new XmlTextWriter(ms, Encoding.Unicode);
        XmlDocument doc = new XmlDocument();

        doc.LoadXml(xml);

        xtw.Formatting = System.Xml.Formatting.Indented;

        doc.WriteContentTo(xtw);
        xtw.Flush();

        ms.Seek(0, SeekOrigin.Begin);

        StreamReader sr = new StreamReader(ms);

        return sr.ReadToEnd();
    }

    //public static string GetUserIP(HttpRequest request)
    //{
    //    string ipList = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

    //    if (!string.IsNullOrEmpty(ipList))
    //    {
    //        return string.Concat(ipList.Split(','));
    //    }

    //    return request.ServerVariables["REMOTE_ADDR"];
    //}

    //public static object DeserializarXML(XmlDocument oXml, Type oType)
    //{
    //    //Assuming oXml is an XML document containing a serialized object and oType is a System.Type set to the type of the object.

    //    var ser = new XmlSerializer(oType);
    //    object obj = null;

    //    using (var reader = new XmlNodeReader(oXml.DocumentElement))
    //    {
    //        obj = ser.Deserialize(reader);
    //    }

    //    return obj;
    //}

    public static T XmlToObject<T>(XmlDocument oXml)
    {
        //Assuming oXml is an XML document containing a serialized object and oType is a System.Type set to the type of the object.
        Type oType = typeof(T);
        var ser = new XmlSerializer(oType);
        object obj = null;

        using (var reader = new XmlNodeReader(oXml.DocumentElement))
        {
            obj = ser.Deserialize(reader);
        }

        return (T)obj;
    }

    public static T XmlToObject<T>(string xmlString)
    {
        //Assuming oXml is an XML document containing a serialized object and oType is a System.Type set to the type of the object.

        XmlDocument oXml = new XmlDocument();
        oXml.LoadXml(xmlString);

        Type oType = typeof(T);
        var ser = new XmlSerializer(oType);
        object obj = null;

        using (var reader = new XmlNodeReader(oXml.DocumentElement))
        {
            obj = ser.Deserialize(reader);
        }

        return (T)obj;
    }

    public static XmlDocument ObjectToXml(object oObjeto, XmlSerializerNamespaces oNamespaces)
    {
        return ObjectToXml(oObjeto, oNamespaces, true);
    }

    public static XmlDocument ObjectToXml(object oObjeto, XmlSerializerNamespaces oNamespaces, bool omitXmlDeclaration)
    {
        XmlSerializer oSerializer = new XmlSerializer(oObjeto.GetType());
        XmlDocument oXdoc = new XmlDocument();

        if (oNamespaces == null)
        {
            oNamespaces = new XmlSerializerNamespaces();
            oNamespaces.Add("", "");
        }

        using (MemoryStream oMs = new MemoryStream())
        {
            XmlWriterSettings oStt = new XmlWriterSettings();
            oStt.Encoding = new UTF8Encoding(false, true);
            oStt.Indent = false;
            oStt.IndentChars = "\t";
            oStt.OmitXmlDeclaration = omitXmlDeclaration;
            oStt.CheckCharacters = false;

            oStt.NewLineChars = Environment.NewLine;
            oStt.ConformanceLevel = ConformanceLevel.Document;

            using (XmlWriter oXmlW = XmlTextWriter.Create(oMs, oStt))
            {
                oSerializer.Serialize(oXmlW, oObjeto, oNamespaces);
            }

            oMs.Position = 0;

            var oXml = Encoding.UTF8.GetString(oMs.ToArray(), 0, oMs.ToArray().Length);

            oXdoc.LoadXml(oXml);
        }

        return oXdoc;
    }

    public static string GetAppSettingValue(string keyValue)
    {
        return ConfigurationManager.AppSettings[keyValue];
    }

    public static string DataTableToHTML(DataTable dt)
    {
        string html = "<table>";

        html += "<tr>";

        for (int i = 0; i < dt.Columns.Count; i++)
        {
            html += "<td>" + dt.Columns[i].ColumnName + "</td>";
        }

        html += "</tr>";

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            html += "<tr>";
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                html += "<td>" + dt.Rows[i][j].ToString() + "</td>";
            }

            html += "</tr>";
        }

        html += "</table>";

        return html;
    }

    //public static string DataTableToJson(DataTable dtTable)
    //{
    //    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
    //    List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

    //    Dictionary<string, object> row;
    //    foreach (DataRow oRow in dtTable.Rows)
    //    {
    //        row = new Dictionary<string, object>();
    //        foreach (DataColumn col in dtTable.Columns)
    //        {
    //            row.Add(col.ColumnName, oRow[col]);
    //        }

    //        rows.Add(row);
    //    }

    //    return serializer.Serialize(rows);
    //}

    //public static string ObjectToJson(object oObject)
    //{
    //    JavaScriptSerializer serializer = new JavaScriptSerializer();

    //    return serializer.Serialize(oObject);
    //}

    public static string ObjectToBinary(object oBj)
    {
        MemoryStream memoStream = new MemoryStream();
        BinaryFormatter bf = new BinaryFormatter();

        bf.Serialize(memoStream, oBj);

        memoStream.Flush();
        memoStream.Position = 0;

        return Convert.ToBase64String(memoStream.ToArray());
    }

    //public static T JsonToObject<T>(T Object, string jsonString)
    //{
    //    JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();

    //    var oX = (T)jsonSerializer.Deserialize<T>(jsonString);

    //    return oX;
    //}

    public static void SetBitOnOff(ref int value, int bitPosition, bool turnOn)
    {
        int i = 0;
        //bitPosition = bitPosition - 1;

        if (turnOn)
        {
            i = TurnBitOn(value, bitPosition);
        }
        else
        {
            i = TurnBitOff(value, bitPosition);
        }

        if (Debugger.IsAttached)
        {
            GetIntBinaryString(i);
        }

        value = i;

        //return i;
    }

    private static int TurnBitOn(int value, int bitPosition)
    {
        return (value | (1 << bitPosition));
    }

    private static int TurnBitOff(int value, int bitPosition)
    {
        return (value & ~(1 << bitPosition));
    }

    public static int FlipBit(int value, int bitToFlip)
    {
        return (value ^ bitToFlip);
    }

    public static bool BitIsOn(int value, int bitPosition)
    {
        bool bitOn = false;

        if ((value & (1 << bitPosition)) != 0)
        {
            bitOn = true;
        }

        return bitOn;
    }

    private static string GetIntBinaryString(int value)
    {
        string str = "";

        char[] b = new char[32];
        int pos = 31;
        int i = 0;

        while (i < 32)
        {
            if ((i % 8) == 0 & i != 0)
            {
                str = "-" + str;
            }

            if ((value & (1 << i)) != 0)
            {
                str = "1" + str;

                //b[pos] = '1';
            }
            else
            {
                str = "0" + str;
                //b[pos] = '0';
            }

            pos--;
            i++;
        }

        Debug.WriteLine(str + " (" + value + ")");

        return str;
    }

    public static string EncodeToBase64(string toEncode)
    {
        byte[] toEncodeAsBytes = System.Text.UTF8Encoding.UTF8.GetBytes(toEncode);
        string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);

        return returnValue;
    }

    public static string DecodeFromBase64(string toDecode)
    {
        byte[] encodedDataAsBytes = System.Convert.FromBase64String(toDecode);
        string returnValue = System.Text.UTF8Encoding.UTF8.GetString(encodedDataAsBytes);

        return returnValue;
    }

    public static string DecodeFromBase64(string toDecode, bool replace)
    {
        var x = toDecode.Replace(' ', '+');
        x = x.Replace("}", "");

        byte[] encodedDataAsBytes = System.Convert.FromBase64String(x);
        string returnValue = System.Text.UTF8Encoding.UTF8.GetString(encodedDataAsBytes);

        return returnValue;
    }


    public static string StringToCamelCase(string toCamelCase)
    {
        TextInfo textInfo = new CultureInfo("es-MX", false).TextInfo;
        return textInfo.ToTitleCase(toCamelCase);
    }
}
