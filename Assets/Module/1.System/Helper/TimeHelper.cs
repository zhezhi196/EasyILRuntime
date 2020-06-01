using DG.Tweening;
using UnityEngine;

namespace Module
{
    public static class TimeHelper
    {
        private static Tweener timeTween;
        private static float scaleOrignal;
        public static Tweener SetTimeScale(float target, float time)
        {
            scaleOrignal = Time.timeScale;
            timeTween = DOTween.To(() => Time.timeScale, (value) =>
            {
                Time.fixedDeltaTime = value * 0.02f;
                Time.timeScale = value;
            }, target, time).SetUpdate(true);
            
            return timeTween;
        }

        public static bool InterruptScale()
        {
            if (timeTween.IsPlaying())
            {
                
                timeTween.PlayBackwards();
                return true;
            }

            return false;
        }
    }
}