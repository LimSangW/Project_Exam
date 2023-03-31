using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Xml;
using System.IO;

[Serializable]
public class TableDoc
{
    public Dictionary<TableType, TableInfo> infoDict;
}

[Serializable]
public class TableInfo
{
    public string HashName;
    public long Size;

    public bool HasCache(string[] hashNames)
    {
        for (int i = 0; i < hashNames.Length; ++i)
        {
            if (Func.FastIndexOf(hashNames[i], HashName) != -1)
                return true;
        }
        return false;
    }
}

public class Tables
{
    private Dictionary<TableType, List<TRFoundation>> tableDatas = new Dictionary<TableType, List<TRFoundation>>();
    private Dictionary<TableType, List<TRFoundation>> localTableDatas = new Dictionary<TableType, List<TRFoundation>>();
    private Dictionary<Type, TableType> tableTypeDic = new Dictionary<Type, TableType>();
    const string Password = "pfproject";

    public static string TableDataPath => Application.persistentDataPath + "/Tables";

    public void InitTabeDatas()
    {
        tableDatas.Clear();
        LoadTableData();
    }

    private void LoadTableData()
    {
        DebugEx.Log("LoadTableData");
#if LOAD_LOCAL_TABLE
        var path = Func.GetProjectPath() + "/Assets/AssetBundles/Tables";
        var tableInfoPath = "Assets/AssetBundles/Tables/TableInfo.json";

#if UNITY_EDITOR
        var tableInfo = AssetDatabase.LoadAssetAtPath(tableInfoPath, typeof(TextAsset)) as TextAsset;
        var tableDoc = JsonConvert.DeserializeObject<TableDoc>(tableInfo.text);
#else
        FileStream file = new FileStream(tableInfoPath, FileMode.Open);
        StreamReader tableInfoReader = new StreamReader(file);
        string tableInfo = tableInfoReader.ReadToEnd();
        var tableDoc = JsonConvert.DeserializeObject<TableDoc>(tableInfo);
#endif

        for (int i = 0; i < (int)TableType.Length; ++i)
        {
            var type = (TableType)i;
            var hashName = tableDoc.infoDict[type].HashName;
            var reader = BinaryReaderFromFile(string.Format("{0}/{1}.bytes", path, hashName));
            ReadBinaryTable(type, reader);
        }
#else
        //for (int i = 0; i < (int)TableType.Length; ++i)
        //{
        //    var type = (TableType)i;
        //    var path = GetTablePath(type);
        //    DebugEx.Log($"[Table]type:{type}/path:{path}");
        //    var reader = BinaryReaderFromFile(path);
        //    if (reader != null)
        //        DebugEx.Log($"reader length:{reader.BaseStream.Length}");
        //    ReadBinaryTable(type, reader);
        //}
        //for (int i = 0; i < (int)TableType.Length; i++)
        //{
        //    var type = (TableType)i;
        //    var tableInfo = AddressAbleBundleLoader.Instance.LoadAsset<TextAsset>("TableInfo");
        //    var tableDoc = JsonConvert.DeserializeObject<TableDoc>(tableInfo.text);
        //    if (tableDoc.infoDict.ContainsKey(type) == true)
        //    {
        //        var tableData = AddressAbleBundleLoader.Instance.LoadAsset<TextAsset>(tableDoc.infoDict[type].hashName);
        //        var reader = BinaryReaderFromFile(tableData.bytes);
        //        if (reader != null)
        //            DebugEx.Log($"reader length:{reader.BaseStream.Length}");
        //        ReadBinaryTable(type, reader);
        //    }
        //    else
        //    {
        //        DebugEx.Log("[Table] [Failed] can't find table type : " + type);
        //    }
        //}
#endif
    }

    private string GetTablePath(TableType type)
    {
        string path = string.Empty;
        string hashName = string.Empty;
        path = TableDataPath;
        DebugEx.Log(path);
        var content = File.ReadAllText(path + "/TableInfo.json");
        var tableDoc = JsonConvert.DeserializeObject<TableDoc>(content);
        hashName = tableDoc.infoDict[type].HashName;
        return string.Format("{0}/{1}.bytes", path, hashName);
    }

    public static BinaryReader BinaryReaderFromFile(string path)
    {
        try
        {
            byte[] bytes = File.ReadAllBytes(path);
            return BinaryReaderFromFile(bytes);
        }
        catch (FileNotFoundException e)
        {
            DebugEx.LogError($"[Failed] IOHelper.BinaryReaderFromFile(): {path}, Message : {e.Message}");
            return null;
        }
    }

    public static BinaryReader BinaryReaderFromFile(byte[] bytes)
    {
        Stream s = new MemoryStream(bytes);
        return new BinaryReader(s);
    }

