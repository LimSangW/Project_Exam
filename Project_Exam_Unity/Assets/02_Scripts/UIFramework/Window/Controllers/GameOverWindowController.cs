using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Project.UIFramework;
using Project.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameOverWindowController : AWindowController
{
    [SerializeField] private TextMeshProUGUI scoreText;
    
    protected override void OnPropertiesSet()
    {
        scoreText.SetText(InGameManager.Instance.Score.ToString());
    }

    public void OnClickRetry()
    {
        Signals.Get<NavigateToWindowSignal>().Dispatch(ScreenIds.MainPlayWindow, null);
    }

    public void OnClickExit()
    {
        Application.Quit();
    }
}
