using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    public string KeyName => _keyName;
    protected string _keyName;
    protected double _time;
    protected Action _callback;
    protected bool _isTimeOver = false;

    public bool _isStart = false;

    public bool IsTimeOver => _isTimeOver;

    public double Time => _time;

    public Timer(string keyName, double time, Action timeoverCallback)
    {
        _isTimeOver = false;
        _keyName = keyName;
        _time = time;
        _callback = timeoverCallback;
        TimerManager.Instance.AddTimer(this);
    }

    // �ð��� �ٽ� ���� �Լ� �����.
    public virtual void OnUpdate()
    {
        if (_isTimeOver == true)
            return;

        if(_isStart)
        {
            _time -= UnityEngine.Time.unscaledDeltaTime;
            if (_time <= 0)
            {
                _isTimeOver = true;

                _callback?.Invoke();
            }
        }

    }

    public virtual void RefreshTime(double time)
    {
        _time = time;
    }

    public virtual void Release()
    {
        TimerManager.Instance.RemoveTimer(this);
    }
}

public class Counter : Timer
{
    double _limitTime;

    public Counter(string keyName, double time, double limitTime, Action timeoverCallback) : base(keyName, time, timeoverCallback)
    {
        _limitTime = limitTime;
    }

    public override void OnUpdate()
    {
        _time += UnityEngine.Time.unscaledDeltaTime;
        if (_isTimeOver == false)
        {
            if (_time >= _limitTime)
            {
                _isTimeOver = true;
                _callback?.Invoke();
            }
        }
    }
}

public class FriendTimer : Timer 
{
    public bool refreshFriend = false;
    private double _maxTime;
    private float recoveryTime = 0;

    public FriendTimer(string keyName, double time, Action timeoverCallback, float _recoveryTime) : base(keyName, time, timeoverCallback)
    {
        _maxTime = _time;
        recoveryTime = _recoveryTime;
    }

    public override void OnUpdate()
    {
        if (_isTimeOver == true)
            return;

        if(_isStart)
        {
            if (refreshFriend)
            {
                _time += UnityEngine.Time.unscaledDeltaTime;

                //InGameManager.Instance.FriendRecovery(KeyName, recoveryTime);
                if (_time >= _maxTime)
                {
                    refreshFriend = false;
                    _isStart = false;
                }
            }
            else
            {
                _time -= UnityEngine.Time.unscaledDeltaTime;
                if (_time <= 0)
                {
                    _isTimeOver = true;
                    //TimerManager.Instance.RemoveTimer(this);

                    _callback?.Invoke();
                }
            }
        }
    }
}


public class TimerManager : SingletonWithMono<TimerManager>
{
    Dictionary<string, Timer> _timers;

    public void Awake()
    {
        _timers = new Dictionary<string, Timer>();
    }

    public void AddTimer(Timer timer)
    {
        if (_timers.ContainsKey(timer.KeyName) == false)
        {
            _timers.Add(timer.KeyName, timer);
        }
    }

    public void RemoveTimer(Timer timer)
    {
        if (_timers.ContainsKey(timer.KeyName) == true)
        {
            _timers.Remove(timer.KeyName);
        }
    }

    public void RemoveTimer(string keyName)
    {
        if (_timers.ContainsKey(keyName) == true)
            _timers.Remove(keyName);
    }

    public void Update()
    {
        foreach(KeyValuePair<string, Timer> kv in _timers)
        {
            kv.Value.OnUpdate();
        }
    }

    public static Tuple<int, Tuple<string, string>[]> RemainTime(double time, 
        int lessThan_1Minute_Index = 56, 
        int lessThan_60Minute_Index = 55,
        int lessThan_24Hour_Index = 54,
        int excess_24Hour_Index = 53)
    {
        int stringIndex = -1;
        Tuple<string, string>[] parameter = null;

        if (time < 60)
        {
            stringIndex = lessThan_1Minute_Index;
            parameter = Utilities.GetTupleArray<string, string>(new[] { ("-1", "-1") });
            //1�й̸�
            //returnValue = "1�й̸�";
        }
        else if(time < 3600)
        {
            //1�� �̻� 60�� �̸�.
            //returnValue = ((int)time / 60).ToString();
            int minuteValue = (int)time / 60;
            int secondValue = (int)time % 60;
            stringIndex = lessThan_60Minute_Index;
            parameter = Utilities.GetTupleArray<string, string>(new[] 
            {
                ("0", minuteValue.ToString()), 
                ("1", secondValue.ToString()) 
            });
        }
        else if(time < 86400)
        {
            int timeValue = (int)time / 3600;
            int minuteValue = ((int)time - timeValue * 3600) / 60;
            stringIndex = lessThan_24Hour_Index;
            parameter = Utilities.GetTupleArray<string, string>(new[] 
            {
                ("0", timeValue.ToString()),
                ("1", minuteValue.ToString())
            });
        }
        else
        {
            int dayValue = (int)time / 86400;
            stringIndex = excess_24Hour_Index;
            parameter = Utilities.GetTupleArray<string, string>(new[] 
            {
                ("0", dayValue.ToString()),
            });
        }

        return new Tuple<int, Tuple<string, string>[]>(stringIndex, parameter);
    }
}
