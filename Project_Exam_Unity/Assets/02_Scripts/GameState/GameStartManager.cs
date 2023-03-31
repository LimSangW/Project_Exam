using System.Collections.Generic;
using System.Linq;

public class GameStartManager : UpdateableManager<GameStartManager> // 게임 시작 플로우 관리 Manager
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

        //Add 된 순서대로 ChangeState
        var nextType = actionTypeList[0];
        if (statesDic.ContainsKey(nextType) == true)
        {
            //ChangeState후에는 필요없고 다음 것 탐색을 위해 Remove
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
