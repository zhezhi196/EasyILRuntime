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

        public QualityType value { get; set; }

        public void Init()
        {
            value = ReadData();
            SetQuality(value);
        }

        private void SetQuality(QualityType quality)
        {
            QualitySettings.SetQualityLevel((int) quality, true);
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