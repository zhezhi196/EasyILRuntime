using System;
using System.Collections.Generic;
using System.IO;
using LitJson;
using SqlCipher4Unity3D;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Module
{
    public class DataMgr : Singleton<DataMgr>
    {
        public const string subFoder = "Chapter2/";
        public static Dictionary<string, object> tableDic = new Dictionary<string, object>();

        private static Dictionary<string, string> configToken = new Dictionary<string, string>();

        #region Init私有方法
        
        public SqlService<T> InitSqlData<T>(string dbName, SQLiteOpenFlags flag, SQLiteConnection connection) where T : ISqlData, new()
        {
            SqlService<T> data = new SqlService<T>(dbName, ConstKey.SqlPassword, flag, connection);
            tableDic.Add(typeof(T).FullName, data);
            return data;
        }

        public void InitJsonData<T>(string dataName,Action callback)
        {
            AssetLoad.PreloadAsset<TextAsset>(string.Format(ConstKey.JsonConfigPath, dataName), txt =>
            {
                if (txt.Result != null)
                {
                    string json = txt.Result.text;
                    tableDic.Add(typeof(T).FullName, JsonMapper.ToObject<T[]>(json));
                    callback?.Invoke();
                    AssetLoad.Release(txt);
                }
            });
        }

        #endregion

        #region 数据的获取

        public bool TryGetValue<T>(out object result) where T : ISqlData, new()
        {
            return tableDic.TryGetValue(typeof(T).FullName, out result);
        }

        /// <summary>
        /// 获取本地数据组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T[] GetDataArray<T>(bool autoKill)
        {
            object result = null;
            if (tableDic.TryGetValue(typeof(T).FullName, out result))
            {
                if (autoKill)
                {
                    tableDic.Remove(typeof(T).FullName);
                }
                
                return result as T[];
            }

            return null;
        }

        public void ClearData<T>()
        {
            string key = typeof(T).FullName;
            if (!tableDic.ContainsKey(key)) return;

            tableDic.Remove(key);
        }
        
        public SqlService<T> GetSqlService<T>() where T : ISqlData, new()
        {
            object result = tableDic[typeof(T).FullName];
            return (SqlService<T>)result;
        }

        #endregion
        
        #region 获取SqliteConnect对象

        public SQLiteConnection GetSqlConnection(string dbName, string mPassword, SQLiteOpenFlags flag)
        {
            string filepath = string.Format("{0}/{1}", Application.persistentDataPath, dbName);
#if UNITY_EDITOR

            string dbPath = string.Format(@"Assets/StreamingAssets/{0}{1}", subFoder,dbName);
            if (!flag.HasFlag(SQLiteOpenFlags.ReadWrite))
            {
                File.Copy(dbPath, filepath,true);
            }
#else

#if UNITY_ANDROID
            string dbPath = "jar:file://" + Application.dataPath + "!/assets/" + subFoder + dbName;
            WWW loadDb = new WWW(dbPath);
            while (!loadDb.isDone)
            {
            }

            if (!File.Exists(filepath))
            {
                File.WriteAllBytes(filepath, loadDb.bytes);
            }
            else
            {
                if (!flag.HasFlag(SQLiteOpenFlags.ReadWrite))
                {
                    string streamHash = Tools.GetFileMD5(loadDb.bytes);
                    WWW perDB = new WWW(dbPath);
                    while (!perDB.isDone)
                    {
                    }

                    string perHash = Tools.GetFileMD5(perDB.bytes);
                    if (streamHash != perHash)
                    {
                        File.WriteAllBytes(filepath, loadDb.bytes);
                    }
                }
            }

#elif UNITY_IOS
            string dbPath = Application.dataPath + "/Raw/" + subFoder + dbName; // this is the path to your StreamingAssets in iOS
            if (!File.Exists(filepath))
            {
                File.Copy(dbPath, filepath);
            }
            else
            {
                if (!flag.HasFlag(SQLiteOpenFlags.ReadWrite))
                {
                    string streamHash = Tools.GetFileMD5(dbPath);
                    string perHash = Tools.GetFileMD5(filepath);
                    if (streamHash != perHash)
                    {
                        File.Copy(dbPath, filepath,true);
                    }
                }
            }
#else
            string dbPath = Application.dataPath + "/StreamingAssets/" + subFoder + dbName; // this is the path to your StreamingAssets in iOS
            if (!File.Exists(filepath))
            {
                File.Copy(dbPath, filepath);
            }
            else
            {
                if (!flag.HasFlag(SQLiteOpenFlags.ReadWrite))
                {
                    string streamHash = Tools.GetFileMD5(dbPath);
                    string perHash = Tools.GetFileMD5(filepath);
                    if (streamHash != perHash)
                    {
                        File.Copy(dbPath, filepath,true);
                    }
                }
            }

#endif

#endif
            
            SQLiteConnection _connection = new SQLiteConnection(filepath, mPassword, flag);
            return _connection;
        }

        #endregion
    }
}