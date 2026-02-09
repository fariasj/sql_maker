using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SqlV2;

public static class DAExtensions
{
    public static bool IsGenericList(this object o)
    {
        var oType = o.GetType();
        return (oType.IsGenericType && (oType.GetGenericTypeDefinition() == typeof(List<>)));
    }

    public static string FormatToHml(this string value)
    {
        value = value.Replace("<", "&#lt;");
        value = value.Replace(">", "&gt;");
        value = value.Replace("&", "&amp;");
        value = value.Replace("'", "&apos;");
        value = value.Replace("¢", "&cent;");
        value = value.Replace("£", "&pound;");
        value = value.Replace("¥", "&yen;");
        value = value.Replace("€", "&euro;");
        value = value.Replace("©", "&copy;");
        value = value.Replace("®", "&reg;");

        return value;
    }


    public static string RemoveString(this string value, char caracterInicial, char caracterFinal)
    {
        int start = value.IndexOf(caracterInicial) + 1;
        int end = value.IndexOf(caracterFinal, start);
        string result = "";

        if (end < start)
        {
            return value;
        }

        result = value.Substring(start, end - start);

        return value.Replace(caracterInicial + result + caracterFinal, "");
    }

    public static string ToDescription(this Enum value)
    {
        var da = (DescriptionAttribute[])(value.GetType().GetField(value.ToString())?.GetCustomAttributes(typeof(DescriptionAttribute), false) ?? Array.Empty<DescriptionAttribute>());
        return da.Length > 0 ? da[0].Description : value.ToString();
    }

    public static string Reverse(this string s)
    {
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }

    public static object? GetDefaultValue(this Type t)
    {
        if (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
            return Activator.CreateInstance(t);
        else
            return null;
    }

    public static T? Parse<T>(this string value)
    {
        T? result = default(T);

        if (!string.IsNullOrEmpty(value))
        {
            TypeConverter tc = TypeDescriptor.GetConverter(typeof(T));
            result = (T?)tc.ConvertFrom(value);
        }

        return result;
    }

