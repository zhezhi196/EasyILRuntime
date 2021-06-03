using System;
using UnityEngine;

namespace Module.Set
{


    public class Quality: ISettingData<QualityType>
    {
        public string key
        {
            get { return "QualitySetting"; }
        }
        int scWidth = Screen.width;
        int scHeight = Screen.height;
        int designWidth = 1920; //这个是设计分辨率
        int designHeight = 1080;
        public QualityType value { get; set; }

        public void Init()
        {
            value = ReadData();
            SetQuality(value);
        }

        private void SetQuality(QualityType quality)
        {
            GameDebug.Log("SetQuality:" + quality);
            QualitySettings.SetQualityLevel((int) quality, true);
            if (quality == QualityType.Low)
            {
                if (scWidth < 1280)
                {
                    Screen.SetResolution(scWidth, scHeight, true);
                }
                else {
                    float f = 1280f / scWidth;
                    Screen.SetResolution(1280, (int)(scHeight*f), true);
                }
                Setting.graphicSetting.fps.WriteData(30);
            }
            else
            {
                Screen.SetResolution(scWidth, scHeight, true);
                Setting.graphicSetting.fps.WriteData(60);
            }
        }

        public QualityType ReadData()
        {
            if (HasSetting())
            {
                return (QualityType) LocalFileMgr.GetInt(key);
            }
            else
            {
                return GraphicSetting.graphicSetting.quality;
            }
        }

        public void WriteData(QualityType value1)
        {
            if (value != value1)
            {
                GameDebug.Log("Setting Quality:"+value1);
                value = value1;
                SetQuality(value1);
                LocalFileMgr.SetInt(key, (int)value1);
            }
        }
        public bool HasSetting()
        {
            return LocalFileMgr.ContainKey(key);
        }
    }
}