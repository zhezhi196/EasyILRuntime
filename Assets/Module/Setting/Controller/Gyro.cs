using MoreMountains.NiceVibrations;

namespace Module.Set
{
    public class Gyro: ISettingData<float>
    {
        public string key
        {
            get { return "GyroSetting"; }
        }

        public float value { get; set; }

        public bool isOpen;

        public void Init()
        {
            value = ReadData();
        }

        public float ReadData()
        {
            if (HasSetting())
            {
                var str = LocalFileMgr.GetString(key);
                var split = str.Split(ConstKey.Spite0);
                isOpen = split[0] == "1";
                return float.Parse(split[1]);
            }
            else
            {
                isOpen = false;
                return 1f;
            }
        }

        public void WriteData(float value1)
        {
            if (this.value != value1)
            {
                GameDebug.LogFormat("Setting Gyro:{0} {1}" ,isOpen ,value1);
                this.value = value1;
                var t = isOpen ? 1 : 0;
                LocalFileMgr.SetString(key , $"{t}_{value1}");
                // LocalFileMgr.SetFloat(key, value1);
            }
        }

        public void WriteDataForce()
        {
            GameDebug.LogFormat("Setting Gyro:{0} {1}" ,isOpen ,value);
            var t = isOpen ? 1 : 0;
            LocalFileMgr.SetString(key , $"{t}_{value}");
        }

        public bool HasSetting()
        {
            return LocalFileMgr.ContainKey(key);
        }
    }
}