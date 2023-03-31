using UnityEngine;

public abstract class Singleton<T> where T : class, new()
{
    protected static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
            }
            return _instance;
        }
    }

    public static bool IsCreatedInstance()
    {
        return (_instance != null);
    }
}

public abstract class SingletonWithMono<T> : MonoBehaviour where T : Component
{
    protected static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject go = new GameObject(typeof(T).ToString() + " (Singleton)");
                    _instance = go.AddComponent<T>();
                    if (!Application.isBatchMode)
                    {
                        if (Application.isPlaying)
                            DontDestroyOnLoad(go);
                    }
                }
            }
            return _instance;
        }
    }

    public static bool IsCreatedInstance()
    {
        return (_instance != null);
    }
}
