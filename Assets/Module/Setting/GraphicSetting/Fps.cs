using System;
using UnityEngine;

namespace Module.Set
{
    public class Fps : ISettingData<int>
    {
        private long mFrameCount = 0;
        private long mLastFrameTime = 0;
        static long mLastFps = 0;
        public string key { get; }
        public int value { get; set; }

        public void Init()
        {
            value = ReadData();
            Application.targetFrameRate = value;
        }

        public int ReadData()
        {
            return GraphicSetting.graphicSetting.fps;
        }

        public void WriteData(int value1)
        {
            GameDebug.LogFormat("Setting Fps:{0}" , value1);
            value = value1;
            Application.targetFrameRate = value;
        }

        public void Update()
        {
            mFrameCount++;
            long nCurTime = TickToMilliSec(System.DateTime.Now.Ticks);
            if (mLastFrameTime == 0)
            {
                mLastFrameTime = TickToMilliSec(System.DateTime.Now.Ticks);
            }

            if ((nCurTime - mLastFrameTime) >= 1000)
            {
                long fps = (long) (mFrameCount * 1.0f / ((nCurTime - mLastFrameTime) / 1000.0f));

                mLastFps = fps;

                mFrameCount = 0;

                mLastFrameTime = nCurTime;
                value = (int)fps;
            }
        }
        public long TickToMilliSec(long tick)
        {
            return tick / (10 * 1000);
        }

        public bool HasSetting()
        {
            return false;
        }
    }
}