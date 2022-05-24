using System;
using Module;
using SQLite.Attributes;

public class MissionSaveData : ISqlData
{
    [PrimaryKey, AutoIncrement] public int ID { get; set; }
    public int missionID { get; set; }
    public int station { get; set; }
    public float crossTime { get; set; }
}

public class PlayerSaveData : ISqlData
{
    [PrimaryKey, AutoIncrement] public int ID { get; set; }
    public int soul { get; set; }
    public int crystal { get; set; }

}
public class SkillSaveData : ISqlData
{
    [PrimaryKey, AutoIncrement] public int ID { get; set; }
    public string skillName { get; set; }
    public int backFragment { get; set; }
    public int frontFragment { get; set; }
    public int level { get; set; }
    public int exp { get; set; }
}

public class SkillSlotSaveData : ISqlData
{
    [PrimaryKey, AutoIncrement] public int ID { get; set; }
    public int saveSlotId { get; set; }
}

public class AideSkillSaveData : ISqlData
{
    [PrimaryKey, AutoIncrement] public int ID { get; set; }
    public string skillName { get; set; }
}
