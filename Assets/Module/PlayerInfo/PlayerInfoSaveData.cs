using SQLite.Attributes;

namespace Module
{
    public class PlayerInfoSaveData: ISqlData
    {
        [PrimaryKey, AutoIncrement] public int ID { get; set; }
        public string creatDataTime { get; set; }
    }
}