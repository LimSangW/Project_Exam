using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public enum UserState { Normal, Cheating, Over};
    public UserState userState;

    private int currentDirView = 5;
    public int CurrentDirView
    {
        get
        {
            if (currentDirView == 5)
            {
                userState = UserState.Normal;
                StopLook();
            }
            else
            {
                userState = UserState.Cheating;
                LookAtFriend(currentDirView);
            }
            
            return currentDirView;
        }
        set
        {
            currentDirView = value;
        }
    }

    public void LookAtFriend(int tDir)
    {
        if(CurrentDirView == tDir) return;

        Transform target = InGameManager.Instance.GetCurrentFriend(tDir).transform;
        Vector3 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        
        CurrentDirView = tDir;
    }
    public void StopLook()
    {
        CurrentDirView = 5;
    }
}