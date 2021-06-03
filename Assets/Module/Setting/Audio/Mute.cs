using System;

namespace Module.Set
{
    public class Mute: ISettingData<bool>
    {
        public string key
        {
            get { return "MuteSetting"; }
        }

        public bool value { get; set; }

        public void Init()
        {
            value = ReadData();
            AudioPlay.SetMute(value);
        }

        public bool ReadData()
        {
            if (HasSetting())
            {
                return LocalFileMgr.GetBool(key);
            }
            else
            {
                return false;
            }
        }

        public void WriteData(bool value1)
        {
            if (this.value != value1)
            {
                GameDebug.LogFormat("Setting Mute:{0}", value1);
                this.value = value1;
                LocalFileMgr.SetBool(key, value1);
                AudioPlay.SetMute(value);
            }
        }
        public bool HasSetting()
        {
            return LocalFileMgr.ContainKey(key);
        }
    }
}