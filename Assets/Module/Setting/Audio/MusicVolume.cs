using System;

namespace Module.Set
{
    public class MusicVolume: ISettingData<float>
    {
        public string key
        {
            get { return "MusicVolumeSetting"; }
        }
        
        public float value { get; set; }

        public void Init()
        {
            value = ReadData();
            AudioPlay.SetMusicVolume(value);
        }

        public float ReadData()
        {
            if (HasSetting())
            {
                return LocalFileMgr.GetFloat(key);
            }
            else
            {
                return 1;
            }
        }

        public void WriteData(float value1)
        {
            if (this.value != value1)
            {
                GameDebug.LogFormat("Setting MusicVolume:{0}",value1);
                value = value1;
                LocalFileMgr.SetFloat(key, value1);
                AudioPlay.SetMusicVolume(value);
            }
        }
        public bool HasSetting()
        {
            return LocalFileMgr.ContainKey(key);
        }
    }
}