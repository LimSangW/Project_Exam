using System.Collections.Generic;
using System.IO;
using System.Xml;

public struct VectorInt
{
    public int[] vectorIntArray;

    public VectorInt(int length)
    {
        if (length < 0)
            length = 0;

        vectorIntArray = new int[length];
    }

    public override string ToString()
    {
        return "(" + string.Join(", ", vectorIntArray) + ")";
    }

    public void CopyFrom(VectorInt vectorInt)
    {
        this.vectorIntArray = vectorInt.vectorIntArray;
    }

    public void Set(int index, int value)
    {
        vectorIntArray[index] = value;
    }

    public List<int> GetList(int startIndex)
    {
        List<int> list = new List<int>();
        for (int i = startIndex; i < vectorIntArray.Length; i++)
        {
            list.Add(vectorIntArray[i]);
        }
        return list;
    }

    public static VectorInt zero = new VectorInt();
}

public static class VectorIntHelper
{
    public static bool Parse(string packed, ref VectorInt value)
    {
        string result = packed;

        string[] group = new string[] { "(", ")" };
        foreach (var g in group)
        {
            result = result.Replace(g, string.Empty);
        }

        string[] items = result.Split(DelimiterHelper.Comma);

        value = VectorInt.zero;
        try
        {
            value.vectorIntArray = new int[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                int resultValue = 0;
                if (int.TryParse(items[i], out resultValue))
                {
                    value.Set(i, resultValue);
                }
                else
                {
                    return false;
                }
            }
        }
        catch (System.Exception e)
        {
            DebugEx.Log($"Exception Message : {e.Message}");
            return false;
        }

        return true;
    }

    public static void WriteBinary(BinaryWriter writer, VectorInt[] value)
    {
        writer.Write(value.Length);
        for (int i = 0; i < value.Length; i++)
        {
            writer.Write(value[i].vectorIntArray.Length);
            for (int j = 0; j < value[i].vectorIntArray.Length; j++)
            {
                writer.Write(value[i].vectorIntArray[j]);
            }
        }
    }

    public static bool ReadBinary(BinaryReader reader, ref VectorInt value)
    {
        int innerLength = reader.ReadInt32();
        VectorInt vectorInt = new VectorInt(innerLength);
        for (int i = 0; i < innerLength; i++)
        {
            vectorInt.Set(i, reader.ReadInt32());
        }
        value = vectorInt;
        return true;
    }

    public static bool ReadXML(XmlReader reader, string desc, string elementName, ref VectorInt value)
    {
        string result = string.Empty;
        bool readResult = true;
        reader.ReadStartElement(elementName);
        readResult &= XmlHelper.ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();
        readResult &= VectorIntHelper.Parse(result, ref value);
        return readResult;
    }
}