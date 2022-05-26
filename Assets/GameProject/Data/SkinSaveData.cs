using Module;
using SQLite.Attributes;

public class SkinSaveData : ISqlData
{
    [PrimaryKey, AutoIncrement] public int ID { get; set; }
    public string channel { get; set; }
    public int skinID { get; set; }
    public int station { set; get; }
}
