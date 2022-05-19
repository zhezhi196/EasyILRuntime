using System;
using UnityEngine;

namespace Module.Set
{
    public class Resolution: ISettingData<Vector2Int>
    {
        public float ScreenScale;
        public string key
        {
            get { return "ResolutionSetting"; }
        }

        public Vector2Int value { get; set; }
        public void Init()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            value = ReadData();
            AdjustResolution(value.x, value.y, true);
        }

        public Vector2Int ReadData()
        {
            if (HasSetting())
            {
                return LocalFileMgr.GetVector2Int(key, ConstKey.Spite0);
            }
            else
            {
                //return GraphicSetting.graphicSetting.resolution;
            }

            return default;
        }
        
        private (int width, int height) AdjustResolution(int width, int height, bool full)
        {
            var current = Screen.currentResolution;

            float wRate = (float)width / current.width;
            float hRate = (float)height / current.height;

            int nWidth = 0;
            int nHeight = 0;
            if (wRate <= hRate)
            {
                ScreenScale = hRate;
                nHeight = height;
                nWidth = (int)(hRate * current.width);
            }
            else
            {
                ScreenScale = hRate;
                nWidth = width;
                nHeight = (int)(wRate * current.height);
            }
            Debug.LogFormat("Setting scren=[{0},{1}] set=[{2},{3}] rate=[{4},{5}] new=[{6},{7}]", current.width, current.height, width, height, wRate, hRate, nWidth, nHeight);
            Screen.SetResolution(nWidth, nHeight, full);
            return (nWidth, nHeight);
        }

        public void WriteData(Vector2Int value1)
        {
            if (this.value != value1)
            {
                GameDebug.Log("Setting Resolution:" + value1);
                LocalFileMgr.SetVector2Int(key, value1, ConstKey.Spite0);
                AdjustResolution(value.x, value.y, true);
            }
        }
        public bool HasSetting()
        {
            return LocalFileMgr.ContainKey(key);
        }
    }
}