    public static bool IsNumber(this string x)
    {
        try
        {
            int.Parse(x);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsNullOrEmpty(this string? x)
    {
        return string.IsNullOrEmpty(x);
    }

    public static bool IsDateTime(this string x)
    {
        try
        {
            DateTime.Parse(x);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static string ToLetters(this int value)
    {
        string result = string.Empty;

        while (--value >= 0)
        {
            result = (char)('A' + value % 26) + result;
            value /= 26;
        }

        return result;
    }

    //public static string ToSqlNumber(this int x)
    //{
    //    return x.Replace(",", "").Replace("$", "");
    //}

    public static string ToSqlNumber(this string x)
    {
        return x.Replace(",", "").Replace("$", "");
    }

    public static string ToSqlString(this string? x)
    {
        if ((x == null))
        {
            return " null ";
        }
        else
        {
            return "'" + x.Replace("'", "''") + "'";
        }
    }

    public static string ToSqlString(this string x, int longitudMaxima)
    {
        int longMax;

        longMax = longitudMaxima;

        if (longitudMaxima > x.Length)
        {
            longMax = x.Length;
        }

        return x.Substring(0, longMax).ToSqlString();
    }

    public static string ToSqlDate(this DateTime x)
    {
        return "'" + x.ToString("yyyyMMdd") + "'";
    }

    public static string ToSqlDateTime(this DateTime x)
    {
        return "'" + x.ToString("yyyyMMdd HH:mm:ss.fff") + "'";
    }

    public static string ToBase64(this string x)
    {
        return DAUtileriasSistema.EncodeToBase64(x);
    }

    public static string FromBase64(this string x)
    {
        return DAUtileriasSistema.DecodeFromBase64(x, true);
    }

    public static string ToTitleCase(this string? mText)
    {
        if (mText == null) return mText;

        System.Globalization.CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
        System.Globalization.TextInfo textInfo = cultureInfo.TextInfo;

        // TextInfo.ToTitleCase only operates on the string if is all lower case, otherwise it returns the string unchanged.
        return textInfo.ToTitleCase(mText.ToLower()).Replace("_", "");
    }

    public static string AddSpacesToSentence(this string text, bool preserveAcronyms)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        StringBuilder newText = new StringBuilder(text.Length * 2);
        newText.Append(text[0]);
        for (int i = 1; i < text.Length; i++)
        {
            if (char.IsUpper(text[i]))
                if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                    (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                     i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                    newText.Append(' ');
            newText.Append(text[i]);
        }
        return newText.ToString();
    }

    public static bool IsBetween<T>(this T number, T minNumber, T maxNumber) where T : IComparable<T>
    {
        return number.CompareTo(minNumber) >= 0 && number.CompareTo(maxNumber) < 0;
    }

    public static XmlDocument? ToXml<T>(this T obj)
    {
        try
        {
            return DAUtileriasSistema.ObjectToXml(obj, null, true);
        }
        catch
        {
            return null;
        }
    }

    public static string ToJSon<T>(this T obj)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };
        string json = JsonSerializer.Serialize(obj, options);
        return json;
    }

    public static T? ToObject<T>(this string x)
    {
        return DAUtileriasSistema.XmlToObject<T>(x);
    }

    public static T? FromJson<T>(this string x)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        return JsonSerializer.Deserialize<T>(x, options);
    }

    public static XmlDocument? ToXml<T>(this T obj, XmlSerializerNamespaces? nameSpaces)
    {
        return DAUtileriasSistema.ObjectToXml(obj, nameSpaces);
    }

    public static string Transform(this XmlDocument xmlDocument, string stringXSLT)
    {
        return DAUtileriasSistema.TransformXml(stringXSLT, xmlDocument.OuterXml);
    }

    public static T? GetValue<T>(this DataRow r, string columnName)
    {
        try
        {
            var x = (T)r[columnName];
            return x;
        }
        catch
        {
            return default(T);
        }
    }

    private static string GetSubstringByString(string a, string b, string c)
    {
        if (c.IsNullOrEmpty())
        {
            return "";
        }

        if (!c.Contains(a))
        {
            return c;
        }

        if (!c.Contains(b))
        {
            return c;
        }

        return c.Substring((c.IndexOf(a) + a.Length), (c.IndexOf(b) - c.IndexOf(a) - a.Length));
    }

    public static string ToHTML(this DataTable dt, string tituloX, string tituloY)
    {
        string html = "<table class=\"hoverTable\">";

        var xT = GetSubstringByString("(", ")", tituloX);
        var yT = GetSubstringByString("(", ")", tituloY);

        tituloY = tituloY.Replace("(" + yT + ")", "");
        tituloX = tituloX.Replace("(" + xT + ")", "");

        //html += "<th colspan=\"2\">";
        html += "	<tr>";
        html += "		<td class=\"titleX\"/>";
        html += "		<th colspan=" + (dt.Columns.Count - 1) + ">";
        html += "			<b>" + (tituloX.IsNullOrEmpty() ? "" : tituloX + " / ") + (tituloY.IsNullOrEmpty() ? "" : tituloY) + " </b>";
        html += "		</th>";
        html += "	</tr>";
        //html += "</th>";


        html += "<tr>";
        for (int i = 0; i < dt.Columns.Count; i++)
        {
            if (i == 0)
            {
                var gT = GetSubstringByString("(", ")", dt.Columns[i].ColumnName);
                var tituloG = dt.Columns[i].ColumnName.Replace("(" + gT + ")", "");

                html += "<td class=\"titleY\" style=\"width:\"" + dt.Columns[i].ColumnName.Length + "\"><b>" + tituloG + "</b></td>";
            }
            else
            {
                html += "<td class=\"titleAnswers\"><b>" + dt.Columns[i].ColumnName + "</b></td>";
            }
        }

        html += "</tr>";

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            html += "<tr>";
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                if (j == 0)
                {
                    html += "<td>" + dt.Rows[i][j].ToString() + "</td>";
                }
                else
                {
                    html += "<td align=\"right\">" + dt.Rows[i][j].ToString().Parse<int>().ToString("##,##0") + "</td>";
                }
            }

            html += "</tr>";
        }

        html += "</table>";

        return html;
    }

    //public static string Transform(this XmlDocument xmlDocument, string stringXSLT)
    //{
    //    return "";
    //}

    //public static string ToJSon<T>(this T obj)
    //{
    //    return DAUtileriasSistema.ObjectToJson(obj);
    //}
}
