using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameStartManager : UpdateableManager<GameStartManager>
{
    private Dictionary<GameStartState.GameStartStateType, GameStartState> statesDic;
    private List<GameStartState.GameStartStateType> actionTypeList;
    private GameStartState currentState;

    private bool isInitialized = false;

    public GameStartManager()
    {
        Init();
        statesDic = new Dictionary<GameStartState.GameStartStateType, GameStartState>();
        
        statesDic.Add(GameStartState.GameStartStateType.Init, new GameStartState_Init(this));
        statesDic.Add(GameStartState.GameStartStateType.InitManager, new GameStartState_InitManager(this));
        
        actionTypeList = new List<GameStartState.GameStartStateType>();
    }

    public void OnStartToTitle()
    {
        Init();
        actionTypeList.Add(GameStartState.GameStartStateType.Init);
        actionTypeList.Add(GameStartState.GameStartStateType.InitManager);

        OnNextAction();
    }

    public void OnStartToMain()
    {
        Init();
        actionTypeList.Add(GameStartState.GameStartStateType.Init);
        actionTypeList.Add(GameStartState.GameStartStateType.InitManager);

        OnNextAction();
    }

    public void OnNextAction()
    {
        if (actionTypeList.Count <= 0)
            return;

        var nextType = actionTypeList[0];
        if (statesDic.ContainsKey(nextType) == true)
        {
            actionTypeList.Remove(nextType);
            ChangeState(nextType);
        }
    }

    public void AddAction(GameStartState.GameStartStateType type)
    {
        actionTypeList.Add(type);
    }

    public override void ClearData()
    {
        isInitialized = false;
        statesDic.Clear();
        actionTypeList.Clear();
        currentState = null;
    }

    public override void Init()
    {
        if (isInitialized == false)
        {
            isInitialized = true;
            base.Init();
        }
    }

    public override void OnUpdate()
    {
        currentState?.OnUpdate();
    }

    public void ChangeState(GameStartState.GameStartStateType type)
    {
        if (statesDic.ContainsKey(type) == true)
        {
            currentState?.OnExit();
            currentState = statesDic[type];
            DebugEx.Log("[GameStartManager] Change GameStartState : " + type);
            currentState?.OnEnter();
        }
        else
            DebugEx.LogError(string.Format("[GameStartManager] Not ContainsKey : {0}", type));
    }

    public void OnClickTouchToScreen()
    {
        currentState.OnClickTouchToScreen();
    }
}
