using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Friend : MonoBehaviour
{
    public enum EFriendState
    {
        Normal,
        LookAtMe,
        Over
    }

    private EFriendState friendState;
    public EFriendState FriendState => friendState;

    public int currentLocationPoint;
    private float decreaseSpeed;
    public float DecreaseSpeed { get { return decreaseSpeed; } set { decreaseSpeed = value; } }
    public FriendTimer friendTimer;
}
