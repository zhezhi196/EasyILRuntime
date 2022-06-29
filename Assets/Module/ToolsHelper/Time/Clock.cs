/*
 * 脚本名称：Clock
 * 项目名称：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2018-05-21 12:02:24
 * 脚本作用：
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public class Clock : IProcess, Identify
    {
        #region Static

        private static List<Clock> clockList = new List<Clock>();

        public static void Update()
        {
            for (int i = 0; i < clockList.Count; i++)
            {
                clockList[i].OnUpdate();
            }
        }
        
        public static void KillAll()
        {
            clockList.Clear();
        }

        public static void Kill(object id)
        {
            for (int i = 0; i < clockList.Count; i++)
            {
                if (clockList[i].ID == id)
                {
                    clockList[i].Stop();
                }
            }
        }

        public static Clock GetClockByID(object id)
        {
            for (int i = 0; i < clockList.Count; i++)
            {
                if (clockList[i].ID == id)
                {
                    return clockList[i];
                }
            }

            return new Clock().SetID(id);
        }

        public static Clock GetClockByID(object id, float time)
        {
            for (int i = 0; i < clockList.Count; i++)
            {
                if (clockList[i].ID == id)
                {
                    return clockList[i];
                }
            }
            
            return new Clock(time).SetID(id);
        }

        #endregion

        #region PrivateIndex

        private int currentIntervalIndex;
        private int currentSecond;
        private int currentMinute;
        private int currentHour;
        
        private FloatField _currentTimeEncryption;
        private float _currentTime; 

        #endregion

        #region 属性

        public object Current
        {
            get { return currentTime; }
        }

        public object ID { get; set; }
        public Func<bool> listener { get; set; }

        public bool autoKill { get; set; } = true;
        
        public bool encryption { get; set; }

        /// <summary>
        /// 当前已经进行的时间
        /// </summary>
        public float currentTime
        {
            get { return encryption ? _currentTimeEncryption : _currentTime; }
            set
            {
                if (encryption)
                {
                    _currentTimeEncryption = new FloatField(value);
                }
                else
                {
                    _currentTime = value;
                }
            }
        }

        public float percent
        {
            get { return Mathf.Clamp01(currentTime / targetTime); }
        }

        public float remainTime
        {
            get { return targetTime - currentTime; }
        }
        /// <summary>
        /// 默认周期,到达这个数值会触发complete
        /// </summary>
        public float targetTime { get; set; }
        public ITimeCtrl owner { get; set; }
        /// <summary>
        /// 是否忽略时间缩放影响,和dotween的SetUpdate函数传的参数一样一样的
        /// </summary>
        public bool ignoreTimescale { get; set; }

        public bool ignorePause { get; set; }

        public bool isActive
        {
            get { return clockList.Contains(this); }
        }
        /// <summary>
        /// 间隔时间
        /// </summary>
        public float[] intervalTime { get; set; }

        public DateTime timeStamp;

        #endregion

        #region 事件

        public event Action onStart;
        public event Action onRestart;
        public event Action onPause;
        public event Action onComplete;
        public event Action onStop;
        public event Action<float> onUpdate;
        public event Action<int> onInterval;
        public event Action<int> onSecond;
        public event Action<int> onMinute;
        public event Action<int> onHour; 

        #endregion

        public Clock(float time)
        {
            targetTime = time;
            timeStamp = TimeHelper.now;
        }
        
        public Clock()
        {
            targetTime = float.MaxValue;
            timeStamp = TimeHelper.now;
        }

        public void SetInterval(float time)
        {
            int count = (int) (targetTime / time);
            intervalTime = new float[count];
            for (int i = 0; i < count; i++)
            {
                intervalTime[i] = time * (i + 1);
            }
        }
        
        public void SetActive(bool active)
        {
            if (active)
            {
                if (!isActive)
                    clockList.Add(this);
            }
            else
            {
                if (isActive)
                    clockList.Remove(this);
            }
        }
        
        public void StartTick()
        {
            SetActive(true);
            if (onStart != null) onStart();
        }
        
        public void Restart()
        {
            Reset();
            SetActive(true);
            onRestart?.Invoke();
        }
        
        public void Pause()
        {
            SetActive(false);
            onPause?.Invoke();
        }

        public virtual void Stop()
        {
            SetActive(false);
            Reset();

            onStop?.Invoke();
            if (autoKill)
            {
                onStart = null;
                onRestart = null;
                onPause = null;
                onComplete = null;
                onStop = null;
                onUpdate = null;
                onInterval = null;
                onSecond = null;
                onMinute = null;
                onHour = null;
            }
        }

        public void Complete()
        {
            SetActive(false);
            onComplete?.Invoke();
        }

        private float GetDelatime()
        {
            float delatime = 0;
            if (owner == null)
            {
                if (ignorePause)
                {
                    delatime = (!ignoreTimescale
                        ? TimeHelper.deltaTimeIgnorePause
                        : TimeHelper.unscaledDeltaTimeIgnorePause);
                }
                else
                {
                    delatime = (!ignoreTimescale ? TimeHelper.deltaTime : TimeHelper.unscaledDeltaTime);
                }
            }
            else
            {
                if (ignoreTimescale)
                {
                    delatime = owner.GetDelatime(ignorePause);
                }
                else
                {
                    delatime = owner.GetUnscaleDelatime(ignorePause);
                }
            }

            return delatime;
        }

        public void OnUpdate()
        {
            float delatime = GetDelatime();
            DelatimeUpdate(delatime);
        }

        public void DelatimeUpdate(float delatime)
        {
            var tempCurr = currentTime;

            if (tempCurr < targetTime)
            {
                tempCurr += delatime;
                currentTime = tempCurr;
                if (tempCurr > targetTime)
                {
                    currentTime = targetTime;
                    tempCurr = targetTime;
                }

                #region interval 秒 分 时

                if (!intervalTime.IsNullOrEmpty() && currentIntervalIndex < intervalTime.Length)
                {
                    if (tempCurr >= intervalTime[currentIntervalIndex])
                    {
                        onInterval?.Invoke(currentIntervalIndex);
                        currentIntervalIndex++;
                    }
                }

                if (tempCurr >= currentSecond + 1)
                {
                    currentSecond = (int) tempCurr;
                    onSecond?.Invoke(currentSecond);
                }

                if (tempCurr / 60 >= currentMinute + 1)
                {
                    currentMinute = (int) (tempCurr / 60);
                    onMinute?.Invoke(currentMinute);
                }

                if (tempCurr / 3600 >= currentHour + 1)
                {
                    currentHour = (int) (tempCurr / 3600);
                    onHour?.Invoke(currentHour);
                }

                #endregion

                if (tempCurr >= targetTime || (listener != null && listener.Invoke()))
                {
                    Complete();
                }
                else
                {
                    onUpdate?.Invoke(tempCurr);
                }
            }
        }

        public bool MoveNext()
        {
            return !isComplete;
        }

        public void Reset()
        {
            currentTime = 0;
            currentIntervalIndex = 0;
            currentSecond = 0;
            currentMinute = 0;
            currentHour = 0;
        }

        public Clock SetID(object ID, string tag1 = null)
        {
            this.ID = ID;
            return this;
        }

        public bool isComplete
        {
            get
            {
                return currentTime >= targetTime;
            }
        }

        public void SetListener(Func<bool> listen)
        {
            this.listener = listen;
        }

        public override string ToString()
        {
            return timeStamp.ToString();
        }
    }
} 