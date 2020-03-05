using Newtonsoft.Json;
using System;
using System.Text;

/// <summary>
/// Summary description for Extension
/// </summary>
public interface ISerializer
{

}
internal static class Extension
{

    public static string Data { get; set; }
    internal static object Serialize(this ISerializer ser)
    {
        return JsonConvert.SerializeObject(ser, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        });
    }
    internal static T Deserialize<T>(this string str)
    {

        return JsonConvert.DeserializeObject<T>(str, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        });
    }
    internal static byte[] ToByte(this string str)
    {
        return UTF8Encoding.UTF8.GetBytes(str);
    }
    internal static int ParseInt(this object value)
    {
        return Convert.ToInt32(value);
    }
    internal static long ParseLongInt(this object value)
    {
        return Convert.ToInt64(value);
    }
    internal static decimal ParseDecimal(this object value)
    {
        return Convert.ToDecimal(value);
    }
    internal static double ParseDouble(this object value)
    {
        return Convert.ToDouble(value);
    }
    internal static bool IsNull(this object value)
    {
        if (value == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    internal static string ToDate(this object value, int format)
    {
        switch (format)
        {
            case 0:
                return Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss").Trim();
            case 1:
                return Convert.ToDateTime(value).ToString("MM").Trim();
            case 2:
                return Convert.ToDateTime(value).ToString("yyyy").Trim();
            case 3:
                return Convert.ToDateTime(value).ToString("yyyy-MM-dd").Trim();
            case 4:
                return Convert.ToDateTime(value).ToString("MMMM dd, yyyy").Trim();
            default:
                return "0000-00-0000";
        }
    }

}