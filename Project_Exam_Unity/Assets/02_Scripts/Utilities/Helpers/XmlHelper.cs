using UnityEngine;
using System.Collections.Generic;
using System;
using System.Xml;
using System.Globalization;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class XmlHelper
{
    private static XmlReaderSettings defaultReadsettings;

    public static XmlWriterSettings GetDefaultSetting()
    {
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.OmitXmlDeclaration = false;
        settings.IndentChars = ("\t");
        settings.NewLineChars = "\r\n";
        return settings;
    }

    public static XmlReaderSettings GetDefaultReadSetting()
    {
        if (defaultReadsettings == null)
        {
            defaultReadsettings = new XmlReaderSettings();
            defaultReadsettings.IgnoreComments = true;
            defaultReadsettings.IgnoreWhitespace = true;
            defaultReadsettings.IgnoreProcessingInstructions = true;
        }
        return defaultReadsettings;
    }

    public static void WriteStartRoot(XmlWriter writer)
    {
        writer.WriteStartElement("Root");
        writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
    }

    public static void WriteEndRoot(XmlWriter writer)
    {
        writer.WriteEndElement();
    }

    public static T ReadXML<T>(XmlReader reader, string elementName)
    {
        T result = default(T);
        bool readResult = false;
        reader.ReadStartElement(elementName);
        readResult = ReadXML<T>(reader, ref result);
        reader.ReadEndElement();
        if (!readResult)
        {
            DebugEx.LogError("invalid Xml elementName: " + elementName);
            return default(T);
        }
        return result;
    }

    public static bool ReadXML<T>(XmlReader reader, string elementName, ref T result, string desc = "")
    {
        bool readResult = false;
        reader.ReadStartElement(elementName);
        readResult = ReadXML<T>(reader, ref result, desc);
        reader.ReadEndElement();
        return readResult;
    }

    public static bool ReadXML<T>(XmlReader reader, ref T result, string desc = "")
    {
        try
        {
            string elementValue = reader.ReadContentAsString();
            if (string.IsNullOrEmpty(elementValue))
            {
                DebugEx.Log(string.Format("[Failed] empty value: {0}, type: {1}", desc, typeof(T).ToString()));
                return false;
            }

            result = (T)System.Convert.ChangeType(elementValue, typeof(T));
        }
        catch (System.FormatException e)
        {
            if (string.IsNullOrEmpty(desc))
                desc = reader.Name;

            DebugEx.LogError($"[Failed] invalid value, Name:{desc}, Message:{e.Message}");
            return false;
        }
        catch (System.InvalidCastException e)
        {
            if (string.IsNullOrEmpty(desc))
                desc = reader.Name;

            DebugEx.LogError("[Failed] invalid value type: " + desc + ", " + e.Message);
            return false;
        }

        return true;
    }

    public static bool ReadString(XmlReader reader, string elementName, ref string result)
    {
        string desc = elementName;
        bool readResult = false;

        if (reader.IsEmptyElement)
        {
            reader.ReadStartElement();
            result = string.Empty;
            DebugEx.Log($"[Failed] empty element: {elementName}");
#if UNITY_EDITOR
            TableErrorLog.ShowWindow($"[Failed] empty element: {elementName}");
#endif
            return true;
        }
        else
        {
            reader.ReadStartElement(elementName);
            readResult = ReadXML<string>(reader, ref result, desc);
            reader.ReadEndElement();
            return readResult;
        }
    }

    public static bool ReadBoolInt(XmlReader reader, string elementName, ref bool result)
    {
        bool readResult = false;
        int tempInt = 0;
        readResult = XmlHelper.ReadInt(reader, elementName, ref tempInt);
        if (readResult)
        {
            if (tempInt == 0)
                result = false;
            else
                result = true;
        }
        return readResult;
    }

    public static bool ReadInt(XmlReader reader, string elementName, ref int result)
    {
        bool readResult = false;
        try
        {
            var desc = elementName;
            reader.ReadStartElement(elementName);
            readResult = ReadXML<int>(reader, ref result, desc);
            reader.ReadEndElement();
        }
        catch (XmlException e)
        {
            DebugEx.LogError("[Failed] invalid xml node name: " + elementName + ", " + e.Message);
        }

        return readResult;
    }

    public static bool ReadUInt(XmlReader reader, string elementName, ref uint result)
    {
        bool readResult = false;
        try
        {
            var desc = elementName;
            reader.ReadStartElement(elementName);
            readResult = ReadXML<uint>(reader, ref result, desc);
            reader.ReadEndElement();
        }
        catch (XmlException e)
        {
            DebugEx.LogError("[Failed] invalid xml node name: " + elementName + ", " + e.Message);
        }

        return readResult;
    }

    public static bool ReadEnumStringInt<T>(XmlReader reader, string elementName, ref int result) where T : IConvertible
    {
        string tempInt = string.Empty;
        bool readResult = ReadString(reader, elementName, ref tempInt);
        T enumValue = (T)Enum.Parse(typeof(T), tempInt);
        result = (int)System.Convert.ChangeType(enumValue, typeof(int));

        return readResult;
    }

    public static bool ReadEnumInt<T>(XmlReader reader, string elementName, ref T result) where T : IConvertible
    {
        int tempInt = 0;
        bool readResult = ReadInt(reader, elementName, ref tempInt);
        result = (T)(Enum.ToObject(typeof(T), tempInt));

        return readResult;
    }

    public static bool ReadEnumString<T>(XmlReader reader, string elementName, ref T result) where T : IConvertible
    {
        string tempString = string.Empty;
        bool readResult = ReadString(reader, elementName, ref tempString);
        result = (T)Enum.Parse(typeof(T), tempString);

        return readResult;
    }

    public static bool ReadEnumStringArray<T>(XmlReader reader, string elementName, ref T[] value) where T : IConvertible
    {
        string desc = elementName;
        string result = string.Empty;
        bool readResult = true;
        reader.ReadStartElement(elementName);
        readResult &= ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();

        string[] enumString = null;
        string[] splits = result.Split(DelimiterHelper.Semicolon);
        readResult &= IOHelper.ReadArray2<string>(desc, splits, ref enumString);

        value = new T[enumString.Length];
        for (int i = 0; i < enumString.Length; i++)
        {
            value[i] = (T)Enum.Parse(typeof(T), enumString[i]);
        }

        return readResult;
    }

    public static bool ReadEnumIntArray<T>(XmlReader reader, string elementName, ref T[] value) where T : IConvertible
    {
        string desc = elementName;
        string result = string.Empty;
        bool readResult = true;
        reader.ReadStartElement(elementName);
        readResult &= ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();

        string[] enumString = null;
        string[] splits = result.Split(DelimiterHelper.Semicolon);
        readResult &= IOHelper.ReadArray2<string>(desc, splits, ref enumString);

        value = new T[enumString.Length];
        for (int i = 0; i < enumString.Length; i++)
        {
            value[i] = (T)Enum.Parse(typeof(T), enumString[i]);
        }

        return readResult;
    }


    public static bool ReadLong(XmlReader reader, string elementName, ref long result)
    {
        string desc = elementName;
        bool readResult = false;
        try
        {
            reader.ReadStartElement(elementName);
            readResult = ReadXML<long>(reader, ref result, desc);
            reader.ReadEndElement();
        }
        catch (XmlException e)
        {
            DebugEx.LogError("[Failed] invalid xml node name: " + elementName + ", " + e.Message);
        }

        return readResult;
    }

    public static bool ReadFloat(XmlReader reader, string elementName, ref float result)
    {
        string desc = elementName;
        bool readResult = false;
        try
        {
            reader.ReadStartElement(elementName);
            readResult = ReadXML<float>(reader, ref result, desc);
            reader.ReadEndElement();
        }
        catch (XmlException e)
        {
            DebugEx.LogError("[Failed] invalid xml node name: " + elementName + ", " + e.Message);
        }

        return readResult;
    }

    public static bool ReadVector2(XmlReader reader, string elementName, ref Vector2 value)
    {
        bool readResult = Vector2Helper.ReadXML(reader, elementName, elementName, ref value);
#if UNITY_EDITOR
        if (!readResult)
            TableErrorLog.ShowWindow($"[TableTypeError]<color=#FF0000>{elementName}</color> is wrong type");
#endif
        return readResult;
    }

    public static bool ReadVector3(XmlReader reader, string elementName, ref Vector3 value)
    {
        bool readResult = Vector3Helper.ReadXML(reader, elementName, elementName, ref value);
#if UNITY_EDITOR
        if (!readResult)
            TableErrorLog.ShowWindow($"[TableTypeError]<color=#FF0000>{elementName}</color> is wrong type");
#endif
        return readResult;
    }

    public static bool ReadVectorInt(XmlReader reader, string elementName, ref VectorInt value)
    {
        bool readResult = VectorIntHelper.ReadXML(reader, elementName, elementName, ref value);
#if UNITY_EDITOR
        if (!readResult)
            TableErrorLog.ShowWindow($"[TableTypeError]<color=#FF0000>{elementName}</color> is wrong type");
#endif
        return readResult;
    }

    public static bool ReadVectorString(XmlReader reader, string elementName, ref VectorString value)
    {
        bool readResult = VectorStringHelper.ReadXML(reader, elementName, elementName, ref value);
#if UNITY_EDITOR
        if (!readResult)
            TableErrorLog.ShowWindow($"[TableTypeError]<color=#FF0000>{elementName}</color> is wrong type");
#endif
        return readResult;
    }

    public static bool ReadColor(XmlReader reader, string elementName, ref Color value)
    {
        bool readResult = ColorHelper.ReadXML(reader, elementName, elementName, ref value);
#if UNITY_EDITOR
        if (!readResult)
            TableErrorLog.ShowWindow($"[TableTypeError]<color=#FF0000>{elementName}</color> is wrong type");
#endif
        return readResult;
    }

    public static bool ReadColor32(XmlReader reader, string elementName, ref Color32 value)
    {
        bool readResult = ColorHelper.ReadXML(reader, elementName, elementName, ref value);
#if UNITY_EDITOR
        if (!readResult)
            TableErrorLog.ShowWindow($"[TableTypeError]<color=#FF0000>{elementName}</color> is wrong type");
#endif
        return readResult;
    }

    public static bool ReadDateTime(XmlReader reader, string elementName, ref DateTime value)
    {
        string desc = elementName;
        string result = string.Empty;
        bool readResult = false;
        reader.ReadStartElement(elementName);
        readResult = XmlHelper.ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();

        CultureInfo provider = CultureInfo.InvariantCulture;

        result = result.Replace(" (UTC)", string.Empty);
        result = result.Replace("GMT", string.Empty);

        try
        {
            DateTimeOffset dt = DateTimeOffset.Parse(result);
            value = dt.DateTime;
        }
        catch (System.Exception e)
        {
            string log = string.Format("[Failed] invalid value: elementName:{0}, {1}", elementName, e.Message);
            DebugEx.LogError(log);
        }
#if UNITY_EDITOR
        if (!readResult)
            TableErrorLog.ShowWindow($"[TableTypeError]<color=#FF0000>{desc}</color> is wrong type");
#endif
        return readResult;
    }

    public static bool ReadTimeSpan(XmlReader reader, string elementName, ref TimeSpan value)
    {
        string desc = elementName;
        string result = string.Empty;
        bool readResult = false;
        reader.ReadStartElement(elementName);
        readResult = XmlHelper.ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();

        try
        {
            value = TimeSpan.Parse(result);
        }
        catch (System.Exception e)
        {
            string log = string.Format("[Failed] invalid value: elementName:{0}, {1}", elementName, e.Message);
            DebugEx.LogError(log);
        }
#if UNITY_EDITOR
        if (!readResult)
            TableErrorLog.ShowWindow($"[TableTypeError]<color=#FF0000>{desc}</color> is wrong type");
#endif
        return readResult;
    }

    public static bool ReadRectWidthHeight(XmlReader reader, string elementName, ref Rect value)
    {
        string desc = elementName;
        string result = string.Empty;
        bool readResult = false;
        reader.ReadStartElement(elementName);
        readResult = XmlHelper.ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();

        Vector2[] array = new Vector2[2];
        string[] splits = result.Split(DelimiterHelper.Semicolon);
        for (int i = 0; i < splits.Length; i++)
        {
            Vector2Helper.Parse(splits[i], ref array[i]);
        }
        value.Set(array[0].x, array[0].y, array[1].x, array[1].y);
#if UNITY_EDITOR
        if (!readResult)
            TableErrorLog.ShowWindow($"[TableTypeError]<color=#FF0000>{desc}</color> is wrong type");
#endif
        return readResult;
    }

    public static bool ReadRectMinMax(XmlReader reader, string elementName, ref Rect value)
    {
        string desc = elementName;
        string result = string.Empty;
        bool readResult = false;
        reader.ReadStartElement(elementName);
        readResult = XmlHelper.ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();

        Vector2[] array = new Vector2[2];
        string[] splits = result.Split(DelimiterHelper.Semicolon);
        for (int i = 0; i < splits.Length; i++)
        {
            Vector2Helper.Parse(splits[i], ref array[i]);
        }

        value.xMin = array[0].x;
        value.xMax = array[0].y;

        value.yMin = array[1].x;
        value.yMax = array[1].y;
#if UNITY_EDITOR
        if (!readResult)
            TableErrorLog.ShowWindow($"[TableTypeError]<color=#FF0000>{desc}</color> is wrong type");
#endif
        return readResult;
    }

    public static bool ReadStringArray(XmlReader reader, string elementName, ref string[] value)
    {
        string desc = elementName;
        string result = string.Empty;
        bool readResult = true;
        reader.ReadStartElement(elementName);
        readResult &= ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();

        string[] splits = result.Split(DelimiterHelper.Semicolon);
        readResult &= IOHelper.ReadArray2<string>(desc, splits, ref value);
#if UNITY_EDITOR
        if (!readResult)
            TableErrorLog.ShowWindow($"[TableTypeError]<color=#FF0000>{desc}</color> is wrong type");
#endif
        return readResult;
    }

    public static bool ReadStringList(XmlReader reader, string elementName, ref List<string> value)
    {
        string desc = elementName;
        string result = string.Empty;
        bool readResult = true;
        reader.ReadStartElement(elementName);
        readResult &= ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();

        string[] splits = result.Split(DelimiterHelper.Semicolon);
        readResult &= IOHelper.ReadList<string>(desc, splits, ref value);
#if UNITY_EDITOR
        if (!readResult)
            TableErrorLog.ShowWindow($"[TableTypeError]<color=#FF0000>{desc}</color> is wrong type");
#endif
        return readResult;
    }

    public static bool ReadBoolIntArray(XmlReader reader, string elementName, ref bool[] value)
    {
        string desc = elementName;
        string result = string.Empty;
        bool readResult = true;
        reader.ReadStartElement(elementName);
        readResult &= ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();

        string[] splits = result.Split(DelimiterHelper.Semicolon);
        int[] intValues = null;
        readResult &= IOHelper.ReadArray2<int>(desc, splits, ref intValues);
        value = System.Array.ConvertAll(intValues, x => { return x == 1 ? true : false; });
#if UNITY_EDITOR
        if (!readResult)
            TableErrorLog.ShowWindow($"[TableTypeError]<color=#FF0000>{desc}</color> is wrong type");
#endif
        return readResult;
    }

    public static bool ReadIntArray(XmlReader reader, string elementName, ref int[] value)
    {
        string desc = elementName;
        string result = string.Empty;
        bool readResult = true;
        reader.ReadStartElement(elementName);
        readResult &= ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();

        string[] splits = result.Split(DelimiterHelper.Semicolon);

        if (splits.Length == 1)
        {
            if (splits[0] == string.Empty)
            {
                value = new int[0];
                return false;
            }
        }
        readResult &= IOHelper.ReadArray2<int>(desc, splits, ref value);
#if UNITY_EDITOR
        if (!readResult)
            TableErrorLog.ShowWindow($"[TableTypeError]<color=#FF0000>{desc}</color> is wrong type");
#endif
        return readResult;
    }

    public static bool ReadIntList(XmlReader reader, string elementName, ref List<int> value)
    {
        string desc = elementName;
        string result = string.Empty;
        bool readResult = true;
        reader.ReadStartElement(elementName);
        readResult &= ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();

        string[] splits = result.Split(DelimiterHelper.Semicolon);
        readResult &= IOHelper.ReadList<int>(desc, splits, ref value);
#if UNITY_EDITOR
        if (!readResult)
            TableErrorLog.ShowWindow($"[TableTypeError]<color=#FF0000>{desc}</color> is wrong type");
#endif
        return readResult;
    }

    public static bool ReadFloatArray(XmlReader reader, string elementName, ref float[] value)
    {
        string desc = elementName;
        string result = string.Empty;
        bool readResult = true;
        reader.ReadStartElement(elementName);
        readResult &= ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();

        string[] splits = result.Split(DelimiterHelper.Semicolon);
        readResult &= IOHelper.ReadArray2<float>(desc, splits, ref value);
#if UNITY_EDITOR
        if (!readResult)
            TableErrorLog.ShowWindow($"[TableTypeError]<color=#FF0000>{desc}</color> is wrong type");
#endif
        return readResult;
    }

    public static bool ReadDoubleArray(XmlReader reader, string elementName, ref double[] value)
    {
        string desc = elementName;
        string result = string.Empty;
        bool readResult = true;
        reader.ReadStartElement(elementName);
        readResult &= ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();

        string[] splits = result.Split(DelimiterHelper.Semicolon);
        readResult &= IOHelper.ReadArray2<double>(desc, splits, ref value);
#if UNITY_EDITOR
        if (!readResult)
            TableErrorLog.ShowWindow($"[TableTypeError]<color=#FF0000>{desc}</color> is wrong type");
#endif
        return readResult;
    }

    public static bool ReadFloatList(XmlReader reader, string elementName, ref List<float> value)
    {
        string desc = elementName;
        string result = string.Empty;
        bool readResult = true;
        reader.ReadStartElement(elementName);
        readResult &= ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();

        string[] splits = result.Split(DelimiterHelper.Semicolon);
        readResult &= IOHelper.ReadList<float>(desc, splits, ref value);
#if UNITY_EDITOR
        if (!readResult)
            TableErrorLog.ShowWindow($"[TableTypeError]<color=#FF0000>{desc}</color> is wrong type");
#endif
        return readResult;
    }

    public static bool ReadVector2Array(XmlReader reader, string elementName, ref Vector2[] value)
    {
        string desc = elementName;
        string result = string.Empty;
        reader.ReadStartElement(elementName);
        bool readResult = ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();

        string[] splits = result.Split(DelimiterHelper.Semicolon);
        value = new Vector2[splits.Length];

        for (int i = 0; i < splits.Length; i++)
        {
            Vector2Helper.Parse(splits[i], ref value[i]);
        }
#if UNITY_EDITOR
        if (!readResult)
            TableErrorLog.ShowWindow($"[TableTypeError]<color=#FF0000>{desc}</color> is wrong type");
#endif
        return true;
    }

    public static bool ReadVectorIntArray(XmlReader reader, string elementName, ref VectorInt[] value)
    {
        string desc = elementName;
        string result = string.Empty;
        reader.ReadStartElement(elementName);
        bool readResult = ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();

        string[] splits = result.Split(DelimiterHelper.Semicolon);
        value = new VectorInt[splits.Length];

        if (splits.Length == 1)
        {
            if (splits[0] == string.Empty)
            {
#if UNITY_EDITOR
                TableErrorLog.ShowWindow($"[TableTypeError]<color=#FF0000>{desc}</color> is wrong type");
#endif
                value = new VectorInt[0];
                return false;
            }
        }

        for (int i = 0; i < splits.Length; i++)
        {
            VectorIntHelper.Parse(splits[i], ref value[i]);
        }
#if UNITY_EDITOR
        if (!readResult)
            TableErrorLog.ShowWindow($"[TableTypeError]<color=#FF0000>{desc}</color> is wrong type");
#endif
        return true;
    }

    public static bool ReadVectorFloatArray(XmlReader reader, string elementName, ref VectorFloat[] value)
    {
        string desc = elementName;
        string result = string.Empty;
        reader.ReadStartElement(elementName);
        bool readResult = ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();

        string[] splits = result.Split(DelimiterHelper.Semicolon);
        value = new VectorFloat[splits.Length];

        if (splits.Length == 1)
        {
            if (splits[0] == string.Empty)
            {
#if UNITY_EDITOR
                TableErrorLog.ShowWindow($"[TableTypeError]<color=#FF0000>{desc}</color> is wrong type");
#endif
                value = new VectorFloat[0];
                return false;
            }
        }

        for (int i = 0; i < splits.Length; i++)
        {
            VectorFloatHelper.Parse(splits[i], ref value[i]);
        }
#if UNITY_EDITOR
        if (!readResult)
            TableErrorLog.ShowWindow($"[TableTypeError]<color=#FF0000>{desc}</color> is wrong type");
#endif
        return true;
    }

    public static bool ReadVectorStringArray(XmlReader reader, string elementName, ref VectorString[] value)
    {
        string desc = elementName;
        string result = string.Empty;
        reader.ReadStartElement(elementName);
        bool readResult = ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();

        string[] splits = result.Split(DelimiterHelper.Semicolon);
        value = new VectorString[splits.Length];

        if (splits.Length == 1)
        {
            if (splits[0] == string.Empty)
            {
#if UNITY_EDITOR
                TableErrorLog.ShowWindow($"[TableTypeError]<color=#FF0000>{desc}</color> is wrong type");
#endif
                value = new VectorString[0];
                return false;
            }
        }

        for (int i = 0; i < splits.Length; i++)
        {
            VectorStringHelper.Parse(splits[i], ref value[i]);
        }
#if UNITY_EDITOR
        if (!readResult)
            TableErrorLog.ShowWindow($"[TableTypeError]<color=#FF0000>{desc}</color> is wrong type");
#endif
        return true;
    }

    public static bool ReadVector2IntArray(XmlReader reader, string elementName, ref Vector2Int[] value)
    {
        string desc = elementName;
        string result = string.Empty;
        reader.ReadStartElement(elementName);
        bool readResult = ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();

        string[] splits = result.Split(DelimiterHelper.Semicolon);
        value = new Vector2Int[splits.Length];

        for (int i = 0; i < splits.Length; i++)
        {
            Vector2Helper.Parse(splits[i], ref value[i]);
        }
#if UNITY_EDITOR
        if (!readResult)
            TableErrorLog.ShowWindow($"[TableTypeError]<color=#FF0000>{desc}</color> is wrong type");
#endif
        return true;
    }

    public static bool ReadVector2StringArray(XmlReader reader, string elementName, ref Vector2String[] value)
    {
        string desc = elementName;
        string result = string.Empty;
        reader.ReadStartElement(elementName);
        bool readResult = ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();

        string[] splits = result.Split(DelimiterHelper.Semicolon);
        value = new Vector2String[splits.Length];

        for (int i = 0; i < splits.Length; i++)
        {
            Vector2Helper.Parse(splits[i], ref value[i]);
        }
#if UNITY_EDITOR
        if (!readResult)
            TableErrorLog.ShowWindow($"[TableTypeError]<color=#FF0000>{desc}</color> is wrong type");
#endif
        return true;
    }

    public static bool ReadVector2List(XmlReader reader, string elementName, ref List<Vector2> value)
    {
        string desc = elementName;
        string result = string.Empty;
        reader.ReadStartElement(elementName);
        bool readResult = ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();

        string[] splits = result.Split(DelimiterHelper.Semicolon);
        readResult &= IOHelper.ReadVector2List(desc, splits, ref value);
#if UNITY_EDITOR
        if (!readResult)
            TableErrorLog.ShowWindow($"[TableTypeError]<color=#FF0000>{desc}</color> is wrong type");
#endif
        return readResult;
    }

    public static bool ReadVector3Array(XmlReader reader, string elementName, ref Vector3[] value)
    {
        string desc = elementName;
        string result = string.Empty;
        reader.ReadStartElement(elementName);
        bool readResult = ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();

        string[] splits = result.Split(DelimiterHelper.Semicolon);
        value = new Vector3[splits.Length];
        for (int i = 0; i < splits.Length; i++)
        {
            Vector3Helper.Parse(splits[i], ref value[i]);
        }
#if UNITY_EDITOR
        if (!readResult)
            TableErrorLog.ShowWindow($"[TableTypeError]<color=#FF0000>{desc}</color> is wrong type");
#endif
        return true;
    }

    public static bool ReadVector3List(XmlReader reader, string elementName, ref List<Vector3> value)
    {
        string desc = elementName;
        string result = string.Empty;
        reader.ReadStartElement(elementName);
        bool readResult = ReadXML<string>(reader, ref result, desc);
        reader.ReadEndElement();

        string[] splits = result.Split(DelimiterHelper.Semicolon);
        readResult &= IOHelper.ReadVector3List(desc, splits, ref value);

#if UNITY_EDITOR
        if (!readResult)
            TableErrorLog.ShowWindow($"[TableTypeError]<color=#FF0000>{desc}</color> is wrong type");
#endif
        return readResult;
    }

    public static void WriteElement(XmlWriter writer, string name, string value)
    {
        writer.WriteStartElement(name);
        writer.WriteValue(value);
        writer.WriteEndElement();
    }

    public static void WriteElement(XmlWriter writer, string name, bool value)
    {
        writer.WriteStartElement(name);
        writer.WriteValue((Boolean)value);
        writer.WriteEndElement();
    }

    public static void WriteElement(XmlWriter writer, string name, int value)
    {
        writer.WriteStartElement(name);
        writer.WriteValue(value);
        writer.WriteEndElement();
    }

    public static void WriteElement(XmlWriter writer, string name, float value)
    {
        writer.WriteStartElement(name);
        writer.WriteValue((Single)value);
        writer.WriteEndElement();
    }

    public static void WriteElement(XmlWriter writer, string name, Color value)
    {
        writer.WriteStartElement(name);
        writer.WriteValue(ColorHelper.Pack(value));
        writer.WriteEndElement();
    }

    public static void WriteElement(XmlWriter writer, string name, Color32 value)
    {
        writer.WriteStartElement(name);
        writer.WriteValue(ColorHelper.Pack(value));
        writer.WriteEndElement();
    }

    public static void WriteElement(XmlWriter writer, string name, Vector2 value)
    {
        writer.WriteStartElement(name);
        writer.WriteValue(Vector2Helper.Pack(value));
        writer.WriteEndElement();
    }

    public static void WriteElement(XmlWriter writer, string name, Vector3 value)
    {
        writer.WriteStartElement(name);
        writer.WriteValue(Vector3Helper.Pack(value));
        writer.WriteEndElement();
    }

    public static void WriteElement(XmlWriter writer, string name, List<string> valueList)
    {
        writer.WriteStartElement(name);
        string value = string.Empty;
        for (int i = 0; i < valueList.Count; i++)
        {
            value += valueList[i];
            if (i < valueList.Count - 1)
                value += ";";
        }
        writer.WriteValue(value);
        writer.WriteEndElement();
    }

    public static void WriteElement(XmlWriter writer, string name, List<Vector3> valueList)
    {
        writer.WriteStartElement(name);
        string value = string.Empty;
        for (int i = 0; i < valueList.Count; i++)
        {
            value += Vector3Helper.Pack(valueList[i]);
            if (i < valueList.Count - 1)
                value += ";";
        }
        writer.WriteValue(value);
        writer.WriteEndElement();
    }
}