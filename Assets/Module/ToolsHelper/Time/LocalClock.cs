namespace Module
{
    public class LocalClock: Clock
    {
        public bool autoSave = true;

        public string localUid
        {
            get { return ID.ToString(); }
        }
        
        public string localKey
        {
            get { return "LocalClock" + ID; }
        }

        public LocalClock(string uid, float time)
        {
            this.ID = uid;
            string key = localKey;
            if (LocalFileMgr.ContainKey(key))
            {
                targetTime = LocalFileMgr.GetFloat(key);
            }
            else
            {
                targetTime = time;
            }
        }

        public LocalClock(string uid)
        {
            this.ID = uid;
            string key = localKey;
            if (LocalFileMgr.ContainKey(key))
            {
                targetTime = LocalFileMgr.GetFloat(key);
            }
            else
            {
                targetTime = float.MaxValue;
            }
        }
        
        public void ReadData(string data)
        {
            if (data != null)
            {
                currentTime = data.ToFloat();
            }

            onSecond += LocalSecond;
            onComplete += LocalComplete;
        }

        private void LocalComplete()
        {
            LocalFileMgr.RemoveKey(localKey);
        }

        private void LocalSecond(int obj)
        {
            if (autoSave && currentTime <= targetTime)
            {
                Save();
            }
        }

        public void Save()
        {
            LocalFileMgr.SetFloat(localKey, currentTime);
        }
    }
}