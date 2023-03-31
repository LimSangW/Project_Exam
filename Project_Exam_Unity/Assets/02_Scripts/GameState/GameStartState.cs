public class GameStartState
{
    public enum GameStartStateType //������ ���� Ÿ�Ե�, ���� �����ϴ� ���º��� �ۼ�
    {
        Init, //������ ���� ���� ���� (�׽�Ʈ �ÿ���)
        InitManager, //��κ��� Manager �ʱ�ȭ
        Length
    }

    protected GameStartManager stateManager;

    public GameStartState(GameStartManager manager)
    {
        stateManager = manager;
    }

    //�ش� ���¿� ���� ��
    public virtual void OnEnter()
    {

    }

    //�ش� ���� ������ ������Ʈ
    public virtual void OnUpdate()
    {

    }

    //���� ���� ��
    public virtual void OnExit()
    {

    }

    //�ش� ���¿��� ��ġ��
    public virtual void OnClickTouchToScreen()
    {

    }
}
