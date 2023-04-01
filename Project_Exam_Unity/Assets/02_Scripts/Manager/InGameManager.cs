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
public class InGameManager : UpdateableManager<InGameManager>
{
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
        //�ð��� �ٷ��. �������� �ð�, �غ�ð�.



        //  - - - - - -
        // friend �ٷ�
        
        if (currentFriend == null)
        {
            return;
        }
        //currentFriend;
        // �����忡 ����ǥ�� ��������Ʈ �ٿ��� �ٷ糪? ���⼭?
        // �ٷ�� �Ǹ� ����ǥ�� ���� �����Ǿ�� �� ui������ ǥ���ϱ� ����
        // ��������Ʈ�� �Լ� ����� Friend�� ����ǥ �����ϴ� �Լ��� ���� ������.




    }

    public override void ClearData()
    {


    }

    public void FriendInit()
    {
        // �ʿ� ��ġ�� ģ���� ��������

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
        // ���尡 ���� �� ������ xml �����Ϳ��� �������ֱ�.
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
                bIsStart = false;
            }
            break;
            default:
            break;
        }

        if(bIsStart)
        {
            // update ����
            currentFriend = targetObj;
        }
        else
        {
            //over
        }
    }
}
