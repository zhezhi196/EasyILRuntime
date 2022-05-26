using Module;
using SQLite.Attributes;

public class WeaponSaveData : ISqlData
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public int weaponID { get; set; }
    public int equipSkin { get; set; }
    public int level { get; set; }
}
