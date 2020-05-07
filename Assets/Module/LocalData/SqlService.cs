/*
 * 脚本名称：EnergyValue
 * 项目名称：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2018-05-17 10:18:35
 * 脚本作用：
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Module;
using SqlCipher4Unity3D;
using UnityEngine;

namespace Module
{
    public interface ISqlService : IDisposable
    {
    }

    public class SqlService<T> : ISqlService where T : SqlData, new()
    {
        #region 字段属性

        private List<T> m_tableList = new List<T>();
        private string m_tableName;
        private string m_passWord;
        private string m_DbName;
        private SQLiteOpenFlags m_flag;

        public List<T> tableList
        {
            get { return m_tableList; }
        }

        public string DbName
        {
            get { return m_DbName; }
        }

        public string tableName
        {
            get { return m_tableName; }
        }

        public SQLiteOpenFlags openFlag
        {
            get { return m_flag; }
        }

        #endregion

        #region 构造函数 

        public SqlService(string dbName, string password, SQLiteOpenFlags flag)
        {
            m_DbName = dbName;
            m_passWord = password;
            m_flag = flag;
            using (SQLiteConnection conn = GetSqlConnection(dbName, password, flag))
            {
                TableQuery<T> table = conn.Table<T>();

                try
                {
                    table.Count();
                }
                catch
                {
                    conn.CreateTable<T>();
                    try
                    {
                        T[] t = typeof(T).InvokeMember("DefineDefault", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public, null, null, null) as T[];
                        if (!t.IsNullOrEmpty()) conn.InsertAll(t, typeof(T));
                    }
                    catch
                    {
                    }
                }
                finally
                {
                    m_tableName = table.Table.TableName;

                    foreach (var value in table)
                    {
                        tableList.Add(value);
                    }

                    //conn.Close();
                    conn.Dispose();
                }
            }
        }

        #endregion

        #region 获取SqliteConnect对象

        public SQLiteConnection GetSqlConnection(string dbName, string mPassword, SQLiteOpenFlags flag)
        {
#if UNITY_EDITOR
            string dbPath = string.Format(@"Assets/StreamingAssets/{0}", dbName);
#else
// check if file exists in Application.persistentDataPath
            string filepath = string.Format("{0}/{1}", Application.persistentDataPath, dbName);

            if (!File.Exists(filepath))
            {
#if UNITY_ANDROID
                WWW loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + dbName); // this is the path to your StreamingAssets in android
                while (!loadDb.isDone)
                {
                } // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check

                // then save to Application.persistentDataPath
                File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                string loadDb = Application.dataPath + "/Raw/" + dbName; // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#else
                string loadDb =
 Application.dataPath + "/StreamingAssets/" + dbName; // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);

#endif
            }

            string dbPath = filepath;
#endif
            SQLiteConnection _connection = new SQLiteConnection(dbPath,m_passWord, flag);
            return _connection;
        }

        #endregion

        #region 增删改查

        public void InsertAll(T[] value)
        {
            if (string.IsNullOrEmpty(DbName)) return;
            for (int i = 0; i < value.Length; i++)
            {
                tableList.Add(value[i]);
            }

            using (SQLiteConnection conn = GetSqlConnection(DbName, m_passWord, m_flag))
            {
                conn.InsertAll(value);
                conn.Close();
            }
        }

        /// <summary>
        /// 增
        /// </summary>
        /// <typeparam iapName="U"></typeparam>
        /// <param iapName="tableName"></param>
        /// <param iapName="value"></param>
        public void Insert(T value)
        {
            if (string.IsNullOrEmpty(DbName)) return;
            tableList.Add(value);
            using (SQLiteConnection conn = GetSqlConnection(DbName, m_passWord, m_flag))
            {
                conn.Insert(value);
                conn.Close();
            }
        }

        /// <summary>
        /// 删
        /// </summary>
        /// <param iapName="id"></param>
        public void Delete(Predicate<T> predicata)
        {
            if (string.IsNullOrEmpty(DbName)) return;
            T temp = Where(predicata);
            tableList.Remove(temp);
            using (SQLiteConnection conn = GetSqlConnection(DbName, m_passWord, m_flag))
            {
                conn.Delete(temp);
                conn.Close();
            }
        }

        /// <summary>
        /// 改
        /// </summary>
        /// <param iapName="id"></param>
        /// <param iapName="argName"></param>
        /// <param iapName="value"></param>
        public void Update(Predicate<T> predicata, string argName, object value)
        {
            if (string.IsNullOrEmpty(DbName)) return;
            T temp = Where(predicata);
            typeof(T).GetProperty(argName).SetValue(temp, value);
            using (SQLiteConnection conn = GetSqlConnection(DbName, m_passWord, m_flag))
            {
                conn.Update(temp);
                conn.Close();
            }
        }

        public void UpdateAll(Predicate<T> predicate, string[] argName, object[] value)
        {
            if (string.IsNullOrEmpty(DbName)) return;
            List<T> temp = WhereList(predicate);

            for (int i = 0; i < temp.Count; i++)
            {
                for (int j = 0; j < argName.Length; j++)
                {
                    typeof(T).GetProperty(argName[j]).SetValue(temp[i], value[j]);
                }
            }

            using (SQLiteConnection conn = GetSqlConnection(DbName, m_passWord, m_flag))
            {
                conn.UpdateAll(temp);
                conn.Close();
            }
        }

        /// <summary>
        /// 提供第一个符合条件的table
        /// </summary>
        /// <param iapName="fuc"></param>
        /// <returns></returns>
        public T Where(Predicate<T> predicate)
        {
            List<T> temp = WhereList(predicate);
            if (temp != null && temp.Count > 0)
            {
                return temp[0];
            }

            return null;
        }

        /// <summary>
        /// 根据条件寻找
        /// </summary>
        /// <param iapName="Fuc"></param>
        /// <returns></returns>
        public List<T> WhereList(Predicate<T> predicate)
        {
            List<T> temp = new List<T>();
            if (predicate == null)
            {
                return tableList;
            }
            else
            {
                for (int i = 0; i < tableList.Count; i++)
                {
                    if (predicate.Invoke(tableList[i]))
                    {
                        temp.Add(tableList[i]);
                    }
                }
            }

            return temp;
        }

        public void Dispose()
        {
        }

        #endregion
    }
}