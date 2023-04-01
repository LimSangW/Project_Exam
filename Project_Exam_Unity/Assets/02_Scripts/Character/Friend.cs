using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friend : MonoBehaviour
{
    public enum EFriendState { Normal,LookatMe,Danger,Over}
    private EFriendState friendState;
    public EFriendState FriendState
    {
        get { return friendState; }
        set { friendState = value; }
    }

    // 시간의 최대값.
    // 시간 현재 .


    // Start is called before the first frame update
    public bool bIsLookatMe = false;
    public int currentLocationPoint;
    
    //player가 해당 friend를 보고 있을 때 hp가 줄어드는 속도. 
    private float decreaseSpeed;
    public float DecreaseSpeed { get { return decreaseSpeed; } set { decreaseSpeed = value; } }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      
    }

}
