using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using System.Collections.Generic;

public static class Vector3Helper
{
    public static Vector2 GetVector2XY(this Vector3 vec3)
    {
        return new Vector2(vec3.x, vec3.y);
    }

    public static Vector2 GetVector2XZ(this Vector3 vec3)
    {
        return new Vector2(vec3.x, vec3.z);
    }

    public static Vector3 GetVelocity(Vector3 origin, Vector3 destination, float speed)
    {
        return ((destination - origin).normalized * speed);
    }

    public static Vector3 GetVelocityDuring(Vector3 origin, Vector3 destination, float duration)
    {
        return ((destination - origin) / duration);
    }

    public static Vector3 Move(Vector3 current, Vector3 target, Vector3 velocity, float deltaTime)
    {
        current += velocity * deltaTime;

        for (int i = 0; i < 3; i++)
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

    public static string Pack(Vector3 value)
    {
        return string.Format("({0}, {1}, {2})", value.x, value.y, value.z);
    }

    public static string TransToString(Vector3 value)
    {
        return string.Format("({0:0.##}, {1:0.##}, {2:0.##})", value.x, value.y, value.z);
    }

    public static bool Parse(string packed, ref Vector3 value)
    {
        string result = packed;
        value = Vector3.zero;

        string[] group = new string[] { "(", ")" };
        foreach (var g in group)
        {
            result = result.Replace(g, string.Empty);
        }

        string[] items = result.Split(DelimiterHelper.Comma);
        if (items.Length != 3)
        {
            DebugEx.LogError(string.Format("[Failed] invalid Vector3 string: {0}", packed));
            return false;
        }

        try
        {
            value.Set(float.Parse(items[0]), float.Parse(items[1]), float.Parse(items[2]));
        }
        catch (System.Exception e)
        {
            DebugEx.LogError(string.Format("[Failed] invalid Vector3 string: {0}, \nerror Message:{1}",
                                          packed, e.Message));
            return false;
        }
        return true;
    }

    public static void WriteBinary(System.IO.BinaryWriter writer, Vector3 value)
    {
        writer.Write(value.x);
        writer.Write(value.y);
        writer.Write(value.z);
    }

    public static bool ReadBinary(System.IO.BinaryReader reader, ref Vector3 result)
    {
        for (int i = 0; i < 3; i++)
        {
            result[i] = reader.ReadSingle();
        }
        return true;
    }

    public static bool ReadXML(XmlReader reader, string desc, string elementName, ref Vector3 value)
    {
        string result = string.Empty;
        bool readResult = true;
        reader.ReadStartElement(elementName);
        readResult &= XmlHelper.ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();
        value = Vector3.zero;
        readResult &= Vector3Helper.Parse(result, ref value);
        return readResult;
    }

    public static void SetPositiveRotationValue(ref Vector3 value)
    {
        if (value.x < 0f)
        {
            while (true)
            {
                value.x += 360;
                if (value.x >= 0f)
                    break;
            }
        }

        if (value.y < 0f)
        {
            while (true)
            {
                value.y += 360;
                if (value.y >= 0f)
                    break;
            }
        }

        if (value.z < 0f)
        {
            while (true)
            {
                value.z += 360;
                if (value.z >= 0f)
                    break;
            }
        }
    }

    public static float[] GetFloatArray(Vector3[] vectors)
    {
        int length = (vectors == null) ? 0 : vectors.Length * 3;
        float[] array = new float[length + 1];
        array[0] = length;
        int j = 1;

        for (int i = 0; i < vectors.Length; ++i)
        {
            array[j] = vectors[i].x;
            ++j;
            array[j] = vectors[i].y;
            ++j;
            array[j] = vectors[i].z;
            ++j;
        }
        return array;
    }

    public static byte[] GetByteArray(Vector3[] vectors)
    {
        int length = (vectors == null) ? 0 : vectors.Length;
        var list = new List<byte>();
        list.AddRange(BitConverter.GetBytes(length));
        for (int i = 0; i < vectors.Length; ++i)
        {
            list.AddRange(BitConverter.GetBytes(vectors[i].x));
            list.AddRange(BitConverter.GetBytes(vectors[i].y));
            list.AddRange(BitConverter.GetBytes(vectors[i].z));
        }
        return list.ToArray();
    }
}
