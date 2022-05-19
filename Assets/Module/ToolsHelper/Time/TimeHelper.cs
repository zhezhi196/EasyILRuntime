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
        
        public static DateTime now
        {
            get { return DateTime.Now; }
        }
        
        public static DateTime today
        {
            get { return DateTime.Today; }
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
        /// <summary>
        /// 距明天还有多长时间
        /// </summary>
        public static TimeSpan remainTomorrow
        {
            get
            {
                DateTime nextDay = DateTime.Today.AddDays(1);
                return nextDay - now;
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

        private static float lastTimeScale = 1f;

        
        //private static float lastGameSpeed = 0;
        public static void Pause()
        {
            if (!isPause)
            {
                isPause = true;
                lastTimeScale = timeScale;
                timeTween.Pause();
                timeScale = 0;
                AudioPlay.PauseAudio(st => !st.ignorePause);
            }
        }

        public static void Continue()
        {
            if (isPause)
            {
                isPause = false;
                timeScale = lastTimeScale;
                timeTween.Play();
                AudioPlay.ContinueAudio(st => !st.ignorePause);
            }
        }

        public static void ResetTimeScale()
        {
            isPause = false;
            timeScale = 1;
            AudioPlay.ContinueAudio(st => !st.ignorePause);
        }

        public static void ChangeBattleScene()
        {
            if (timeTween != null)
                timeTween.Kill();
            timeScale = 1;
            isPause = false;
        }

        public static async void Frame(int count)
        {
            if (timeScale == 1)
            {
                timeScale = 0;
                int index = 0;
                while (index <= count)
                {
                    await Async.WaitForEndOfFrame();
                    index++;
                }
                timeScale = 1;
            }
        }



        public static bool IsNewDay(string key)
        {
            key = "newDay" + key;
            DateTime res = LocalFileMgr.GetDateTime(key);
            if (now.IsNewDay(res))
            {
                LocalFileMgr.SetDateTime(key, now);
                return true;
            }

            return false;
        }

        public static bool IsNewHour(string key)
        {
            key = "newHour" + key;
            DateTime res = LocalFileMgr.GetDateTime(key);
            if (now.IsNewHour(res))
            {
                LocalFileMgr.SetDateTime(key, now);
                return true;
            }

            return false;
        }

        private static DateTime lastTime;

        public static void Update()
        {
            if (now.IsNewDay(lastTime))
            {
                GameDebug.Log("新的一天");
                onNewDay?.Invoke();
            }

            lastTime = now;
        }



    }
}