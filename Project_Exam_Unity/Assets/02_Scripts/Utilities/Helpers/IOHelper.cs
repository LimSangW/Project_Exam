using System.Collections.Generic;
using System.IO;
using System;
using System.Xml;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public static class IOHelper
{
    const int ArrayLengthLimit = 512;

    public static bool IsEOF(TextReader reader, string desc)
    {
        if (reader.Peek() == -1)
        {
            if (string.IsNullOrEmpty(desc))
            {
                DebugEx.Log("[EOF]");
            }
            else
            {
                DebugEx.Log("[EOF] " + desc);
            }
            return true;
        }
        return false;
    }

    public static bool IsEOF(BinaryReader reader, string desc)
    {
        if (reader.PeekChar() == -1)
        {
            if (string.IsNullOrEmpty(desc))
            {
                DebugEx.Log("[EOF]");
            }
            else
            {
                DebugEx.Log("[EOF] " + desc);
            }
            return true;
        }
        return false;
    }

    public static string[] GetAssetPath(string path)
    {
        var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            var start = files[i].IndexOf("Assets/");
            files[i] = files[i].Substring(start);
        }
        return files;
    }

    public static byte[] GetBytesOrNull(Action<BinaryWriter> serializeAction)
    {
        try
        {
            using (BinaryWriter writer = new BinaryWriter(new MemoryStream()))
            {
                serializeAction(writer);
                return ((MemoryStream)writer.BaseStream).GetBuffer();
            }
        }
        catch (Exception e)
        {
            DebugEx.Log(e.Message);
            return null;
        }
    }

    public static void GetData(byte[] bytes, Action<BinaryReader> deserializeAction)
    {
        if (bytes.Length <= 0)
            return;

        try
        {
            using (BinaryReader reader = new BinaryReader(new MemoryStream(bytes)))
            {
                deserializeAction(reader);
            }
        }
        catch (Exception e)
        {
            DebugEx.Log("[Failed] IOHelper.GetData: ");
            DebugEx.Log(e.Message);
        }
    }

    public static bool Read<T>(TextReader reader, string desc, ref T result)
    {
        if (IsEOF(reader, desc))
        {
            return false;
        }
        result = (T)System.Convert.ChangeType(reader.ReadLine(), typeof(T));

        return true;
    }

    public static bool Read2<T>(TextReader reader, string desc, ref T result)
    {
        if (IsEOF(reader, desc))
        {
            return false;
        }
        result = (T)System.Convert.ChangeType(reader.ReadLine(), typeof(T));

        return true;
    }

    public static bool Read<T>(TextReader reader, ref T result)
    {
        return Read<T>(reader, string.Empty, ref result);
    }

    public static void WriteColor(TextWriter writer, Color color)
    {
        for (int i = 0; i < 4; i++)
        {
            writer.WriteLine(color[i]);
        }
    }

    public static bool ReadColor(TextReader reader, string desc, ref Color result)
    {
        if (IsEOF(reader, desc))
        {
            return false;
        }

        for (int i = 0; i < 4; i++)
        {
            result[i] = float.Parse(reader.ReadLine());
        }
        return true;
    }

    public static bool ReadColor(TextReader reader, ref Color result)
    {
        return ReadColor(reader, string.Empty, ref result);
    }

    public static void WriteArray<T>(TextWriter writer, T[] array)
    {
        writer.WriteLine(array.Length);
        for (int i = 0; i < array.Length; ++i)
        {
            writer.WriteLine(array[i]);
        }
    }

    public static void WriteArray(BinaryWriter writer, string[] array)
    {
        writer.Write(array.Length);
        for (int i = 0; i < array.Length; ++i)
        {
            writer.Write(array[i]);
        }
    }

    public static void WriteArray(BinaryWriter writer, int[] array)
    {
        writer.Write(array.Length);
        for (int i = 0; i < array.Length; i++)
        {
            writer.Write(array[i]);
        }
    }

    public static void WriteArray(BinaryWriter writer, float[] array)
    {
        writer.Write(array.Length);
        for (int i = 0; i < array.Length; i++)
        {
            writer.Write(array[i]);
        }
    }

    public static void WriteArray(BinaryWriter writer, double[] array)
    {
        writer.Write(array.Length);
        for (int i = 0; i < array.Length; i++)
        {
            writer.Write(array[i]);
        }
    }

    public static bool ReadArray<T>(TextReader reader, string desc, ref T[] array)
    {
        int count = 0;
        if (!IOHelper.Read<int>(reader, "ReadArray<T>", ref count)) { return false; };
        array = new T[count];
        for (int i = 0; i < array.Length; i++)
        {
            if (!IOHelper.Read<T>(reader,
                                  string.Format("index:{0},{1}", i, desc),
                                  ref array[i]))
            {
                return false;
            }
        }
        return true;
    }

    public static bool ReadArray2<T>(string desc, string[] splits, ref T[] array)
    {
        try
        {
            int count = splits.Length;
            array = new T[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = (T)System.Convert.ChangeType(splits[i], typeof(T));
            }
        }
        catch
        {
            return false;
        }
        return true;
    }

    public static bool ReadList<T>(string desc, string[] splits, ref List<T> list)
    {
        int count = splits.Length;
        list = new List<T>(count);
        for (int i = 0; i < count; i++)
        {
            T item = (T)System.Convert.ChangeType(splits[i], typeof(T));
            list.Add(item);
        }
        return true;
    }

    public static bool ReadVector2List(string desc, string[] splits, ref List<Vector2> list)
    {
        bool result = true;
        int count = splits.Length;
        list = new List<Vector2>(count);
        for (int i = 0; i < count; i++)
        {
            Vector2 value = Vector2.zero;
            result = Vector2Helper.Parse(splits[i], ref value);
            list.Add(value);
        }
        return result;
    }

    public static bool ReadVector3List(string desc, string[] splits, ref List<Vector3> list)
    {
        bool result = true;
        int count = splits.Length;
        list = new List<Vector3>(count);
        for (int i = 0; i < count; i++)
        {
            Vector3 value = Vector3.zero;
            result = Vector3Helper.Parse(splits[i], ref value);
            list.Add(value);
        }
        return result;
    }

    public static void WriteVector3List(TextWriter writer, Vector3[] array)
    {
        for (int i = 0; i < array.Length; ++i)
        {
            writer.WriteLine(Vector3Helper.TransToString(array[i]));
        }
    }

    public static T[] ReadArray2<T>(string desc, string[] splits)
    {
        int count = splits.Length;
        T[] array = new T[count];
        for (int i = 0; i < count; i++)
        {
            array[i] = (T)System.Convert.ChangeType(splits[i], typeof(T));
        }
        return array;
    }

    public static bool ReadBinaryArray(BinaryReader reader, string desc, ref int[] array)
    {
        int count = reader.ReadInt32();
        array = new int[count];
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = reader.ReadInt32();
        }

        return true;
    }

    public static bool ReadBinaryArray(BinaryReader reader, string desc, ref float[] array)
    {
        int count = reader.ReadInt32();
        array = new float[count];
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = reader.ReadSingle();
        }

        return true;
    }

