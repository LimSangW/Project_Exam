using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Reflection;
using System.Linq;
using System;

public class PFProjectSettings : EditorWindow
{
    [MenuItem("PFProject/Settings")]
    public static void ShowWindow()
    {
        PFProjectSettings window = (PFProjectSettings)GetWindow(typeof(PFProjectSettings));
        window.minSize = new Vector2(500, 500);
    }

    private GUIStyle clientVersionStyle;

    private Dictionary<string, TableType> tableTypeDic = new Dictionary<string, TableType>();


    public bool LoadLocalTable
    {
        get { return EditorPrefsEx.GetBool("LoadLocalTable"); }
        set { EditorPrefsEx.SetBool("LoadLocalTable", value); }
    }


    private void OnGUI()
    {
        clientVersionStyle = new GUIStyle(EditorStyles.textArea);
        clientVersionStyle.fontSize = 20;
        clientVersionStyle.wordWrap = true;
        GUIContent content = new GUIContent("Client Version: " + ClientInfo.ClientVersion);
        EditorGUILayout.LabelField(content, clientVersionStyle, GUILayout.ExpandWidth(true), GUILayout.Height(36f));
        clientVersionStyle.fontSize = 12;

        if (GUILayout.Button("Playerprefs delete"))
        {
            PlayerPrefs.DeleteAll();
        }

        LoadLocalTable = EditorGUILayout.Toggle("Load Local Table: ", LoadLocalTable);
        EditorGUILayout.Space();

        var boldText = new GUIStyle(GUI.skin.label);
        boldText.fontStyle = FontStyle.Bold;
        boldText.fontSize = 15;
        EditorGUILayout.LabelField("CreateData", boldText);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("table convert"))
        {
            ConvertBinary();

            CheckTableData();
            CheckTableIndex();
        }
        EditorGUILayout.EndHorizontal();

