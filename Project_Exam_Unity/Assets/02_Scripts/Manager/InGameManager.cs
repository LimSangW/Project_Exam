using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FriendRoundInfo
{
    public float FriendHp;
    public float DontSee;
    public float FriendRecoveryDuration;

    public FriendRoundInfo(TRFriend friend)
    {
        FriendHp = friend.FriendHp;
        DontSee = friend.DontSee;
        FriendRecoveryDuration = friend.FriendRecoveryDuration;
    }
}

public class InGameManager : ManagerWithMono<InGameManager>
{
    static public string Friend_ = "Friend_";

    public int ready;

    public int round = 0;
    public int time;
    public int score;

    public int maxRound;
    public int maxScore;

    private Dictionary<int, Friend> friendsDic;
    private Dictionary<int, FriendRoundInfo> friendRoundInfoDic = new Dictionary<int, FriendRoundInfo>();
    private float decreaseSpeed;

    private Friend currentFriend;

    [SerializeField] private Transform friendGroup;
    public Player MainPlayer;
    public Timer GameTimer;


    public override void Init()
    {
        base.Init();    
        GameManager.Instance.RegisterManager(this);

        FriendInit();
        InitData();
        FriendDataResettings();
    }

    public override void ClearData()
    {
        friendRoundInfoDic.Clear();
    }
    
    public void PreparationTime()
    {
        int temptime = 3;
        if(GameTimer == null)
        {
            GameTimer = new Timer("InGameManagerTimer", temptime, StartRound);
        }
        else
        {
            GameTimer.RefreshTime(temptime);
        }
        GameTimer._isStart = true;
    }

    public void FriendInit()
    {
        friendsDic = new Dictionary<int, Friend>();
        Friend[] friends = friendGroup.GetComponentsInChildren<Friend>();

        for (int i = 0; i < friends.Length; i++)
        {
            Friend thisFriend = friends[i];
            int thisFriendLocation = thisFriend.currentLocationPoint;
            
            friendsDic.Add(thisFriendLocation, friends[i]);
            thisFriend.friendTimer = new FriendTimer(Friend_ + thisFriendLocation, 180f, GameOver_FriendTime, 0f);
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
    
    private void InitData()
    {
        List<TRFriend> friendData = DataManager.Instance.GetRecords<TRFriend>(TableType.Friend);

        for (int i = 0; i < friendData.Count; i++)
            friendRoundInfoDic.Add((int)friendData[i].PlayTime, new FriendRoundInfo(friendData[i]));
    }

    public Friend GetCurrentFriend(int num)
    {
        currentFriend = friendsDic[num];
        return currentFriend;
    }
    
    public void FriendDataResettings()
    {

    }
    
    public void StartRound()
    {
        GameTimer._isStart = false;

        int tempj = 0;
        for (int i = 0; i < friendsDic.Count; i++)
        {
            tempj = i + 1;
            if (tempj == 5)
            {
                tempj = 9;
            }
            friendsDic[tempj].friendTimer._isStart = true;
            friendsDic[tempj].friendTimer.refreshFriend = true;
        }
    }
}
