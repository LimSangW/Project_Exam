using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using System.Collections.Generic;

public struct Vector2String
{
    public string x;
    public string y;

    public Vector2String(string x, string y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return string.Format("({0}, {1})", x, y);
    }

    public string this[int index]
    {
        get
        {
            switch (index)
            {
                case 0:
                    return x;

                case 1:
                    return y;

                default:
                    throw new System.IndexOutOfRangeException();
            }
        }

        set
        {
            switch (index)
            {
                case 0:
                    x = value;
                    break;

                case 1:
                    y = value;
                    break;

                default:
                    throw new System.IndexOutOfRangeException();
            }
        }
    }

    public void Set(string x, string y)
    {
        this.x = x;
        this.y = y;
    }

    public static Vector2String zero = new Vector2String(string.Empty, string.Empty);
}

public static class Vector2Helper
{
    public static Vector2 GetVelocity(Vector2 origin, Vector2 destination, float speed)
    {
        return ((destination - origin).normalized * speed);
    }

    public static Vector3 GetVelocityDuring(Vector2 origin, Vector2 destination, float duration)
    {
        return ((destination - origin) / duration);
    }

    public static Vector3 Move(Vector2 current, Vector2 target, Vector2 velocity, float deltaTime)
    {
        current += velocity * deltaTime;
        for (int i = 0; i < 2; i++)
        {
            if (velocity[i] > 0.0f)
            {
                current[i] = current[i] > target[i] ? target[i] : current[i];
            }
            else if (velocity[i] < 0.0f)
            {
                current[i] = current[i] < target[i] ? target[i] : current[i];
            }
        }
        return current;
    }

    public static string Pack(Vector2 value)
    {
        return string.Format("({0}, {1})", value.x, value.y);
    }

    public static bool Parse(string packed, ref Vector2 value)
    {
        string result = packed;
        value = Vector2.zero;

        string[] group = new string[] { "(", ")" };
        foreach (var g in group)
        {
            result = result.Replace(g, string.Empty);
        }

        string[] items = result.Split(DelimiterHelper.Comma);
        if (items.Length != 2)
        {
            DebugEx.Log(string.Format("[Failed] invalid Vector2 string: {0}", packed));
            return false;
        }

        try
        {
            value.Set(float.Parse(items[0]), float.Parse(items[1]));
        }
        catch (System.Exception e)
        {
            DebugEx.Log(string.Format("[Failed] invalid Vector2 string: {0}, \nerrorMessage:{1}",
                                          packed, e.Message));
            return false;
        }
        return true;
    }

    public static bool Parse(string packed, ref Vector2Int value)
    {
        string result = packed;
        value = Vector2Int.zero;

        string[] group = new string[] { "(", ")" };
        foreach (var g in group)
        {
            result = result.Replace(g, string.Empty);
        }

        string[] items = result.Split(DelimiterHelper.Comma);
        if (items.Length != 2)
        {
            DebugEx.Log(string.Format("[Failed] invalid Vector2Int string: {0}", packed));
            return false;
        }

        try
        {
            value.Set(int.Parse(items[0]), int.Parse(items[1]));
        }
        catch (System.Exception e)
        {
            DebugEx.Log(string.Format("[Failed] invalid Vector2Int string: {0}, \nerrorMessage:{1}",
                                          packed, e.Message));
            return false;
        }
        return true;
    }

    public static bool Parse(string packed, ref Vector2String value)
    {
        string result = packed;
        value = Vector2String.zero;

        string[] group = new string[] { "(", ")" };
        foreach (var g in group)
        {
            result = result.Replace(g, string.Empty);
        }

        string[] items = result.Split(DelimiterHelper.Comma);
        if (items.Length != 2)
        {
            DebugEx.Log(string.Format("[Failed] invalid Vector2String string: {0}", packed));
            return false;
        }

        try
        {
            value.Set(items[0], items[1]);
        }
        catch (System.Exception e)
        {
            DebugEx.Log(string.Format("[Failed] invalid Vector2String string: {0}, \nerrorMessage:{1}",
                                          packed, e.Message));
            return false;
        }
        return true;
    }

    public static void WriteBinary(System.IO.BinaryWriter writer, Vector2 value)
    {
        writer.Write(value.x);
        writer.Write(value.y);
    }

    public static void WriteBinary(System.IO.BinaryWriter writer, Vector2[] values)
    {
        writer.Write(values.Length);
        for (int i = 0; i < values.Length; i++)
        {
            writer.Write(values[i].x);
            writer.Write(values[i].y);
        }
    }

    public static bool ReadBinary(System.IO.BinaryReader reader, ref Vector2 result)
    {
        for (int i = 0; i < 2; i++)
        {
            result[i] = reader.ReadSingle();
        }
        return true;
    }

    public static bool ReadBinary(System.IO.BinaryReader reader, ref Vector2[] results)
    {
        int count = reader.ReadInt32();
        results = new Vector2[count];
        for (int i = 0; i < results.Length; i++)
        {
            results[i].x = reader.ReadSingle();
            results[i].y = reader.ReadSingle();
        }
        return true;
    }

    public static bool ReadXML(XmlReader reader, string desc, string elementName, ref Vector2 value)
    {
        string result = string.Empty;
        bool readResult = true;
        reader.ReadStartElement(elementName);
        readResult &= XmlHelper.ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();
        value = Vector2.zero;
        readResult &= Vector2Helper.Parse(result, ref value);
        return readResult;
    }

    public static bool ReadXML(XmlReader reader, string desc, string elementName, ref Vector2Int value)
    {
        string result = string.Empty;
        bool readResult = true;
        reader.ReadStartElement(elementName);
        readResult &= XmlHelper.ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();
        value = Vector2Int.zero;
        readResult &= Vector2Helper.Parse(result, ref value);
        return readResult;
    }

    public static bool ReadXML(XmlReader reader, string desc, string elementName, ref Vector2String value)
    {
        string result = string.Empty;
        bool readResult = true;
        reader.ReadStartElement(elementName);
        readResult &= XmlHelper.ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();
        value = Vector2String.zero;
        readResult &= Vector2Helper.Parse(result, ref value);
        return readResult;
    }

    public static float[] GetFloatArray(Vector2[] vectors)
    {
        int length = (vectors == null) ? 0 : vectors.Length * 2;
        float[] array = new float[length + 1];
        array[0] = length;
        int j = 1;
        for (int i = 0; i < vectors.Length; ++i)
        {
            array[j] = vectors[i].x;
            ++j;
            array[j] = vectors[i].y;
            ++j;
        }
        return array;
    }

    public static byte[] GetByteArray(Vector2[] vectors)
    {
        int length = (vectors == null) ? 0 : vectors.Length;
        var list = new List<byte>();
        list.AddRange(BitConverter.GetBytes(length));
        for (int i = 0; i < vectors.Length; ++i)
        {
            list.AddRange(BitConverter.GetBytes(vectors[i].x));
            list.AddRange(BitConverter.GetBytes(vectors[i].y));
        }
        return list.ToArray();
    }
}
