using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public static class FPS
    {
        private static float time;
        private static int frameCount;
        public static float fps;
        public static int maxFps = 60;
        
        public static void SetFps()
        {
            Application.targetFrameRate = maxFps;
        }

        public static void Update()
        {
            time += Time.unscaledDeltaTime;
            frameCount++;
            if (time >= 1 && frameCount >= 1)
            {
                fps = frameCount / time;
                time = 0;
                frameCount = 0;
            }
        }
    }
}