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
    //컴포넌트에 부착될때? 게임 시작했을때? 라운드 시작했을때? min과 max로 random값 뽑아 각 friend 마다 값 설정.
    public float minDecreaseSpeed;
    public float maxDecreaseSpeed;
}
public class InGameManager : UpdateableManager<InGameManager>
{
    public int ready; // 라운드 to 라운드 대기시간.

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
    private bool bIsStart = false;

    public override void Init()
    {
        GameManager.Instance.RegisterUpdateableManager(this);


        friendXmlMap = new Dictionary<int, StrFriend>();


        FriendInit();

        RoundInitData(round);
        FriendDataResettings();
    }

    public override void OnUpdate()
    {
        //시간을 다룬다. 게임진행 시간, 준비시간.



        //  - - - - - -
        // friend 다룸
        
        if (currentFriend == null)
        {
            return;
        }
        //currentFriend;
        // 프렌드에 느낌표를 스프라이트 붙여서 다루나? 여기서?
        // 다루게 되면 느낌표가 점점 해제되어가는 건 ui적으로 표현하기 위해
        // 델리게이트로 함수 등록해 Friend의 느낌표 조절하는 함수를 점점 낮추자.




    }

    public override void ClearData()
    {


    }

    public void FriendInit()
    {
        // 맵에 배치된 친구들 데려오기

        friendsObjMap = new Dictionary<int, Friend>();
        Friend[] tFriendObjs = UnityEngine.Object.FindObjectsOfType<Friend>();
        int key = 0;
        foreach (Friend tFriendObj in tFriendObjs)
        {
            key = tFriendObj.currentLocationPoint;
            friendsObjMap.Add(key, tFriendObj);
        }
    }
    public void RoundInitData(int tRound)
    {
        // 라운드가 갱신 될 때마다 xml 데이터에서 세팅해주기.
        StrFriend tValue;
        if (friendXmlMap.TryGetValue(tRound, out tValue))
        {
            currentRoundData = tValue;
        }
    }
    public void FriendDataResettings()
    {
        for (int i = 0; i < friendsObjMap.Count; i++)
        {
            friendsObjMap.Values.ElementAt(i).DecreaseSpeed = Random.Range(currentRoundData.minDecreaseSpeed, currentRoundData.maxDecreaseSpeed);
        }
    }

    public void NextRound()
    {
        round++;
        RoundInitData(round);
        FriendDataResettings();
    }

    public void TargetAngryStart(int tTarget)
    {
        if(tTarget == 5)
        {
            bIsStart = false;
            currentFriend = null;
            return;
        }

        bIsStart = true;

        Friend targetObj = friendsObjMap.Values.ElementAt(tTarget);
        EFriendState targetState = targetObj.FriendState;

        //캐릭터가 보는 방향이 타겟의 포인트와 같냐?

        // 타겟의 상태를 확인,타겟의 decreasespeed를 확인
        switch (targetState) 
        {
            case EFriendState.Normal:
            targetObj.FriendState = EFriendState.LookatMe;
            break;

            case EFriendState.Over:
            if(targetObj.currentLocationPoint == PlayerManager.Instance.CurrentDirView)
            {
                bIsStart = false;
            }
            break;
            default:
            break;
        }

        if(bIsStart)
        {
            // update 시작
            currentFriend = targetObj;
        }
        else
        {
            //over
        }
    }
}
