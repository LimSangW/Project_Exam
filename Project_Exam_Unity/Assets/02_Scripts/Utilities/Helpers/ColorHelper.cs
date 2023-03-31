using System.Xml;
using System.IO;
using UnityEngine;

public class ColorHelper
{
    public static string Pack(Color value)
    {
        return string.Format("({0}, {1}, {2}, {3})", value.r, value.g, value.b, value.a);
    }

    public static string Pack(Color32 value)
    {
        return string.Format("({0}, {1}, {2}, {3})", value.r, value.g, value.b, value.a);
    }

    public static bool Parse(string packed, ref Color value)
    {
        string result = packed;
        value = Color.white;

        string[] group = new string[] { "(", ")" };
        foreach (var g in group)
        {
            result = result.Replace(g, string.Empty);
        }

        string[] items = result.Split(DelimiterHelper.Comma);
        if (items.Length != 4)
        {
            DebugEx.Log(string.Format("[Failed] invalid Color string: {0}", packed));
            return false;
        }

        try
        {
            value = new Color(float.Parse(items[0]), float.Parse(items[1]), float.Parse(items[2]), float.Parse(items[3]));
        }
        catch (System.Exception e)
        {
            DebugEx.Log(string.Format("[Failed] invalid Color string: {0}, \nerror Message:{1}",
                                          packed, e.Message));
            return false;
        }

        return true;
    }

    public static bool ParseColor32(string packed, ref Color32 value)
    {
        string result = packed;

        string[] group = new string[] { "(", ")" };
        foreach (var g in group)
        {
            result = result.Replace(g, string.Empty);
        }

        string[] items = result.Split(DelimiterHelper.Comma);
        if (items.Length != 4)
        {
            DebugEx.Log(string.Format("[Failed] invalid Color32 string: {0}", packed));
            return false;
        }

        try
        {
            value = new Color32(byte.Parse(items[0]), byte.Parse(items[1]), byte.Parse(items[2]), byte.Parse(items[3]));
        }
        catch (System.Exception e)
        {
            DebugEx.Log(string.Format("[Failed] invalid Color32 string: {0}, \nerror Message:{1}",
                                          packed, e.Message));
            return false;
        }

        return true;
    }

    public static bool ReadXML(XmlReader reader, string desc, string elementName, ref Color value)
    {
        string result = string.Empty;
        bool readResult = true;
        reader.ReadStartElement(elementName);
        readResult &= XmlHelper.ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();
        if (result.StartsWith("#"))
        {
            Color color = Color.white;
            ColorUtility.TryParseHtmlString(result, out color);
            value = color;
        }
        else
        {
            value = Color.black;
            readResult &= ColorHelper.Parse(result, ref value);
        }
        return readResult;
    }

    public static bool ReadXML(XmlReader reader, string desc, string elementName, ref Color32 value)
    {
        string result = string.Empty;
        bool readResult = true;
        reader.ReadStartElement(elementName);
        readResult &= XmlHelper.ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();
        if (result.StartsWith("#"))
        {
            Color color = Color.white;
            ColorUtility.TryParseHtmlString(result, out color);
            value = color;
        }
        else
        {
            value = Color.black;
            readResult &= ColorHelper.ParseColor32(result, ref value);
        }
        return readResult;
    }

    public static bool ReadBinary(System.IO.BinaryReader reader, ref Color result)
    {
        for (int i = 0; i < 4; i++)
        {
            result[i] = reader.ReadSingle();
        }
        return true;
    }

    public static void Write(BinaryWriter writer, Color color)
    {
        writer.Write(color.r);
        writer.Write(color.g);
        writer.Write(color.b);
        writer.Write(color.a);
    }
}
