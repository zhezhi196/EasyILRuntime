using System;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using UnityEngine;
namespace Module
{
    public static class TimeHelper
    {
        private static Tweener timeTween;

        public static bool isPause;

        public static event Action onNewDay;
        //private float tempScale;
        public static float gameSpeed { get; set; } = 1;
        public static float timeScale
        {
            get
            {
                return Time.timeScale * gameSpeed;
            }
            set
            {
                fixedDeltaTime = value * 0.02f;
                float result = value * gameSpeed;
                Time.timeScale = result;
            }
        }

        public static float deltaTime
        {
            get
            {
                if (!isPause)
                {
                    return Time.deltaTime * gameSpeed;
                }
                else
                {
                    return 0;
                }
            }
        }

        public static float deltaTimeIgnorePause
        {
            get
            {
                return Time.deltaTime * gameSpeed;
            }
        }

        public static float unscaledDeltaTime
        {
            get
            {
                if (!isPause)
                {
                    return Time.unscaledDeltaTime * gameSpeed;
                }
                else
                {
                    return 0;
                }
            }
        }

        public static float unscaledDeltaTimeIgnorePause
        {
            get
            {
                return Time.unscaledDeltaTime * gameSpeed;
            }
        }

        public static float fixedDeltaTime
        {
            get
            {
                if (!isPause)
                {
                    return Time.fixedDeltaTime * gameSpeed;
                }
                else
                {
                    return 0;
                }
            }
            set { Time.fixedDeltaTime = value / gameSpeed; }
        }

        public static float fixedUnscaledDeltaTime
        {
            get
            {
                if (!isPause)
                {
                    return Time.fixedUnscaledDeltaTime * gameSpeed;
                }
                else
                {
                    return 0;
                }
            }
        }
        
        public static Tweener SetTimeScale(float target, float time)
        {
            if (timeTween != null && timeTween.IsActive() && timeTween.IsPlaying()) timeTween.Kill();
            timeTween = DOTween.To(() => timeScale, (value) =>
            {
                timeScale = value;
            }, target, time).SetUpdate(true);

            return timeTween;
        }

        public static float GetTime(float time)
        {
            return time / gameSpeed;
        }

        private static float lastTimeScale = Time.timeScale;

        private static List<object> pauseFlag = new List<object>();
        
        //private static float lastGameSpeed = 0;
        public static void Pause(object key)
        {
            if (!pauseFlag.Contains(key))
            {
                pauseFlag.Add(key);
            }

            if (!isPause)
            {
                isPause = true;
                lastTimeScale = timeScale;
                timeTween.Pause();
                timeScale = 0;
                AudioPlay.PauseAudio(st => !st.ignorePause);
                GameDebug.LogFormat("TimeHelper Pause: ", string.Join(",", pauseFlag));
            }
        }

        public static void Continue(object key)
        {
            if (pauseFlag.Contains(key))
            {
                pauseFlag.Remove(key);
            }

            if (pauseFlag.Count == 0 && isPause)
            {
                isPause = false;
                timeScale = lastTimeScale;
                timeTween.Play();
                AudioPlay.ContinueAudio(st => !st.ignorePause);
                GameDebug.LogFormat("TimeHelper Continue: " , string.Join(",", pauseFlag));
            }
        }

        public static void Exit()
        {
            if (timeTween != null)
                timeTween.Kill();
            timeScale = 1;
        }

        public static DateTime GetNow()
        {
            return DateTime.Now;
        }

        public static bool IsNewDay(string key)
        {
            DateTime res = LocalFileMgr.GetDateTime(key);
            if (GetNow().IsNewDay(res))
            {
                LocalFileMgr.SetDateTime(key, GetNow());
                return true;
            }

            return false;
        }

        private static DateTime lastTime;

        public static void Update()
        {
            if (GetNow().IsNewDay(lastTime))
            {
                GameDebug.Log("新的一天");
                onNewDay?.Invoke();
            }

            lastTime = GetNow();
        }
    
    }
}