namespace Module
{
    public class LocalClock: Clock, ILocalSave
    {
        public bool autoSave = true;
        private string subPath = "LocalSave";

        public string localUid
        {
            get { return ID.ToString(); }
        }

        public void SetSavePath(string path)
        {
            this.subPath = path;
        }

        public LocalClock(string uid, float time)
        {
            this.ID = uid;
            targetTime = time;
            LocalSave.Read(this,subPath);
        }

        public LocalClock(string uid)
        {
            this.ID = uid;
            targetTime = float.MaxValue;
            LocalSave.Read(this,subPath);
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
            LocalSave.Delete(this,subPath);
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
            LocalSave.Write(this, false,subPath);
        }

        public override void Stop()
        {
            base.Stop();
            LocalSave.Delete(this,subPath);
        }

        public string GetWriteDate()
        {
            return currentTime.ToString();
        }
    }
}