using System.Text;


namespace Module
{
    public class LocalServerClock : Clock
    {
        public bool autoSave = true;

        public string localKey
        {
            get { return "LocalServerClock_" + ID; }
        }

        public string localUid
        {
            get { return ID.ToString(); }
        }

        public LocalServerClock(string uid, float time)
        {
            this.ID = uid;
            string key = localKey;
            if (LocalFileMgr.ContainKey(key))
            {
                string[] temp = LocalFileMgr.GetString(key).Split('_');
                currentTime = temp[0].ToFloat() + (float) ((TimeHelper.now - temp[1].ToDateTime()).TotalSeconds);
            }
            else
            {
                targetTime = time;
            }
            onSecond += LocalSecond;
            onComplete += LocalComplete;
        }

        public LocalServerClock(string uid)
        {
            this.ID = uid;
            if (LocalFileMgr.ContainKey(localKey))
            {
                string[] temp = LocalFileMgr.GetString(localKey).Split('_');
                currentTime = temp[0].ToFloat() + (float) ((TimeHelper.now - temp[1].ToDateTime()).TotalSeconds);
            }
            else
            {
                targetTime = float.MaxValue;
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
            LocalFileMgr.SetString(localKey, currentTime + "_" + TimeHelper.now);
        }
    }
}