#if UNITY_EDITOR
    public static XmlReader CreateXmlReaderFromRawResources(string path)
    {
        path = "Assets/Resources/" + path + ".xml";
        var ta = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
        if (ta == null)
        {
            DebugEx.Log("[Failed] IOHelper.CreateXmlReaderFromTextAssetRaw(): " + path);
            return null;
        }
        Stream stream = new MemoryStream(ta.bytes);
        XmlReaderSettings settings = XmlHelper.GetDefaultReadSetting();
        return XmlReader.Create(stream, settings);
    }
#endif

    public static bool TryParseBool(string text, out bool value, string debugText)
    {
        if (!bool.TryParse(text, out value))
        {
            DebugEx.Log("Could not convert " + text + " to " + debugText);
            return false;
        }
        return true;
    }

    public static bool TryParseInt(string text, out int value, string debugText)
    {
        if (!int.TryParse(text, out value))
        {
            DebugEx.Log("Could not convert " + text + " to " + debugText);
            return false;
        }
        return true;
    }

    public static bool TryParseFloat(string text, out float value, string debugText)
    {
        if (!float.TryParse(text, out value))

        {
            DebugEx.Log("Could not convert " + text + " to " + debugText);
            return false;
        }
        return true;
    }

    public static T ReadTEnum<T>(this BinaryReader reader) where T : IConvertible
    {
        int intValue = reader.ReadInt32();
        //var result = (T)System.Convert.ChangeType(intValue, typeof(T));
        T result = (T)Enum.Parse(typeof(T), intValue.ToString());
        return result;
    }

    public static Color ReadColor(this BinaryReader reader)
    {
        Color color = Color.white;
        ColorHelper.ReadBinary(reader, ref color);
        return color;
    }

    public static Vector3 ReadVector2(this BinaryReader reader)
    {
        Vector2 vector2 = Vector2.zero;
        Vector2Helper.ReadBinary(reader, ref vector2);
        return vector2;
    }

    public static Vector3 ReadVector3(this BinaryReader reader)
    {
        Vector3 vector3 = Vector3.zero;
        Vector3Helper.ReadBinary(reader, ref vector3);
        return vector3;
    }

    public static VectorInt ReadVectorInt(this BinaryReader reader)
    {
        VectorInt vectorInt = VectorInt.zero;
        VectorIntHelper.ReadBinary(reader, ref vectorInt);
        return vectorInt;
    }

    public static VectorFloat ReadVectorFloat(this BinaryReader reader)
    {
        VectorFloat vectorFloat = VectorFloat.zero;
        VectorFloatHelper.ReadBinary(reader, ref vectorFloat);
        return vectorFloat;
    }

    public static VectorString ReadVectorString(this BinaryReader reader)
    {
        VectorString vectorString = VectorString.empty;
        VectorStringHelper.ReadBinary(reader, ref vectorString);
        return vectorString;
    }

    public static T[] ReadTEnumArray<T>(this BinaryReader reader) where T : IConvertible
    {
        int length = reader.ReadInt32();
        if (length > ArrayLengthLimit)
        {
            length = 0;
            return null;
        }
        T[] array = new T[length];
        for (int i = 0; i < length; i++)
        {
            array[i] = reader.ReadTEnum<T>();
        }

        return array;
    }

    public static bool[] ReadBooleanArray(this BinaryReader reader)
    {
        int length = reader.ReadInt32();
        if (length > ArrayLengthLimit)
        {
            length = 0;
            DebugEx.Log("[Failed] array's length is too long, ReadBooleanArray()");
        }
        bool[] array = new bool[length];
        for (int i = 0; i < length; i++)
        {
            array[i] = reader.ReadBoolean();
        }

        return array;
    }

    public static string[] ReadStringArray(this BinaryReader reader)
    {
        int length = reader.ReadInt32();
        if (length > ArrayLengthLimit)
        {
            length = 0;
            DebugEx.Log("[Failed] array's length is too long, ReadStringArray()");
            return null;
        }
        string[] array = new string[length];
        for (int i = 0; i < length; i++)
        {
            array[i] = reader.ReadString();
        }

        return array;
    }

    public static int[] ReadInt32Array(this BinaryReader reader)
    {
        int length = reader.ReadInt32();
        if (length > ArrayLengthLimit)
        {
            length = 0;
            DebugEx.Log("[Failed] array's length is too long, ReadInt32Array()");
            return null;
        }

        int[] array = new int[length];
        for (int i = 0; i < length; i++)
        {
            array[i] = reader.ReadInt32();
        }

        return array;
    }

    public static VectorInt[] ReadVectorIntArray(this BinaryReader reader)
    {
        int length = reader.ReadInt32();
        if (length > ArrayLengthLimit)
        {
            length = 0;
            DebugEx.Log("[Failed] array's length is too long, ReadVectorIntArray()");
            return null;
        }

        VectorInt[] array = new VectorInt[length];
        for (int i = 0; i < length; i++)
        {
            array[i] = reader.ReadVectorInt();
        }
        return array;
    }

    public static VectorFloat[] ReadVectorFloatArray(this BinaryReader reader)
    {
        int length = reader.ReadInt32();
        if (length > ArrayLengthLimit)
        {
            length = 0;
            DebugEx.Log("[Failed] array's length is too long, ReadVectorIntArray()");
            return null;
        }

        VectorFloat[] array = new VectorFloat[length];
        for (int i = 0; i < length; i++)
        {
            array[i] = reader.ReadVectorFloat();
        }
        return array;
    }

    public static VectorString[] ReadVectorStringArray(this BinaryReader reader)
    {
        int length = reader.ReadInt32();
        if (length > ArrayLengthLimit)
        {
            length = 0;
            DebugEx.Log("[Failed] array's length is too long, ReadVectorStringArray()");
            return null;
        }

        VectorString[] array = new VectorString[length];
        for (int i = 0; i < length; i++)
        {
            array[i] = reader.ReadVectorString();
        }
        return array;
    }

    public static float ReadFloat(this BinaryReader reader)
    {
        return reader.ReadSingle();
    }

    public static float[] ReadFloatArray(this BinaryReader reader)
    {
        int length = reader.ReadInt32();
        if (length > ArrayLengthLimit)
        {
            length = 0;
            DebugEx.Log("[Failed] array's length is too long, ReadFloatArray()");
            return null;
        }
        float[] array = new float[length];
        for (int i = 0; i < length; i++)
        {
            array[i] = reader.ReadSingle();
        }

        return array;
    }

    public static double[] ReadDoubleArray(this BinaryReader reader)
    {
        int length = reader.ReadInt32();
        if (length > ArrayLengthLimit)
        {
            length = 0;
            DebugEx.Log("[Failed] array's length is too long, ReadFloatArray()");
            return null;
        }
        double[] array = new double[length];
        for (int i = 0; i < length; i++)
        {
            array[i] = reader.ReadDouble();
        }

        return array;
    }

    public static Color[] ReadColorArray(this BinaryReader reader)
    {
        int length = reader.ReadInt32();
        if (length > ArrayLengthLimit)
        {
            length = 0;
            DebugEx.Log("[Failed] array's length is too long, ReadColorArray()");
        }
        Color[] array = new Color[length];
        for (int i = 0; i < length; i++)
        {
            array[i] = ReadColor(reader);
        }

        return array;
    }

    public static Vector2[] ReadVector2Array(this BinaryReader reader)
    {
        int length = reader.ReadInt32();
        if (length > ArrayLengthLimit)
        {
            length = 0;
            DebugEx.Log("[Failed] array's length is too long, ReadVector2Array()");
            return null;
        }
        Vector2[] array = new Vector2[length];
        for (int i = 0; i < length; i++)
        {
            array[i] = reader.ReadVector2();
        }

        return array;
    }

    public static Vector3[] ReadVector3Array(this BinaryReader reader)
    {
        int length = reader.ReadInt32();
        if (length > ArrayLengthLimit)
        {
            length = 0;
            DebugEx.Log("[Failed] array's length is too long, ReadVector3Array()");
            return null;
        }
        Vector3[] array = new Vector3[length];
        for (int i = 0; i < length; i++)
        {
            array[i] = reader.ReadVector3();
        }

        return array;
    }

    // public static void Write<T>(this BinaryWriter writer, TEnum<T> value)
    // {
    // }

    public static void Write(this BinaryWriter writer, string[] array)
    {
        WriteArray(writer, array);
    }

    public static void Write(this BinaryWriter writer, int[] array)
    {
        WriteArray(writer, array);
    }

    public static void Write(this BinaryWriter writer, float[] array)
    {
        WriteArray(writer, array);
    }

    public static void Write(this BinaryWriter writer, double[] array)
    {
        WriteArray(writer, array);
    }

    public static void Write(this BinaryWriter writer, Color value)
    {
        ColorHelper.Write(writer, value);
    }

    public static void Write(this BinaryWriter writer, Vector2 value)
    {
        Vector2Helper.WriteBinary(writer, value);
    }

    public static void Write(this BinaryWriter writer, Vector2[] value)
    {
        Vector2Helper.WriteBinary(writer, value);
    }

    public static void Write(this BinaryWriter writer, Vector3 value)
    {
        Vector3Helper.WriteBinary(writer, value);
    }

    public static void Write(this BinaryWriter writer, VectorInt[] value)
    {
        VectorIntHelper.WriteBinary(writer, value);
    }

    public static void Write(this BinaryWriter writer, VectorFloat[] value)
    {
        VectorFloatHelper.WriteBinary(writer, value);
    }

    public static void Write(this BinaryWriter writer, VectorString[] value)
    {
        VectorStringHelper.WriteBinary(writer, value);
    }

    public static T LoadJsonFile<T>(string loadPath, string fileName)
    {
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", loadPath, fileName), FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();
        string jsonData = System.Text.Encoding.UTF8.GetString(data);
        return JsonUtility.FromJson<T>(jsonData);
    }
}
