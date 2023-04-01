using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Friend;

public struct StrFriend
{
    public int time;
    public int randomRushNum;
    public int randomRushDecreaseHP;
    //������Ʈ�� �����ɶ�? ���� ����������? ���� ����������? min�� max�� random�� �̾� �� friend ���� �� ����.
    public float minDecreaseSpeed;
    public float maxDecreaseSpeed;
}
public class InGameManager : Manager<InGameManager>
{
    static public string Friend_ = "Friend_";

    public int ready; // ���� to ���� ���ð�.

    public int round=0;
    public int time;
    public int score;

    public int maxRound;
    public int maxScore;

    private Dictionary<int, StrFriend> friendXmlMap;
    private Dictionary<int, Friend> friendsObjMap;
    private StrFriend currentRoundData;
    private float decreaseSpeed;

    private Friend currentFriend;

    public Timer gameTimer;

    public override void Init()
    {
        base.Init();    
        GameManager.Instance.RegisterManager(this);


        friendXmlMap = new Dictionary<int, StrFriend>();


        FriendInit();

        RoundInitData(round);
        FriendDataResettings();
    }

    public override void ClearData()
    {
    }
    
    public void PreparationTime()
    {
        int temptime = 3;
        if(gameTimer == null)
        {
            gameTimer = new Timer("InGameManagerTimer", temptime, StartRound);
        }
        else
        {
            gameTimer.RefreshTime(temptime);
        }
        gameTimer._isStart = true;
    }

    public void FriendInit()
    {
        friendsObjMap = new Dictionary<int, Friend>();
        Friend[] tFriendObjs = UnityEngine.Object.FindObjectsOfType<Friend>();
        int key = 0;
        string temp;
        foreach (Friend tFriendObj in tFriendObjs)
        {
            key = tFriendObj.currentLocationPoint;
            temp = Friend_ + key;
            tFriendObj.friendTimer = new FriendTimer(temp, currentRoundData.time, GameOver_FriendTime, 0f);

            friendsObjMap.Add(key, tFriendObj);
        }
    }
    public void GameOver_FriendTime()
    {
        // ģ�� ���ٰ� Ÿ�� ���� �ƴ�.
        GameOver();
    }
    public void GameOver_TeacherMeet()
    {
        GameOver();
    }
    public void GameOver_TotalTime()
    {
        GameOver();
    }
    public void GameOver()
    {

    }
    public void RoundInitData(int tRound)
    {
        // ���尡 ���� �� ������ xml �����Ϳ��� �������ֱ�.
        StrFriend tValue;
        if (friendXmlMap.TryGetValue(tRound, out tValue))
        {
            currentRoundData = tValue;
        }
    }
    public void FriendDataResettings()
    {
        int tempj = 0;
        for (int i = 0; i < friendsObjMap.Count; i++)
        {
            tempj = i+1;
            if(tempj == 5)
            {
                tempj = 9; 
            }

            friendsObjMap[tempj].DecreaseSpeed = Random.Range(currentRoundData.minDecreaseSpeed, currentRoundData.maxDecreaseSpeed);

            friendsObjMap[tempj].friendTimer.RefreshTime(currentRoundData.time);
            friendsObjMap[tempj].friendTimer._isStart = false;
            friendsObjMap[tempj].friendTimer.refreshFriend = false;
        }
    }
    public void StartRound()    // �������� 3,2,1 ī��Ʈ ������ �����ϰ�����.
    {
        gameTimer._isStart = false;

        int tempj = 0;
        for (int i = 0; i < friendsObjMap.Count; i++)
        {
            tempj = i + 1;
            if (tempj == 5)
            {
                tempj = 9;
            }
            friendsObjMap[tempj].friendTimer._isStart = true;
            friendsObjMap[tempj].friendTimer.refreshFriend = true;
        }
    }

    public void NextRound()
    {
        // ���尡 Ŭ���� ���� ��  �� �Լ��� �����ؾ��Ѵ�.
        round++;
        RoundInitData(round);
        FriendDataResettings();

        PreparationTime();
    }

    public void TargetAngryStart(int tTarget)
    {
        //Ÿ���� �ƴ� �÷��̾ ���ڸ��� ������.
        if (tTarget == 5)
        {
            currentFriend = null;
            return;
        }
        //������ ���� �ִ� friend
        if(currentFriend != null)
        {
            currentFriend.friendTimer.refreshFriend = true;
        }

        Friend targetObj = friendsObjMap[tTarget];
        EFriendState targetState = targetObj.FriendState;

        //ĳ���Ͱ� ���� ������ Ÿ���� ����Ʈ�� ����?

        // Ÿ���� ���¸� Ȯ��,Ÿ���� decreasespeed�� Ȯ��
        switch (targetState) 
        {
            case EFriendState.Normal:
            targetObj.FriendState = EFriendState.LookatMe;
            break;

            case EFriendState.Over:
            if(targetObj.currentLocationPoint == PlayerManager.Instance.CurrentDirView)
            {
                GameOver_FriendTime();
                return;
            }
            break;
            default:
            break;
        }

        currentFriend = targetObj;
        currentFriend.friendTimer.refreshFriend = false;
    }
}
