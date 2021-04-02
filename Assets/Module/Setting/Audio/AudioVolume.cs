using System;

namespace Module.Set
{
    public class AudioVolume : ISettingData<float>
    {
        public string key
        {
            get { return "AudioVolumeSetting"; }
        }

        public float value { get; set; }
        
        public void Init()
        {
            value = ReadData();
            AudioPlay.SetAudioVolume(value);
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

        public void WriteData(float value)
        {
            if (this.value != value)
            {
                GameDebug.Log("Setting Audio:"+value);
                this.value = value;
                LocalFileMgr.SetFloat(key, value);
                AudioPlay.SetAudioVolume(value);
            }
        }

        public bool HasSetting()
        {
            return LocalFileMgr.ContainKey(key);
        }
    }
}