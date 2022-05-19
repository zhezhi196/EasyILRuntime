using System;
using System.Collections.Generic;
using System.IO;
using LitJson;
using SqlCipher4Unity3D;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Module
{
    public static class SqlData
    {
        public static string editorDbPath = $"{Application.dataPath}/../{ConstKey.GetFolder(AssetLoad.AssetFolderType.DB)}/";

        public static Dictionary<string, object> tableDic = new Dictionary<string, object>();
        
        #region Init私有方法

        public static SqlService<T> InitSqlData<T>(string dbName, SQLiteOpenFlags flag, SQLiteConnection connection) where T : ISqlData, new()
        {
            SqlService<T> data = new SqlService<T>(dbName, ConstKey.SqlPassword, flag, connection);
            tableDic.Add(typeof(T).FullName, data);
            return data;
        }

        #endregion

        #region 数据的获取

        public static bool TryGetValue<T>(out object result) where T : ISqlData, new()
        {
            return tableDic.TryGetValue(typeof(T).FullName, out result);
        }

        public static SqlService<T> GetSqlService<T>() where T : ISqlData, new()
        {
            object result = tableDic[typeof(T).FullName];
            return (SqlService<T>)result;
        }

        #endregion
        
        #region 获取SqliteConnect对象

        public static SQLiteConnection GetSqlConnection(string dbName, string mPassword, SQLiteOpenFlags flag)
        {
            string filepath = string.Format("{0}/{1}", Application.persistentDataPath, dbName);
#if UNITY_EDITOR
#if LOG_ENABLE
            mPassword = null;
            string dbPath = editorDbPath + dbName;
#else
            string dbPath = $"{Application.streamingAssetsPath}/{ConstKey.GetFolder(AssetLoad.AssetFolderType.DB)}/{ConstKey.Config_data}";
#endif

            //string dbPath = string.Format(@"Assets/StreamingAssets/{0}{1}", ConstKey.GetFolder(AssetLoad.AssetFolderType.DB),dbName);
            if (!flag.HasFlag(SQLiteOpenFlags.ReadWrite))
            {
                File.Copy(dbPath, filepath,true);
            }
#else

#if UNITY_ANDROID
            string dbPath = "jar:file://" + Application.dataPath + "!/assets/" + ConstKey.GetFolder(AssetLoad.AssetFolderType.DB) + dbName;
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
                    WWW perDB = new WWW(dbPath);
                    while (!perDB.isDone)
                    {
                    }

                    File.WriteAllBytes(filepath, loadDb.bytes);
                }
            }

#elif UNITY_IOS
            string dbPath = Application.dataPath + "/Raw/" + ConstKey.GetFolder(AssetLoad.AssetFolderType.DB) + dbName; // this is the path to your StreamingAssets in iOS
            if (!File.Exists(filepath))
            {
                File.Copy(dbPath, filepath);
            }
            else
            {
                if (!flag.HasFlag(SQLiteOpenFlags.ReadWrite))
                {
                    File.Copy(dbPath, filepath,true);
                }
            }
#else
            string dbPath = Application.dataPath + "/StreamingAssets/" + ConstKey.GetFolder(AssetLoad.AssetFolderType.DB) + dbName; // this is the path to your StreamingAssets in iOS
            if (!File.Exists(filepath))
            {
                File.Copy(dbPath, filepath);
            }
            else
            {
                if (!flag.HasFlag(SQLiteOpenFlags.ReadWrite))
                {
                    File.Copy(dbPath, filepath,true);
                }
            }

#endif

#endif
            SQLiteConnection _connection = new SQLiteConnection(filepath, flag, mPassword);
            return _connection;
        }

        #endregion

        /// <summary>
        /// 删除持久化数据库文件
        /// </summary>
        public static void DeletePersistentDBFile()
        {
            string filepath = string.Format("{0}/{1}", Application.persistentDataPath, ConstKey.Player_data);
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
                GameDebug.Log("删除数据库db成功！");
            }
        }
    }
}