using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : ManagerWithMono<PlayerManager>
{
    
    //유저타입은 삭제 각 각 매니저를 따로 둘 예정인.
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
            //해당하는 타겟 friend의 상태를 변경.  바로 접근해서 함수 실행
            //GameManager.InGameManager.FriendManager.Friend[tDir].bIsState = false;
           // FriendManager.Instance.
        }
        CurrentDirView = tDir;
        //friend manager.

        //해당하는 타겟 friend의 상태를 변경.
        //GameManager.InGameManager.FriendManager.Friend[tDir].bIsState = false;
    }
    public void StopLook()
    {
        CurrentDirView = 5;
    }
}
