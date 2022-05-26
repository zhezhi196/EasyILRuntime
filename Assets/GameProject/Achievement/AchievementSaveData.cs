using Module;
using SQLite.Attributes;

public class AchievementSaveData : ISqlData
{
    [PrimaryKey, AutoIncrement]  public int ID { get; set; }
    public string type { get; set; }
    public int currentCount { get; set; }
    public int currId { get; set; }
    public int complete { get; set; }
    
}