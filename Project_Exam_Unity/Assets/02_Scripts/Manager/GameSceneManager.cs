using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : SingletonWithMono<GameSceneManager>
{
    public static string TitleSceneName = "Title";
    public static string MainSceneName = "Main";
    Action callback;

    public bool IsTitleScene
    {
        get
        {
            Scene scene = SceneManager.GetActiveScene();
            return scene.name == TitleSceneName;
        }
    }

    public bool IsMainScene
    {
        get
        {
            Scene scene = SceneManager.GetActiveScene();
            return scene.name == MainSceneName;
        }
    }

    public void ChangeScene(string sceneName, Action callback)
    {
        this.callback = callback;
        StartCoroutine(coroutine(sceneName));
    }

    IEnumerator coroutine(string sceneName)
    {
        AsyncOperation asyncOper = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncOper.isDone)
        {
            yield return null;
        }
        callback?.Invoke();
    }
}
