/*
 * 脚本名称：DataMgr
 * 项目名称：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2017-12-21 22:43:21
 * 脚本作用：
*/

using System.IO;
using SqlCipher4Unity3D;
using Module;
using UnityEngine;

public class DataInit : Singleton<DataInit>
{
    public int GetDbType(int id)
    {
        return Tools.SpiteID(id, 5, 2);
    }
    
    public static SQLiteConnection config;
    public static SQLiteConnection player;

    public AsyncLoadProcess Init(AsyncLoadProcess process)
    {
        process.Reset();
        InitData();
        process.SetComplete();
        return process;
    }

    public void InitDb()
    {
        if (config == null)
            config = SqlData.GetSqlConnection(ConstKey.Config_data, ConstKey.SqlPassword,
                SQLiteOpenFlags.ReadOnly);
        if (player == null)
            player = SqlData.GetSqlConnection(ConstKey.Player_data, ConstKey.SqlPassword,
                SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite);
    }

    public void InitData()
    {
        InitDb();
        SqlData.InitSqlData<PlayerInfoSaveData>(ConstKey.Player_data, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, player);
        SqlData.InitSqlData<MissionSaveData>(ConstKey.Player_data, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, player);
        SqlData.InitSqlData<PlayerSaveData>(ConstKey.Player_data, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, player);
        SqlData.InitSqlData<SkillSaveData>(ConstKey.Player_data, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, player);
        SqlData.InitSqlData<SkillSlotSaveData>(ConstKey.Player_data, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, player);
        SqlData.InitSqlData<AideSkillSaveData>(ConstKey.Player_data, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, player);
        Project.Data.InitData.Init(config);
    }

    public void DeleteData()
    {
        string filepath = string.Format("{0}/{1}", Application.persistentDataPath, ConstKey.Player_data);
        File.Delete(filepath);
    }

    public SqlService<T> GetSqlService<T>() where T : ISqlData, new()
    {
        object result = null;
        if (config == null || player == null)
        {
            InitData();
            if (!Application.isPlaying)
            {
                config.Close();
                config.Dispose();
            }
        }
        if (!SqlData.TryGetValue<T>(out result))
        {
            result = SqlData.InitSqlData<T>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);

        }

        return (SqlService<T>)result;
    }

    public void Dispose()
    {
        if (config != null)
        {
            config.Close();
            config.Dispose();
        }

        if (player != null)
        {
            player.Close();
            player.Dispose();
        }
    }
}
