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
using Project.Data;
using UnityEngine;

public class DataMgr : Singleton<DataMgr>
{
    public static SQLiteConnection config;
    public static SQLiteConnection player;
    
    #region Init
    
    public AsyncLoadProcess Init(AsyncLoadProcess process)
    {
        process.Reset();
        InitData();
        process.SetComplete();
        return process;
    }
    
    public void InitData()
    {
        InitDb();
        InitPlayer();
        Project.Data.InitData.Init(config);
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
    
    private void InitPlayer()
    {
        SqlData.InitSqlData<PlayerSaveData>(ConstKey.Player_data, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, player);
        SqlData.InitSqlData<PlayerSaveDataAdd>(ConstKey.Player_data, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, player);
        SqlData.InitSqlData<PlayerInfoSaveData>(ConstKey.Player_data, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, player);
        SqlData.InitSqlData<IapSaveData>(ConstKey.Player_data, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, player);
        SqlData.InitSqlData<MissionSaveData>(ConstKey.Player_data, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, player);
        SqlData.InitSqlData<ServiceSaveData>(ConstKey.Player_data, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, player);
        SqlData.InitSqlData<GiftSaveData>(ConstKey.Player_data, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, player);
        SqlData.InitSqlData<WeaponSaveData>(ConstKey.Player_data, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, player);
        SqlData.InitSqlData<SkinSaveData>(ConstKey.Player_data, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, player);
        SqlData.InitSqlData<PropSaveData>(ConstKey.Player_data, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, player);
        SqlData.InitSqlData<DailyTaskSaveData>(ConstKey.Player_data, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, player);
        SqlData.InitSqlData<AchievementSaveData>(ConstKey.Player_data, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, player);
        SqlData.InitSqlData<CatSaveData>(ConstKey.Player_data, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, player);
        SqlData.InitSqlData<TimeRewardSaveData>(ConstKey.Player_data, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, player);
        SqlData.InitSqlData<TeachingSaveData>(ConstKey.Player_data, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, player);
        SqlData.InitSqlData<AdsSaveData>(ConstKey.Player_data, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, player);
    }
    

    #endregion
   

    
    public int GetDbType(int id)
    {
        return Tools.SpiteID(id, 5, 2);
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
    
    public static string CommonData(int id)
    {
        return Instance.GetSqlService<TongYong>().WhereID(id).lockID;
    }
    
    public void DeleteData()
    {
        string filepath = string.Format("{0}/{1}", Application.persistentDataPath, ConstKey.Player_data);
        File.Delete(filepath);
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