    public void ReadBinaryTable(TableType type, BinaryReader reader, bool loadLocal = false)
    {
        byte[] bytes = reader.ReadBytes((int)reader.BaseStream.Length);
        bytes = CryptoHelper.Decrypt(Password, bytes);
        if (null == bytes)
        {
            DebugEx.LogError("[Failed] can't read table binary: " + type.ToString());
            return;
        }

        MemoryStream s = new MemoryStream(bytes);
        try
        {
            reader = new BinaryReader(s);
        }
        catch (ArgumentException e)
        {
            DebugEx.LogError(string.Format("[Failed] can't create table's BinaryReader, type:{0}, msg:{1} ", type.ToString(), e.Message));
        }

        int tableLength = reader.ReadInt32();
        var recordList = new List<TRFoundation>();
        try
        {
            for (int i = 0; i < tableLength; i++)
            {
                var record = CreateRecord(type);
                record.Read(reader);
                recordList.Add(record);
                if (i == 0)
                    tableTypeDic[record.GetType()] = type;

            }
        }
        catch (EndOfStreamException e)
        {
            DebugEx.LogError("[Failed] Not enough data, Table: " + type.ToString());
            DebugEx.Log(e.ToString());
            DebugEx.Log(e.StackTrace);
            reader.Close();
            return;
        }
        catch (Exception e)
        {
            DebugEx.LogError(string.Format("[Failed] {0}Table, {1}", type.ToString(), e.ToString()));
            DebugEx.Log(e.StackTrace);
            reader.Close();
            return;
        }

        if (loadLocal)
            localTableDatas[type] = recordList;
        else
            tableDatas[type] = recordList;

        reader.Close();
    }

    public static bool ReadTable(string xmlFileName, List<TRFoundation> records, TableType type, string tableName)
    {
        XmlReaderSettings settings = XmlHelper.GetDefaultReadSetting();
        using (XmlReader reader = XmlReader.Create(xmlFileName, settings))
        {
            using (XmlReader reader2 = XmlReader.Create(xmlFileName, settings))
            {
                try
                {
                    if (reader == null)
                    {
                        DebugEx.LogError("[Failed] read table: " + xmlFileName);
                        return false;
                    }

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(reader2);

                    string recordName = "TR" + type.ToString();
                    int elementCount = xmlDoc.SelectNodes("Root/" + recordName).Count;
                    if (elementCount == 0)
                    {
                        DebugEx.LogError("[Failed]Table record's count is 0: tableRecordName:" + recordName);
                        return false;
                    }

                    int lineCount = 0;
                    TRFoundation.ReadStartRoot(reader);
                    while (lineCount < elementCount)
                    {
                        TRFoundation newRecord = CreateRecord(type);
                        if (newRecord == null)
                        {
                            reader.Close();
                            return false;
                        }
                        if (!newRecord.ReadRawData(reader))
                        {
                            DebugEx.LogError($"[Failed] {tableName} table wrong line: {lineCount + 1}, index: {newRecord.Index}");
#if UNITY_EDITOR
                            TableErrorLog.ShowWindow($"TableName:<color=#FF0000>{tableName}</color>, Table wrong line:{lineCount + 1}, Index:<color=#FF0000>{newRecord.Index}</color>");
#endif
                            reader.Close();
                            return false;
                        }
                        records.Add(newRecord);
                        lineCount++;
                    }

                    TRFoundation.ReadEndRoot(reader);
                    reader.Close();
                }
                catch (XmlException e)
                {
                    DebugEx.LogError($"[Failed] invalid Table:, path: {xmlFileName}\n{e.Message}");
                    return false;
                }
            }
        }
        return true;
    }

    public static void WriteBinaryTable(List<TRFoundation> records, string tableName, ref TableInfo info)
    {
        string tableRootPath = Func.GetProjectPath() + "/Assets/AssetBundles/Tables";
        MemoryStream memory = new MemoryStream();
        BinaryWriter bwriter = new BinaryWriter(memory);

        bwriter.Write(records.Count);

        for (int i = 0; i < records.Count; i++)
        {
            records[i].Write(bwriter);
        }

        bwriter.Close();
        var bytes = memory.ToArray();
        byte[] encryptedBytes = CryptoHelper.Encrypt(Password, bytes);

        tableName = CryptoHelper.GetHashValueString(encryptedBytes);
        string tablePath = tableRootPath + "/" + tableName + ".bytes";
        File.WriteAllBytes(tablePath, encryptedBytes);

        info.HashName = tableName;
        info.Size = encryptedBytes.LongLength;
    }

    public List<T> GetRecords<T>(TableType type, System.Func<T, bool> selector = null) where T : TRFoundation
    {
        List<T> result = new List<T>();
        if (!tableDatas.ContainsKey(type))
            DebugEx.LogError("[Failed]: table not exist:" + type);

        var recordList = tableDatas[type];
        for (int i = 0; i < recordList.Count; ++i)
        {
            T tr = (T)recordList[i];
            if (selector != null)
            {
                if (selector(tr))
                    result.Add(tr);
            }
            else
                result.Add(tr);
        }
        return result;
    }

    public T GetRecord<T>(TableType type, System.Func<T, bool> selector) where T : TRFoundation
    {
        var recordList = tableDatas[type];
        for (int i = 0; i < recordList.Count; ++i)
        {
            T tr = (T)recordList[i];
            if (selector(tr))
                return tr;
        }
        return default(T);
    }

    public TableType GetTableType(Type type)
    {
        return tableTypeDic[type];
    }


    private static TRFoundation CreateRecord(TableType type)
    {
        TRFoundation result = null;

        switch (type)
        {
            case TableType.Test:
                result = new TRTest();
                break;
        }

        return result;
    }
}
