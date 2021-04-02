using System.Text;
using Sirenix.Utilities;

namespace Module
{
    public class LocalServerClock : Clock, ILocalSave
    {
        public bool autoSave = true;
        private string subPath = "LocalServerClock";

        public string localUid
        {
            get { return ID.ToString(); }
        }

        public LocalServerClock(string uid, float time)
        {
            this.ID = uid;
            targetTime = time;
            LocalSave.Read(this, subPath);
        }

        public LocalServerClock(string uid)
        {
            this.ID = uid;
            targetTime = float.MaxValue;
            LocalSave.Read(this, subPath);
        }

        public void SetSavePath(string path)
        {
            this.subPath = path;
        }

        public void ReadData(string data)
        {
            if (data != null)
            {
                string[] temp = data.Split('_');
                currentTime = temp[0].ToFloat() + (float) ((TimeHelper.GetNow() - temp[1].ToDateTime()).TotalSeconds);
            }

            onSecond += LocalSecond;
            onComplete += LocalComplete;
        }

        private void LocalComplete()
        {
            LocalSave.Delete(this, subPath);
        }

        public override void Stop()
        {
            base.Stop();
            LocalSave.Delete(this, subPath);
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
            LocalSave.Write(this, false, subPath);
        }

        public string GetWriteDate()
        {
            return currentTime + "_" + TimeHelper.GetNow();
        }
    }
}