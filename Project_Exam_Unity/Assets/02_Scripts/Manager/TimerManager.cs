using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    protected string _keyName;
    protected double _time;
    protected Action timeoverCallback;
    protected bool _isTimeOver = false;

    public double MaxTime { get; private set; }
    public bool _isStart = false;
    public bool IsTimeOver => _isTimeOver;
    public string KeyName => _keyName;
    public double Time => _time;
    public Action<double> updateCallback;

    public Timer(string keyName, double time, Action timeoverCallback)
    {
        _isTimeOver = false;
        _keyName = keyName;
        _time = time;
        MaxTime = time;
        this.timeoverCallback = timeoverCallback;
        TimerManager.Instance.AddTimer(this);
    }

    public virtual void OnUpdate()
    {
        if (_isTimeOver == true)
            return;

        if(_isStart)
        {
            _time -= UnityEngine.Time.unscaledDeltaTime;
            updateCallback?.Invoke(_time);
            
            if (_time <= 0)
            {
                _isTimeOver = true;
                timeoverCallback?.Invoke();
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

    public Counter(string keyName, double time, double limitTime, Action timeoverCallback) 
        : base(keyName, time, timeoverCallback)
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
                timeoverCallback?.Invoke();
            }
        }
    }
}

public class FriendTimer : Timer 
{
    public bool refreshFriend = false;
    private float recoveryTime = 0;

    public FriendTimer(string keyName, double time, Action timeoverCallback, float _recoveryTime) : base(keyName, time, timeoverCallback)
    {
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
                if (_time >= MaxTime)
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

                    timeoverCallback?.Invoke();
                }
            }
        }
    }
}


public class TimerManager : SingletonWithMono<TimerManager>
{
    private Dictionary<string, Timer> _timers;

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

    public Timer GetTimer(string key)
    {
        if (_timers.ContainsKey(key))
            return _timers[key];
        return null;
    }
}
