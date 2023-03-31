using UnityEngine;

public interface IManager
{
	void Init();

	void ClearData();
}

public interface IUpdateableManager
{
	void Init();

	void OnUpdate();

	void ClearData();
}

public abstract class Manager<T> : IManager where T : class, new()
{
	protected static T instance;

	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new T();
			}
			return instance;
		}
	}

	public virtual void Init()
	{
        GameManager.Instance.RegisterManager(this);
    }

    public abstract void ClearData();
}

public abstract class ManagerWithMono<T> : MonoBehaviour, IManager where T : Component
{
	protected static T instance;

	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<T>();
				if (instance == null)
				{
					GameObject go = new GameObject(typeof(T).ToString() + " (Singleton)");
					instance = go.AddComponent<T>();
					if (Application.isBatchMode == false)
					{
						if (Application.isPlaying)
						{
							DontDestroyOnLoad(go);
						}
					}
				}
			}
			return instance;
		}
	}

	public virtual void Init()
	{
        GameManager.Instance.RegisterManager(this);
    }

    public abstract void ClearData();
}

public abstract class UpdateableManager<T> : IUpdateableManager where T : class, new()
{
	protected static T instance;

	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new T();
			}
			return instance;
		}
	}

	public virtual void Init()
	{
        GameManager.Instance.RegisterUpdateableManager(this);
    }

    public abstract void OnUpdate();

	public abstract void ClearData();
}

public abstract class UpdateableManagerWithMono<T> : MonoBehaviour, IUpdateableManager where T : Component
{
	protected static T instance;

	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<T>();
			}
			return instance;
		}
	}

	public virtual void Init()
	{
        GameManager.Instance.RegisterUpdateableManager(this);
    }

    public abstract void OnUpdate();

	public abstract void ClearData();
}