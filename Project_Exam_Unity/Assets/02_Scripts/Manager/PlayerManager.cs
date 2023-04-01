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
            if (CurrentDirView == 5)
                return;


            // ��� ����.  
        }
        CurrentDirView = tDir;
    }
    public void StopLook()
    {
        CurrentDirView = 5;
    }


}