        UpdateDefineOption();
    }

    public static void ConvertTable()
    {
        ConvertBinary();
    }

    public static void ConvertBinary()
    {
        CovertExcelToXml("TableExcel", "TableXml");
        ConvertBinary2();
    }

    public static void ConvertBinary2()
    {
        DeleteOldTableData();
        var xmlRoot = Func.GetProjectPath() + "/TableXml";
        var tableDoc = new TableDoc();
        tableDoc.infoDict = new Dictionary<TableType, TableInfo>();
        for (int i = 0; i < (int)TableType.Length; ++i)
        {
            var type = (TableType)i;

            var path = xmlRoot + string.Format("/TR{0}.xml", type);
            var recordList = new List<TRFoundation>();
            Tables.ReadTable(path, recordList, type, type.ToString());
            var tableInfo = new TableInfo();
            Tables.WriteBinaryTable(recordList, string.Format("/TR{0}", type), ref tableInfo);
            tableDoc.infoDict[type] = tableInfo;
        }

        var json = JsonConvert.SerializeObject(tableDoc, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(Func.GetProjectPath() + "/Assets/AssetBundles/Tables/TableInfo.json", json);
        AssetDatabase.Refresh();
        DebugEx.Log("table convert complete");
    }

    public static void CovertExcelToXml(string inputPath, string outputPath)
    {
        string pyFileRoot = Path.Combine(Func.GetProjectPath(), "tools");
        Process process = new Process();
        process.StartInfo.FileName = GetPythonCmd();
        var pythonFile = Path.Combine($"{pyFileRoot}", "excel2xml-table.py");
        process.StartInfo.Arguments = $"{pythonFile} {inputPath} {outputPath}";
        if (!Application.isBatchMode)
        {
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
        }

        DebugEx.Log($"{process.StartInfo.FileName}\n{pyFileRoot}\n{inputPath}\n{outputPath}");

        process.Start();
        process.WaitForExit();
    }

    static string GetPythonCmd()
    {
#if UNITY_EDITOR_WIN
        return "C:/Windows/py.exe";
#endif

#if UNITY_EDITOR_OSX
        return "/usr/local/bin/python3";
#endif
    }

    public static void DeleteOldTableData()
    {
        string exportPath = Func.GetProjectPath() + "/Assets/AssetBundles/Tables";
        if (Directory.Exists(exportPath))
        {
            string[] files = Directory.GetFiles(exportPath);
            for (int i = 0; i < files.Length; i++)
            {
                File.Delete(files[i]);
            }
        }

        DebugEx.Log("deleting old table data is completed");
    }

    private void CheckTableData()
    {
        DataManager.Instance.LoadTable();
        for (int i = 0; i < (int)TableType.Length; ++i)
        {
            var tableType = (TableType)i;
            var recordList = DataManager.Instance.GetRecords(tableType);
            for (int j = 0; j < recordList.Count; ++j)
            {
                bool valid = recordList[j].CheckTableMatch();
                if (valid == false)
                {
                    var content = recordList[j].GetMatchCount();
                    TableErrorLog.ShowWindow(
                        $"[DataMatchError]<color=#FFFF00>{tableType}</color>, index:<color=#FFFF00>{recordList[j].Index}</color>, {content}");
                }
            }
        }
    }

    public void CheckTableIndex()
    {
        tableTypeDic.Clear();
        var subclassTypes = Assembly
            .GetAssembly(typeof(TRFoundation))
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(TRFoundation))).ToList();

        //  var tableDatas = DataManager.Instance.GetTableDatas();

        //CurTable, Dic(PropertyName,TargetTable))
        var dependentIndexDictionary = new Dictionary<TableType, Dictionary<Tuple<string, string, int>, TableType>>();
        foreach (TableType tableType in Enum.GetValues(typeof(TableType)))
        {
            if (tableType == TableType.Length)
                continue;

            var tableList = DataManager.Instance.GetRecords(tableType);

            for (int i = 0; i < tableList.Count; i++)
            {
                var trFoundation = tableList[i];
                if (i == 0)
                {
                    var propertyInfos = trFoundation.GetType().GetProperties();
                    var typeOfTable = trFoundation.GetType();
                    Attribute[] attributes = Attribute.GetCustomAttributes(typeOfTable);
                    for (int j = 0; j < propertyInfos.Length; j++)
                    {
                        PropertyInfo propertyInfo = propertyInfos[j];
                        var customAttributes = propertyInfo.GetCustomAttributes();
                        foreach (var customAttribute in customAttributes)
                        {
                            TRFoundation.DependentIndexFlag dependentIndexFlag =
                                customAttribute as TRFoundation.DependentIndexFlag;


                            if (dependentIndexFlag != null)
                            {
                                Dictionary<Tuple<string, string, int>, TableType> indexDic = null;
                                dependentIndexDictionary.TryGetValue(tableType, out indexDic);
                                if (indexDic == null)
                                {
                                    indexDic = new Dictionary<Tuple<string, string, int>, TableType>();
                                }

                                indexDic.Add(new Tuple<string, string, int>(propertyInfo.Name, "None", -1), dependentIndexFlag.TableType);
                                dependentIndexDictionary[tableType] = indexDic;
                            }


                            TRFoundation.DependentIndexFlagByEnum dependentIndexFlagByEnum = customAttribute as TRFoundation.DependentIndexFlagByEnum;
                            if (dependentIndexFlagByEnum != null)
                            {
                                Dictionary<Tuple<string, string, int>, TableType> indexDic = null;
                                dependentIndexDictionary.TryGetValue(tableType, out indexDic);
                                if (indexDic == null)
                                {
                                    indexDic = new Dictionary<Tuple<string, string, int>, TableType>();
                                }

                                indexDic.Add(new Tuple<string, string, int>(propertyInfo.Name, dependentIndexFlagByEnum.SearchPropertyName, dependentIndexFlagByEnum.EnumValue), dependentIndexFlagByEnum.TableType);
                                dependentIndexDictionary[tableType] = indexDic;

                            }
                        }
                    }

                    break;
                }
            }
        }

        foreach (var dicData in dependentIndexDictionary)
        {
            Dictionary<Tuple<string, string, int>, TableType> dic = null;
            dependentIndexDictionary.TryGetValue(dicData.Key, out dic);
            if (dic == null)
                continue;
            var tableList = DataManager.Instance.GetRecords(dicData.Key);
            foreach (var keyValuePair in dic)
            {
                for (int i = 0; i < tableList.Count; i++)
                {
                    TRFoundation trFoundation = tableList[i];

                    string keyName = keyValuePair.Key.Item1;
                    string subKeyName = keyValuePair.Key.Item2;
                    int enumValue = keyValuePair.Key.Item3;

                    PropertyInfo property = trFoundation.GetType().GetProperty(keyName);
                    if (property == null || trFoundation == null)
                    {
                        continue;
                    }

                    var value = property.GetValue(trFoundation);

                    if (property.PropertyType.IsArray)
                    {
                        int[] indexes = ((IEnumerable)value).Cast<int>().ToArray();

                        for (int j = 0; j < indexes.Length; j++)
                        {
                            if (indexes[j] <= 0)
                                continue;

                            if (enumValue != -1)
                            {
                                PropertyInfo innerProperty = trFoundation.GetType().GetProperty(subKeyName);
                                var findValue = innerProperty.GetValue(trFoundation);
                                int findEnumValue = Convert.ToInt32(findValue);
                                if (enumValue == findEnumValue)
                                {
                                    var result = DataManager.Instance.GetRecords(keyValuePair.Value)
                                        .Find(x => x.Index == indexes[j]);
                                    if (result == null)
                                    {
                                        TableErrorLog.ShowWindow(
                                            $"[TableIndexError]<color=#FF0000>{keyValuePair.Key}</color> Index : {indexes[j]} in {trFoundation.GetTableName()} -> {keyValuePair.Value.ToString()}");
                                    }
                                }
                            }
                            else
                            {
                                var result = DataManager.Instance.GetRecords(keyValuePair.Value)
                                    .Find(x => x.Index == indexes[j]);
                                if (result == null)
                                {
                                    TableErrorLog.ShowWindow(
                                        $"[TableIndexError]<color=#FF0000>{keyValuePair.Key}</color> Index : {indexes[j]} in {trFoundation.GetTableName()} -> {keyValuePair.Value.ToString()}");
                                }
                            }
                        }
                    }
                    else
                    {
                        int findIndex = Convert.ToInt32(value);
                        if (findIndex <= 0)
                            continue;

                        if (enumValue != -1)
                        {
                            var findValue = property.GetValue(subKeyName);
                            int findEnumValue = Convert.ToInt32(value);
                            if (enumValue == findEnumValue)
                            {
                                var result = DataManager.Instance.GetRecords(keyValuePair.Value)
                                    .Find(x => x.Index == findIndex);
                                if (result == null)
                                {
                                    TableErrorLog.ShowWindow(
                                        $"[TableIndexError]<color=#FF0000>{keyValuePair.Key}</color> Index : {findIndex} in {trFoundation.GetTableName()} -> {keyValuePair.Value.ToString()}");
                                }
                            }
                        }
                        else
                        {

                            var result = DataManager.Instance.GetRecords(keyValuePair.Value)
                                .Find(x => x.Index == findIndex);
                            if (result == null)
                            {
                                TableErrorLog.ShowWindow(
                                    $"[TableIndexError]<color=#FF0000>{keyValuePair.Key}</color> Index : {findIndex} in {trFoundation.GetTableName()} -> {keyValuePair.Value.ToString()}");
                            }
                        }
                    }
                }
            }
        }
    }

    private void UpdateDefineOption()
    {
        List<string> options = GetCurrentDefineOption();

        if (LoadLocalTable)
        {
            options.Add("LOAD_LOCAL_TABLE");
        }
        else
        {
            if (options.Contains("LOAD_LOCAL_TABLE"))
                options.Remove("LOAD_LOCAL_TABLE");
        }

        BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        options = options.Distinct().ToList();
        string allSymbols = string.Join(";", options.ToArray());
        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, allSymbols);
    }

    public static List<string> GetCurrentDefineOption()
    {
        BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

        string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
        List<string> options = symbols.Split(DelimiterHelper.Semicolon).ToList();
        return options;
    }

}

