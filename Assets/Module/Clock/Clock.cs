/*
 * 脚本名称：Clock
 * 项目名称：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2018-05-21 12:02:24
 * 脚本作用：
*/

using System;
using System.Collections.Generic;
using UnityEngine;


public class Clock
{
    public static List<Clock> clockList = new List<Clock>();

    public static void UpdateLilst()
    {
        for (int i = 0; i < clockList.Count; i++)
        {
            clockList[i].Update();
        }
    }

    public float intervalTime { get; set; }
    public float realTargetTime { get; set; }
    public string key { get; set; }
    public float interval { get; set; } = -1;
    public float targetTime { get; set; }
    public bool isActive { get; set; }
    public bool isTimeScale { get; set; } = true;
    public int loop { get; set; } = 1;
    
    public event Action onComplete;
    public event Action onStart;
    public event Action onPause;
    public event Action onStop;
    public event Action onInterval;

    public Clock(string key)
    {
        this.key = key;
        if (!clockList.Contains(this))
        {
            clockList.Add(this);
        }
    }

    public Clock()
    {
        if (!clockList.Contains(this))
        {
            clockList.Add(this);
        }
    }

    public void Start()
    {
        isActive = true;
        if (onStart != null) onStart();
    }

    public void Pause()
    {
        isActive = false;
        onPause?.Invoke();
    }

    public void Stop()
    {
        isActive = false;
        targetTime = 0;
        onStop?.Invoke();
        clockList.Remove(this);
    }

    public void Restart()
    {
        isActive = true;
        targetTime = 0;
    }

    public void Update()
    {
        if (isActive)
        {
            float delatime = (isTimeScale ? Time.deltaTime : Time.unscaledDeltaTime);
            realTargetTime += delatime;
            intervalTime += delatime;
            if (intervalTime >= interval && interval != -1)
            {
                intervalTime = 0;
                onInterval?.Invoke();
            }

            if (realTargetTime >=targetTime)
            {
                if (loop == 1)
                {
                    Stop();
                }
                else if (loop > 1)
                {
                    loop--;
                    Restart();
                }
                else if (loop == -1)
                {
                    Restart();
                }

                onComplete?.Invoke();
            }
        }
    }
}