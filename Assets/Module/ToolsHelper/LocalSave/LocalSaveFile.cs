using System;
using System.Collections.Generic;
using System.IO;
using LitJson;
using UnityEngine;

namespace Module
{
    public struct LocalSaveInfo
    {
        public string group;
        public string key;
        public string value;
    }
    
    public class LocalSaveFile
    {
        private static Dictionary<string, LocalSaveFile> allFile = new Dictionary<string, LocalSaveFile>();

        public static void InitLocalFile(string name)
        {
            allFile.Add(name, new LocalSaveFile().InitFile(name));
        }

        public static string GetString(string path, string key)
        {
            LocalSaveFile saveFile = null;
            if (allFile.TryGetValue(path, out saveFile))
            {
                if (!saveFile.info.IsNullOrEmpty())
                {
                    for (int i = 0; i < saveFile.info.Count; i++)
                    {
                        if (saveFile.info[i].key == key)
                        {
                            return saveFile.info[i].value;
                        }
                    }
                }
            }

            return null;
        }

        public static string[] GetGroup(string path, string group)
        {
            LocalSaveFile saveFile = null;
            if (allFile.TryGetValue(path, out saveFile))
            {
                if (!saveFile.info.IsNullOrEmpty())
                {
                    List<string> resu = new List<string>();
                
                    for (int i = 0; i < saveFile.info.Count; i++)
                    {
                        if (saveFile.info[i].@group == group)
                        {
                            resu.Add(saveFile.info[i].value);
                        }
                    }

                    return resu.ToArray();
                }
            }

            return null;
        }

        public static void SetString(string path, LocalSaveInfo value)
        {
            LocalSaveFile saveFile = null;
            if (allFile.TryGetValue(path, out saveFile))
            {
                if(saveFile.info==null) saveFile.info = new List<LocalSaveInfo>();
                for (int i = 0; i < saveFile.info.Count; i++)
                {
                    if (saveFile.info[i].key == value.key)
                    {
                        saveFile.info[i] = new LocalSaveInfo()
                            {@group = value.@group, key = value.key, value = value.value};
                        saveFile.isChanged = true;
                        return;
                    }
                }

                saveFile.info.Add(new LocalSaveInfo() {@group = value.@group, key = value.key, value = value.value});
                saveFile.isChanged = true;
            }
        }

        public static void DeleteFile(string path)
        {
            LocalSaveFile saveFile = null;
            if (allFile.TryGetValue(path, out saveFile))
            {
                if (saveFile != null)
                {
                    File.Delete(saveFile.filePath);
                    saveFile.info?.Clear();
                }
            }
        }

        /// <summary>
        /// GM调用：删除所有的存档
        /// </summary>
        public static void GmDeleteAllFile(bool deleteDB)
        {
            var pathSave = $"{Application.persistentDataPath}/SaveData";
            if (File.Exists(pathSave))
            {
                File.Delete(pathSave);
                GameDebug.Log("删除所有本地存档文件成功!");
            }

            if (deleteDB)
            {
                //删除数据库
                pathSave = $"{Application.persistentDataPath}/{ConstKey.Player_data}";
                if (File.Exists(pathSave))
                {
                    File.Delete(pathSave);
                    GameDebug.Log("删除数据库成功!");
                }
            }
        }

        public static void DeleteKey(string path, string key)
        {
            LocalSaveFile saveFile = null;
            if (allFile.TryGetValue(path, out saveFile))
            {
                if (!saveFile.info.IsNullOrEmpty())
                {
                    for (int i = 0; i < saveFile.info.Count; i++)
                    {
                        if (saveFile.info[i].key == key)
                        {
                            saveFile.info.RemoveAt(i);
                            saveFile.isChanged = true;
                            break;
                        }
                    }
                }
            }
        }

        public static void DeleteGroup(string path, string group)
        {
            LocalSaveFile saveFile = null;
            if (allFile.TryGetValue(path, out saveFile))
            {
                if (!saveFile.info.IsNullOrEmpty())
                {
                    for (int i = saveFile.info.Count - 1; i >= 0; i--)
                    {
                        if (saveFile.info[i].@group == group)
                        {
                            saveFile.info.RemoveAt(i);
                            saveFile.isChanged = true;
                        }
                    }
                }

            }
        }

        public static void Update()
        {
            foreach (KeyValuePair<string,LocalSaveFile> localSaveFile in allFile)
            {
                localSaveFile.Value.UpdateFile();
            }
        }

        #region File

        public List<LocalSaveInfo> info;
        
        private bool isChanged;
        private string filePath;
        
        private LocalSaveFile InitFile(string subPath)
        {
            filePath = Pathelper.PersistentDataPath() + "/" + subPath;
            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string text = reader.ReadToEnd();
#if LOG_ENABLE
                    string aesDecrypt = text;
#else
                    string aesDecrypt = EncryptionHelper.AesDecrypt(text, EncryptionHelper.MD5Encrypt(PlayerInfo.pid + "LocalSave"));
#endif
                    info = JsonMapper.ToObject<List<LocalSaveInfo>>(aesDecrypt);
                }
            }

            return this;
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
            string json = JsonMapper.ToJson(info);
#if LOG_ENABLE
            string data = json;
#else
            string data = EncryptionHelper.AesEncrypt(json, EncryptionHelper.MD5Encrypt(PlayerInfo.pid + "LocalSave"));

#endif
            if (!File.Exists(filePath))
            {
                using (var temp = File.CreateText(filePath))
                {
                    temp.Write(data);
                }
            }
            else
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.Write(data);
                }
            }

            isChanged = false;
        }

        #endregion
    }
}