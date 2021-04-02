using System;
using DG.Tweening;
using UnityEngine;
namespace Module
{
    public static class TimeHelper
    {
        private static Tweener timeTween;

        public static bool isPause;

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
        //private static float lastGameSpeed = 0;
        public static void Pause()
        {
            isPause = true;
            lastTimeScale = timeScale;
            //lastGameSpeed = gameSpeed;
            timeTween.Pause();
            timeScale = 0;
            //gameSpeed = 0;
        }

        public static void Continue()
        {
            //gameSpeed = lastGameSpeed;
            isPause = false;
            timeScale = lastTimeScale;
            timeTween.Play();
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
    }
}