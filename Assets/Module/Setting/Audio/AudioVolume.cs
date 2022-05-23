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
            AudioManager.SetAudioVolume(value);
            // AudioPlay.SetAudioVolume(value);
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
                GameDebug.LogFormat("Setting Audio:{0}",value);
                this.value = value;
                LocalFileMgr.SetFloat(key, value);
                AudioManager.SetAudioVolume(value);
                // AudioPlay.SetAudioVolume(value);
            }
        }

        public bool HasSetting()
        {
            return LocalFileMgr.ContainKey(key);
        }
    }
}