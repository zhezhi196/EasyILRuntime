using Module;
using SqlCipher4Unity3D;

namespace Project.Data
{
    public static class InitData
    {
        public static void Init(SQLiteConnection config)
        {
            SqlData.InitSqlData<AttributeData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<AudioData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<BuffData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<DropData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<IapData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<AdsData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<MissionData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<MonsterData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<PlayerData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<PropData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<ServiceData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<MonsterSkillData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<PlayerSkillData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<SkillSlotData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<AideSkillData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
            SqlData.InitSqlData<WeaponData>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);
        }
    }
}
