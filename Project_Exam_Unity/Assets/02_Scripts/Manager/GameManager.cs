using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : SingletonWithMono<GameManager>
{
    private List<IManager> managerList = new List<IManager>();
    private List<IUpdateableManager> updateableManagerList = new List<IUpdateableManager>();

    private void Update()
    {
        for (int i = 0; i < updateableManagerList.Count; ++i)
        {
            updateableManagerList[i].OnUpdate();
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (GameSceneManager.Instance.IsTitleScene == true)
            GameStartManager.Instance.OnStartToTitle();
        else
            GameStartManager.Instance.OnStartToMain();
    }

    public void OnGameRestart()
    {
        ClearData();

        if (GameSceneManager.Instance.IsTitleScene == true)
            GameStartManager.Instance.OnStartToTitle();
        else if (GameSceneManager.Instance.IsMainScene == true)
            GameSceneManager.Instance.ChangeScene(GameSceneManager.MainSceneName, () => GameStartManager.Instance.OnStartToTitle());
    }

    public void RegisterManager(IManager manager)
    {
        if(managerList.Contains(manager))
        {
            DebugEx.LogWarning("[GameManager] Already Contained Manager");
            return;
        }

        managerList.Add(manager);
    }

    public void UnRegisterManager(IManager manager)
    {
        managerList.Remove(manager);
    }

    public void RegisterUpdateableManager(IUpdateableManager updateableManager)
    {
        if (updateableManagerList.Contains(updateableManager))
        {
            DebugEx.LogWarning("[GameManager] Already Contained UpdateableManager");
            return;
        }

        updateableManagerList.Add(updateableManager);
    }

    public void UnRegisterUpdateableManager(IUpdateableManager updateableManager)
    {
        updateableManagerList.Remove(updateableManager);
    }

    public void ClearData()
    {
        for (int i = 0; i < managerList.Count; i++)
        {
            managerList[i].ClearData();
        }
        managerList.Clear();

        for (int i = 0; i < updateableManagerList.Count; i++)
        {
            updateableManagerList[i].ClearData();
        }
        updateableManagerList.Clear();
    }
}