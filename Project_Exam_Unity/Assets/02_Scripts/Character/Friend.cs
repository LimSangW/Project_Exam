using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public int currentLocationPoint;
    
    //player�� �ش� friend�� ���� ���� �� hp�� �پ��� �ӵ�. 
    private float decreaseSpeed;
    public float DecreaseSpeed { get { return decreaseSpeed; } set { decreaseSpeed = value; } }

    // �ð��� �ִ밪.
    // �ð� ���� .
    public FriendTimer friendTimer;// = new Timer(Friend_1,60,);



    private int currentHP;
    public int CurrentHP
    {
        get { return currentHP; }
        set { currentHP = value; }
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      
    }

}
