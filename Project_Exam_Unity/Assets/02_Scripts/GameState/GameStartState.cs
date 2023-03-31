public class GameStartState
{
    public enum GameStartStateType //게임의 상태 타입들, 먼저 도달하는 상태부터 작성
    {
        Init, //게임의 제일 최초 상태 (테스트 시에는)
        InitManager, //대부분의 Manager 초기화
        Length
    }

    protected GameStartManager stateManager;

    public GameStartState(GameStartManager manager)
    {
        stateManager = manager;
    }

    //해당 상태에 들어올 때
    public virtual void OnEnter()
    {

    }

    //해당 상태 유지시 업데이트
    public virtual void OnUpdate()
    {

    }

    //상태 해지 시
    public virtual void OnExit()
    {

    }

    //해당 상태에서 터치시
    public virtual void OnClickTouchToScreen()
    {

    }
}
