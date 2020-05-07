using System.Collections;
using System.Collections.Generic;
using Module;
using SqlCipher4Unity3D;

namespace Module
{
    public class SqlManager : Manager
    {
        private static Dictionary<string, object> m_tableDic = new Dictionary<string, object>();
        private const string SqlPassword = "";
        private const string Config_data = "Player.db";
        protected override int runOrder { get; } = 5;
        protected override string processDiscription { get; }
        protected override void BeforeInit()
        {
        }

        protected override void Init(RunTimeSequence runtime)
        {
            InitSqlData<PlayerData>(Config_data,SQLiteOpenFlags.ReadWrite| SQLiteOpenFlags.Create);
            runtime.NextAction();
        }
        
        private static void InitSqlData<T>(string dbName, SQLiteOpenFlags flag) where T : SqlData, new()
        {
            SqlService<T> data = new SqlService<T>(dbName, SqlPassword, flag);
            m_tableDic.Add(typeof(T).FullName, data);
        }
        
        public static SqlService<T> GetSqlService<T>() where T : SqlData, new()
        {
            return m_tableDic[typeof(T).FullName] as SqlService<T>;
        }
    }
}
