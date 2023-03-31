using System.Collections.Generic;
using System.IO;
using System.Xml;

public struct VectorFloat
{
    public float[] vectorFloatArray;

    public VectorFloat(int length)
    {
        if (length < 0)
            length = 0;

        vectorFloatArray = new float[length];
    }

    public override string ToString()
    {
        return "(" + string.Join(", ", vectorFloatArray) + ")";
    }

    public void CopyFrom(VectorFloat vectorInt)
    {
        this.vectorFloatArray = vectorInt.vectorFloatArray;
    }

    public void Set(int index, float value)
    {
        vectorFloatArray[index] = value;
    }

    public List<float> GetList(int startIndex)
    {
        List<float> list = new List<float>();
        for (int i = startIndex; i < vectorFloatArray.Length; i++)
        {
            list.Add(vectorFloatArray[i]);
        }
        return list;
    }

    public static VectorFloat zero = new VectorFloat();
}

public static class VectorFloatHelper
{
    public static bool Parse(string packed, ref VectorFloat value)
    {
        string result = packed;

        string[] group = new string[] { "(", ")" };
        foreach (var g in group)
        {
            result = result.Replace(g, string.Empty);
        }

        string[] items = result.Split(DelimiterHelper.Comma);

        value = VectorFloat.zero;
        try
        {
            value.vectorFloatArray = new float[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                float resultValue = 0;
                if (float.TryParse(items[i], out resultValue))
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

    public static void WriteBinary(BinaryWriter writer, VectorFloat[] value)
    {
        writer.Write(value.Length);
        for (int i = 0; i < value.Length; i++)
        {
            writer.Write(value[i].vectorFloatArray.Length);
            for (int j = 0; j < value[i].vectorFloatArray.Length; j++)
            {
                writer.Write(value[i].vectorFloatArray[j]);
            }
        }
    }

    public static bool ReadBinary(BinaryReader reader, ref VectorFloat value)
    {
        int innerLength = reader.ReadInt32();
        VectorFloat vectorFloat = new VectorFloat(innerLength);
        for (int i = 0; i < innerLength; i++)
        {
            vectorFloat.Set(i, reader.ReadFloat());
        }
        value = vectorFloat;
        return true;
    }

    public static bool ReadXML(XmlReader reader, string desc, string elementName, ref VectorFloat value)
    {
        string result = string.Empty;
        bool readResult = true;
        reader.ReadStartElement(elementName);
        readResult &= XmlHelper.ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();
        readResult &= VectorFloatHelper.Parse(result, ref value);
        return readResult;
    }
}