using UnityEngine;
using UnityEditor;
using System;

public static class Func
{
	public static int FastIndexOf(string source, string pattern)
	{
		if (string.IsNullOrEmpty(pattern))
			throw new ArgumentNullException();
		if (pattern.Length == 1) return source.IndexOf(pattern[0]);
		bool found;
		int limit = source.Length - pattern.Length + 1;
		if (limit < 1) return -1;

		char c0 = pattern[0];
		char c1 = pattern[1];

		int first = source.IndexOf(c0, 0, limit);
		while (first != -1)
		{
			if (source[first + 1] != c1)
			{
				first = source.IndexOf(c0, ++first, limit - first);
				continue;
			}
			found = true;
			for (int j = 2; j < pattern.Length; j++)
			{
				if (source[first + j] != pattern[j])
				{
					found = false;
					break;
				}
			}

			if (found) return first;
			first = source.IndexOf(c0, ++first, limit - first);
		}
		return -1;
	}

	public static string GetProjectPath()
	{
		int index = Application.dataPath.IndexOf("Assets");
		return Application.dataPath.Substring(0, index - 1);
	}
}

#if UNITY_EDITOR
public static class EditorPrefsEx
{
	public static bool GetBoolOrSet(string key, bool value)
	{
		if (EditorPrefs.HasKey(key))
		{
			return EditorPrefsEx.GetBool(key);
		}
		else
		{
			EditorPrefsEx.SetBool(key, value);
		}
		return value;
	}

	public static int GetIntOrSet(string key, int value)
	{
		if (EditorPrefs.HasKey(key))
		{
			return EditorPrefs.GetInt(key);
		}
		else
		{
			EditorPrefs.SetInt(key, value);
		}
		return value;
	}

	public static void SetBool(string key, bool value)
	{
		int intValue = value ? 1 : 0;
		EditorPrefs.SetInt(key, intValue);
	}

	public static bool GetBool(string key, bool defaultValue = false)
	{
		int defaultValueInt = (int)System.Convert.ChangeType(defaultValue, typeof(int));
		int value = EditorPrefs.GetInt(key, defaultValueInt);
		bool result = (value == 1) ? true : false;
		return result;
	}
}
#endif

public class DelimiterHelper
{
	public static readonly char[] Comma = new char[] { ',' };
	public static readonly char[] Period = new char[] { '.' };
	public static readonly char[] Pipe = new char[] { '|' };
	public static readonly char[] Semicolon = new char[] { ';' };
	public static readonly char[] Colon = new char[] { ':' };
	public static readonly char[] Newline = new char[] { '\n' };
	public static readonly char[] Underscore = new char[] { '_' };
}