namespace Module.Set
{
    public class Sensitivity: ISettingData<float>
    {
        public virtual string key
        {
            get { return "SensitivitySetting"; }
        }

        public float value { get; set; }
        public void Init()
        {
            value = ReadData();
        }

        public float ReadData()
        {
            if (HasSetting())
            {
                return LocalFileMgr.GetFloat(key);
            }
            else
            {
                return 1f;
            }
        }

        public void WriteData(float value1)
        {
            if (this.value != value1)
            {
                GameDebug.LogFormat("Setting Sensitivity:{0}" , value1);
                this.value = value1;
                LocalFileMgr.SetFloat(key, value1);
            }
        }

        public bool HasSetting()
        {
            return LocalFileMgr.ContainKey(key);
        }
    }
}