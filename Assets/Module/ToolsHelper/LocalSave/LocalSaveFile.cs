using System;
using System.Collections.Generic;
using System.IO;
using LitJson;

namespace Module
{
    public class LocalSaveFile
    {
        private static List<(string, LocalSaveFile)> saveDic = new List<(string, LocalSaveFile)>();

        public static AsyncLoadProcess Init(AsyncLoadProcess process)
        {
            saveDic.Add(("SaveData", new LocalSaveFile().InitFile("SaveData")));
            saveDic.Add(("LocalServerClock", new LocalSaveFile().InitFile("LocalServerClock")));
            saveDic.Add(("LocalSave", new LocalSaveFile().InitFile("LocalSave")));
            return process;
        }

        public static string GetString(string key, string path = "SaveData")
        {
            for (int i = 0; i < saveDic.Count; i++)
            {
                if (saveDic[i].Item1 == path)
                {
                    return saveDic[i].Item2.GetFileString(key);
                }
            }

            return null;
        }

        public static void SetString(string key, string value, string path = "SaveData")
        {
            for (int i = 0; i < saveDic.Count; i++)
            {
                if (saveDic[i].Item1 == path)
                {
                    saveDic[i].Item2.SetFileString(key, value);
                }
            }
        }

        public static void Update()
        {
            for (int i = 0; i < saveDic.Count; i++)
            {
                saveDic[i].Item2.UpdateFile();
            }
        }

        public static void RemoveKey(string getKey, string path = "SaveData")
        {
            for (int i = 0; i < saveDic.Count; i++)
            {
                if (saveDic[i].Item1 == path)
                {
                    saveDic[i].Item2.RemoveFileKey(getKey);
                }
            }
        }

        public static void SetDateTime(string key, DateTime value, string path = "SaveData")
        {
            for (int i = 0; i < saveDic.Count; i++)
            {
                if (saveDic[i].Item1 == path)
                {
                    saveDic[i].Item2.SetFileDateTime(key, value);
                }
            }
        }

        public static DateTime GetDateTime(string key, string path = "SaveData")
        {
            for (int i = 0; i < saveDic.Count; i++)
            {
                if (saveDic[i].Item1 == path)
                {
                    return saveDic[i].Item2.GetFileDateTime(key);
                }
            }

            return default;
        }

        public static void DeleteAll(string path = "SaveData")
        {
            for (int i = 0; i < saveDic.Count; i++)
            {
                if (saveDic[i].Item1 == path)
                {
                    saveDic[i].Item2.DeleteFileAll();
                }
            }

        }

        public static bool ContainKey(string key,string path = "SaveData")
        {
            for (int i = 0; i < saveDic.Count; i++)
            {
                if (saveDic[i].Item1 == path)
                {
                    return saveDic[i].Item2.ContainFileKey(key);
                }
            }

            return false;
        }


        #region File

        private Dictionary<string, string> saveFileDic = new Dictionary<string, string>();
        private bool isChanged;
        private string filePath;
        private string subPath;
        private LocalSaveFile InitFile(string subPath)
        {
            filePath = Pathelper.PersistentDataPath() + "/" + subPath;
            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string text = reader.ReadToEnd();
                    saveFileDic = JsonMapper.ToObject<Dictionary<string, string>>(text);
                }
            }

            return this;
        }

        private string GetFileString(string key)
        {
            string result = null;
            if (saveFileDic.TryGetValue(key, out result))
            {
                return result;
            }

            return null;
        }

        private void SetFileString(string key, string value)
        {
            string result = null;
            if (saveFileDic.TryGetValue(key, out result))
            {
                saveFileDic[key] = value;
            }
            else
            {
                saveFileDic.Add(key, value);
            }

            isChanged = true;
        }

        private void UpdateFile()
        {
            if (isChanged)
            {
                WriteFileJson();
            }
        }

        private void WriteFileJson()
        {
            if (!File.Exists(filePath))
            {
                using (var temp = File.CreateText(filePath))
                {
                    temp.Write(JsonMapper.ToJson(saveFileDic));
                }
            }
            else
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.Write(JsonMapper.ToJson(saveFileDic));
                }
            }

            isChanged = false;
        }

        private void RemoveFileKey(string getKey)
        {
            if (saveFileDic.ContainsKey(getKey))
            {
                saveFileDic.Remove(getKey);
                isChanged = true;
            }
        }

        private void SetFileDateTime(string key, DateTime value)
        {
            SetFileString(key, value.ToString());
        }

        private DateTime GetFileDateTime(string key)
        {
            return GetFileString(key).ToDateTime();
        }

        private void DeleteFileAll()
        {
            saveFileDic.Clear();
            File.Delete(filePath);
        }

        private bool ContainFileKey(string key)
        {
            return saveFileDic.ContainsKey(key);
        }

        #endregion
    }
}