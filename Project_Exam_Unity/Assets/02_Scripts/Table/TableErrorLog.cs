using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class TableErrorLog : EditorWindow
{
    private static List<string> errorLogList = new List<string>();
    private Vector2 scrollPos = Vector2.zero;

    public static void ShowWindow(string errorLog)
    {
        errorLogList.Add(errorLog);
        TableErrorLog window = (TableErrorLog)EditorWindow.GetWindow(typeof(TableErrorLog));
        window.minSize = new Vector2(700, 1000);
    }

    private void OnGUI()
    {
        if (GUILayout.Button("ClearLog"))
        {
            errorLogList.Clear();
        }

        var boldText = new GUIStyle(GUI.skin.label);
        boldText.fontStyle = FontStyle.Bold;
        boldText.fontSize = 12;
        boldText.richText = true;

        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));
        for (int i = 0; i < errorLogList.Count; ++i)
        {
            EditorGUILayout.LabelField(errorLogList[i], boldText);
        }
        EditorGUILayout.EndScrollView();
    }

    private void OnDestroy()
    {
        errorLogList.Clear();
    }
}

#endif