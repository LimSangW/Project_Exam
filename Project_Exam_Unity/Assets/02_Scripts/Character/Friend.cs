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

    // �ð��� �ִ밪.
    // �ð� ���� .


    // Start is called before the first frame update
    public bool bIsLookatMe = false;
    public int currentLocationPoint;
    
    //player�� �ش� friend�� ���� ���� �� hp�� �پ��� �ӵ�. 
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
