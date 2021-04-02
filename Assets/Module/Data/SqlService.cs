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
using System.Linq;
using System.Reflection;
using SqlCipher4Unity3D;
using UnityEngine;

namespace Module
{
    public interface ISqlService : IDisposable
    {
    }

    public class SqlService<T> : ISqlService where T : ISqlData, new()
    {
        public static int SpiteID(int id, int pos, int offset)
        {
            return (id / Pow(pos - offset)) % Pow(offset);
        }
        
        private static int Pow(int count)
        {
            int temp = 1;
            for (int i = 0; i < count; i++)
            {
                temp = temp * 10;
            }
            
            return temp;
        }
        #region 字段属性

        private List<T> m_tableList = new List<T>();
        private string m_tableName;
        private string m_passWord;
        private string m_DbName;
        private SQLiteOpenFlags m_flag;
        private SQLiteConnection connection;
        public List<T> tableList
        {
            get
            {
                return m_tableList;
            }
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

        public SqlService(string dbName, string password, SQLiteOpenFlags flag, SQLiteConnection connection)
        {
            m_DbName = dbName;
            m_passWord = password;
            m_flag = flag;
            this.connection = connection;
            TableQuery<T> table = connection.Table<T>();

            try
            {
                table.Count();
            }
            catch
            {
                if (flag.HasFlag(SQLiteOpenFlags.Create))
                {
                    connection.CreateTable<T>();
                    var defaultMethod = typeof(T).GetMethod("DefineDefault", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public);
                    if (defaultMethod != null)
                    {
                        T[] t = typeof(T).InvokeMember("DefineDefault", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public, null, null, null) as T[];
                        //T[] t = (T[]) defaultMethod.Invoke(null);
                        if (!t.IsNullOrEmpty()) connection.InsertAll(t, typeof(T));
                    }
                }

            }
            finally
            {
                m_tableName = table.Table.TableName;
                foreach (var value in table)
                {
                    tableList.Add(value);
                }
            }
        }

        #endregion

        #region 增删改查
   
        public void InsertAll(T[] value)
        {
            if (string.IsNullOrEmpty(DbName)) return;
            connection.InsertAll(value);
            for (int i = 0; i < value.Length; i++)
            {
                tableList.Add(value[i]);
            }
        }

        public void Insert(T value)
        {
            if (string.IsNullOrEmpty(DbName)) return;
            connection.Insert(value);
            tableList.Add(value);
        }

        public void Delete(Func<T, bool> predicata)
        {
            if (string.IsNullOrEmpty(DbName)) return;
            T temp = Where(predicata);
            tableList.Remove(temp);
            connection.Delete(temp);
        }

        public void Update(Func<T, bool> predicata, string argName, object value)
        {
            if (string.IsNullOrEmpty(DbName)) return;
            T temp = Where(predicata);
            if (temp != null)
            {
                typeof(T).GetProperty(argName).SetValue(temp, value);
                connection.Update(temp);
            }
            else
            {
                GameDebug.LogError("更新表出错");
            }
        }

        public void Update(ISqlData data)
        {
            using (SQLiteConnection con= DataMgr.Instance.GetSqlConnection(DbName,m_passWord,m_flag))
            {
                con.Update(data);
                con.Close();
            }
        }


        public T WhereID(int dataId)
        {
            if (dataId == 0) return default;
            for (int i = 0; i < tableList.Count; i++)
            {
                var tal = tableList[i];
                if (tal.ID == dataId)
                {
                    return tal;
                }
            }

            return default;
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
            connection.UpdateAll(temp);
        }
        
        /// <summary>
        /// 提供第一个符合条件的table
        /// </summary>
        /// <param iapName="fuc"></param>
        /// <returns></returns>
        public T Where(Func<T, bool> predicate)
        {
            return tableList.FirstOrDefault(predicate);
        }

        /// <summary>
        /// 根据条件寻找
        /// </summary>
        /// <param iapName="Fuc"></param>
        /// <returns></returns>
        public List<T> WhereList(Predicate<T> predicate)
        {

            if (predicate == null)
            {
                return tableList;
            }
            else
            {
                List<T> temp = new List<T>();
                for (int i = 0; i < tableList.Count; i++)
                {
                    if (predicate.Invoke(tableList[i]))
                    {
                        temp.Add(tableList[i]);
                    }
                }
                return temp;
            }
        }


        public List<T> WhereList(Predicate<T> predicate,int count)
        {
            List<T> temp = new List<T>();

            for (int i = 0; i < tableList.Count; i++)
            {
                if (predicate.Invoke(tableList[i])) {
                    temp.Add(tableList[i]);
                    for (int j = 1; j < count; j++)
                    {
                        temp.Add(tableList[i + j]);
                    }
                    return temp;
                }
            }

            return null;
        }

        public void Dispose()
        {
        }
        
        #endregion
    }
}