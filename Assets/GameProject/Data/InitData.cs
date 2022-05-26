using Module;
using SqlCipher4Unity3D;

namespace Project.Data
{
    public static class InitData
    {
        public static void Init(SQLiteConnection config)
        {
            SqlData.InitSqlData<AudioData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<BuffData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<DropData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<GiftData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<IapData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<MissionData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<MonsterData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<PlayerData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<PropData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<ServiceData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<SkinData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<WeaponData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<WeaponPartData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<MonsterSkillData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<DailyTaskData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<AchievementData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<BulletData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<AdsData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<JessicaData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<TongYong>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
        }
    }
}
