using Project.UIFramework;
using Project.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class GameStartState_InitManager : GameStartState
{
    public GameStartState_InitManager(GameStartManager manager) : base(manager)
    {

    }

    public override void OnEnter()
    {
        UIManager.Instance.Init();
        DataManager.Instance.Init();

        stateManager.OnNextAction();

        Signals.Get<NavigateToWindowSignal>().Dispatch(ScreenIds.MainPlayWindow, null);
    }
}
