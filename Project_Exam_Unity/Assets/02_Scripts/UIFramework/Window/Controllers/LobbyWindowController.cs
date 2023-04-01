using System.Collections;
using System.Collections.Generic;
using Project.UIFramework;
using Project.Utils;
using UnityEngine;

public class LobbyWindowController : AWindowController
{
    protected override void OnPropertiesSet()
    {
        
    }

    protected override void WhileHiding()
    {
        
    }
    
    public void OnClickStartButton()
    {
        Signals.Get<NavigateToWindowSignal>().Dispatch(ScreenIds.MainPlayWindow, null);
        InGameManager.Instance.PreparationTime();
    }

    public void OnClickOverButton()
    {
        Application.Quit();
    }

    private void DoInTransitionFinished(IUIScreenController screen)
    {
    }
}
