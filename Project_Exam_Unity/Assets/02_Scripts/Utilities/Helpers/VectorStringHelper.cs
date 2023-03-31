using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEngine;

public struct VectorString
{
    public string[] vectorStr;

    public VectorString(int length)
    {
        if (length < 0)
            length = 0;

        vectorStr = new string[length];
    }

    public override string ToString()
    {
        return "(" + string.Join(", ", vectorStr) + ")";
    }

    public void Set(int index, string value)
    {
        vectorStr[index] = value;
    }

    public List<string> GetList(int startIndex)
    {
        List<string> list = new List<string>();
        for (int i = startIndex; i < vectorStr.Length; i++)
        {
            list.Add(vectorStr[i]);
        }
        return list;
    }

    public static VectorString empty = new VectorString(0);
}

public static class VectorStringHelper
{
    public static bool Parse(string packed, ref VectorString vectorString)
    {
        string result = packed;

        string[] group = new string[] { "(", ")" };
        foreach (var g in group)
        {
            result = result.Replace(g, string.Empty);
        }

        string[] items = result.Split(DelimiterHelper.Comma);
        try
        {
            vectorString.vectorStr = new string[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                vectorString.vectorStr[i] = items[i];
            }
        }
        catch (System.Exception e)
        {
            DebugEx.Log($"Exception Message : {e.Message}");
            return false;
        }

        return true;
    }

    public static void WriteBinary(BinaryWriter writer, VectorString vectorString)
    {
        writer.Write(vectorString.vectorStr.Length);
        for (int i = 0; i < vectorString.vectorStr.Length; i++)
        {
            writer.Write(vectorString.vectorStr[i]);
        }
    }

    public static void WriteBinary(BinaryWriter writer, VectorString[] vectorStringArr)
    {
        writer.Write(vectorStringArr.Length);
        for (int i = 0; i < vectorStringArr.Length; i++)
        {
            WriteBinary(writer, vectorStringArr[i]);
        }
    }

    public static bool ReadBinary(BinaryReader reader, ref VectorString result)
    {
        int count = reader.ReadInt32();
        VectorString vectorString = new VectorString(count);
        for (int i = 0; i < count; i++)
        {
            vectorString.Set(i, reader.ReadString());
        }
        result = vectorString;
        return true;
    }

    public static bool ReadBinary(BinaryReader reader, ref VectorString[] result)
    {
        int count = reader.ReadInt32();
        result = new VectorString[count];
        for (int i = 0; i < count; i++)
        {
            ReadBinary(reader, ref result[i]);
        }

        return true;
    }

    public static bool ReadXML(XmlReader reader, string desc, string elementName, ref VectorString value)
    {
        string result = string.Empty;
        bool readResult = true;
        reader.ReadStartElement(elementName);
        readResult &= XmlHelper.ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();
        readResult &= VectorStringHelper.Parse(result, ref value);
        return readResult;
    }
}

