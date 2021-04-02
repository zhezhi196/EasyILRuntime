using System;
using System.Collections.Generic;
using System.Text;
using Sirenix.Utilities;

namespace Module
{
    public static class LocalSave
    {
        public const string localTime = "saveTime";

        private static string GetKey(string key)
        {
            return key;
        }

        public static bool hasSave
        {
            get { return LocalSaveFile.ContainKey(localTime); }
        }

        public static DateTime lastSaveTime
        {
            get
            {
                if (hasSave) return LocalSaveFile.GetDateTime(localTime);
                return default;
            }
        }

        public static string Read(string save, string path = "SaveData")
        {
            string loc = GetKey(save);
            string data = LocalSaveFile.GetString(loc);
            return data;
        }

        public static bool Read(ILocalSave save, string path = "SaveData")
        {
            string loc = GetKey(save.localUid);
            string data = LocalSaveFile.GetString(loc);
            save.ReadData(data);
            return data.IsNullOrEmpty();
        }

        public static void Write(ILocalSave save, bool timeStamp = true, string path = "SaveData")
        {
            if (timeStamp)
            {
                LocalSaveFile.SetDateTime(localTime, TimeHelper.GetNow());
            }

            LocalSaveFile.SetString(GetKey(save.localUid), save.GetWriteDate());
        }

        public static void Delete(ILocalSave save, string path = "SaveData")
        {
            LocalSaveFile.RemoveKey(GetKey(save.localUid));
        }

        public static void DeleteAll(string path = "SaveData")
        {
            LocalSaveFile.DeleteAll();
        }
    }
}