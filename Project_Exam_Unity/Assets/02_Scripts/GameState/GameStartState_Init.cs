using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartState_Init : GameStartState
{
    public GameStartState_Init(GameStartManager manager) : base(manager)
    {
    }
    
    public override void OnEnter()
    {
        stateManager.OnNextAction();
    }
}
