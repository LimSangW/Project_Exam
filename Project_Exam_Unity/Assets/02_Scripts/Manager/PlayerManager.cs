using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : ManagerWithMono<PlayerManager>
{
    
    //����Ÿ���� ���� �� �� �Ŵ����� ���� �� ������.
    public enum UserState { Nnormal, Cheating, Over};
    public UserState userState;

    public Player player;

    private int currentDirView = 5;
    public int CurrentDirView
    {
        get
        {
            if (currentDirView == 5)
            {
                userState = UserState.Nnormal;
            }
            else
            {
                userState = UserState.Cheating;
            }
            return currentDirView;
        }
        set
        {
            currentDirView = value;
        }
    }

    public override void ClearData()
    {
    }
    
    public override void Init()
    {
        base.Init();
        player = new Player();
        player = UnityEngine.Object.FindObjectOfType<Player>(); 

    }

    public void LookatFriend(int tDir)
    {
        if(CurrentDirView == tDir)
        {
            //�ش��ϴ� Ÿ�� friend�� ���¸� ����.  �ٷ� �����ؼ� �Լ� ����
            //GameManager.InGameManager.FriendManager.Friend[tDir].bIsState = false;
           // FriendManager.Instance.
        }
        CurrentDirView = tDir;
        //friend manager.

        //�ش��ϴ� Ÿ�� friend�� ���¸� ����.
        //GameManager.InGameManager.FriendManager.Friend[tDir].bIsState = false;
    }
    public void StopLook()
    {
        CurrentDirView = 5;
    }
}
