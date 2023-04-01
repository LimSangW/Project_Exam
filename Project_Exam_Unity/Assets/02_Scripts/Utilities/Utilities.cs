using UnityEngine;
using UnityEditor;
using System;

public static class Utilities
{
    public static bool IsConnectedInternet
    {
        get
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }
    }
    
    public static Texture2D GetScreenShot(Camera cam, int targetLayers, int width, int height, Texture2D waterMark = null)
    {
        Texture2D _tex;
        //capture.
        cam.cullingMask = targetLayers;

        RenderTexture rt = new RenderTexture(width, height, 24);
        cam.targetTexture = rt;
        _tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        cam.Render();
        RenderTexture.active = rt;
        _tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);

        //destroy.
        cam.targetTexture = null;
        RenderTexture.active = null;
        UnityEngine.Object.Destroy(rt);

        Texture2D myTexture = ScaleTexture(_tex, width, height);

        AddWatermark(myTexture, waterMark);

        return myTexture;
    }

    static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
        Color[] rpixels = result.GetPixels(0);
        float incX = (1.0f / targetWidth);
        float incY = (1.0f / targetHeight);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * Mathf.Floor(px / targetWidth));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }

    static Texture2D AddWatermark(Texture2D target, Texture2D waterMark)
    {
        if (waterMark != null)
        {
            int startX = target.width - waterMark.width;

            for (int x = startX; x < target.width; x++)
            {

                for (int y = 0; y < target.height; y++)
                {
                    Color bgColor = target.GetPixel(x, y);
                    Color wmColor = waterMark.GetPixel(x - startX, y);

                    Color final_color = Color.Lerp(bgColor, wmColor, wmColor.a / 1.0f);

                    target.SetPixel(x, y, final_color);
                }
            }

            target.Apply();
        }

        return target;
    }

    public static string SimplifyValue(Int64 value)
    {
        string returnValue = string.Empty;

        if (value > 100000000)
        {
            returnValue = (value / 1000000).ToString("F1") + "M";
        }
        else if (value > 10000)
        {
            returnValue = (value / 1000).ToString("F1") + "K";
        }
        else
        {
            returnValue = value.ToString();
        }
        return returnValue;
    }

    public static GameObject CreateOrGetGameObject(string name, GameObject parent = null)
    {
        GameObject obj = GameObject.Find(name);
        if (obj == null)
            obj = CreateGameObject(name, parent);
        return obj;
    }

    public static GameObject CreateGameObject(string name, GameObject parent = null)
    {
        GameObject obj = new GameObject(name);

        if (parent != null)
            obj.transform.SetParent(parent.transform);

        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
        obj.transform.localScale = Vector3.one;

        return obj;
    }

    public static GameObject Instantiate(GameObject baseObj)
    {
        GameObject obj = GameObject.Instantiate(baseObj);
        obj.name = baseObj.name;

        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
        obj.transform.localScale = Vector3.one;
        return obj;
    }

    public static void SetChild(Transform trans, Transform parent)
    {
        trans.SetParent(parent);
        trans.localPosition = Vector3.zero;
        trans.localRotation = Quaternion.Euler(Vector3.zero);
        trans.localScale = Vector3.one;
    }
    
    public static Tuple<T1, T2>[] GetTupleArray<T1, T2>((T1, T2)[] arr)
    {
	    Tuple<T1, T2>[] tuples = new Tuple<T1, T2>[arr.Length];

	    for (int i = 0; i < arr.Length; ++i)
	    {
		    tuples[i] = new Tuple<T1, T2>(arr[i].Item1, arr[i].Item2);
	    }

	    return tuples;
    }

    public static Color GetColorByHex(string hexCode)
    {
        Color color;
        ColorUtility.TryParseHtmlString(hexCode, out color);
        return color;
    }
}

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