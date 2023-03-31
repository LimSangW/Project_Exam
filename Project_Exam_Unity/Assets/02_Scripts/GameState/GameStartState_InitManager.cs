using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class GameStartState_InitManager : GameStartState
{
    public GameStartState_InitManager(GameStartManager manager) : base(manager)
    {
        DataManager.Instance.Init();
        StageManager.Instance.Init();

        stateManager.OnNextAction();
    }
}