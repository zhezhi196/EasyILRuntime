using MoreMountains.NiceVibrations;

namespace Module.Set
{
    public class Vibrate: ISettingData<bool>
    {
        public string key
        {
            get { return "VibrateSetting"; }
        }

        public bool value { get; set; }
        public void Init()
        {
            value = ReadData();
        }

        public bool ReadData()
        {
            if (HasSetting())
            {
                return LocalFileMgr.GetBool(key);
            }
            else
            {
                return true;
            }
        }

        public void WriteData(bool value1)
        {
            if (this.value != value1)
            {
                GameDebug.LogFormat("Setting Vibrate:{0}" , value1);
                this.value = value1;
                LocalFileMgr.SetBool(key, value1);
            }
        }

        public bool HasSetting()
        {
            return LocalFileMgr.ContainKey(key);
        }
    }
}