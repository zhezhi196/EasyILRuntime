using System;
using UnityEngine;

namespace Module.Set
{
    public class Fps : ISettingData<int>
    {
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
        }

        public bool HasSetting()
        {
            return false;
        }
    }
}