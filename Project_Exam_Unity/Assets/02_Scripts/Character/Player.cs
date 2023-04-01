using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public enum UserState { Normal, Cheating, Over};
    private UserState userState;
    [SerializeField] private GameObject seekArea;

    private int currentDirView = 5;

    public void PlayerAction(int dir)
    {
        if (dir == 5)
        {
            userState = UserState.Normal;
            StopLook();
        }
        else
        {
            userState = UserState.Cheating;
            LookAtFriend(dir);
        }
        
        currentDirView = dir;
    }

    private void Start()
    {
        PlayerAction(currentDirView);
    }

    private void LookAtFriend(int tDir)
    {
        if(!seekArea.activeSelf) seekArea.SetActive(true);
        if(currentDirView == tDir) return;

        Transform target = InGameManager.Instance.GetCurrentFriend(tDir).transform;
        Vector3 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    private void StopLook()
    {
        if(seekArea.activeSelf) seekArea.SetActive(false);
    }
}