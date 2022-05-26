using System;
using Module;
using SQLite.Attributes;

public class CatSaveData : ISqlData
{
    [PrimaryKey, AutoIncrement]  public int ID { get; set; }
    public int editorID { get; set; }
    public int missionId { get; set; }
}


public class PlayerSaveData : ISqlData
{
    // public int totalCash { get; set; }
    // public int totalGemstone { get; set; }
    // public string cash { get; set; }
    // public string gemstone { get; set; }
    // public int consumCash { get; set; }
    // public int consumGemstone { get; set; }
    public int totalParts { get; set; }
    public int totalAlloy { get; set; }
    public int totalMemory { get; set; }
    
    public string parts { get; set; }
    public string alloy { get; set; }
    public string memory { get; set; }
    
    public int consumParts { get; set; }
    public int consumAlloy { get; set; }
    public int consumMemory { get; set; }
    
    public string playerName { get; set; }
    public long expire { get; set; }
    public string c { get; set; }
    public string w { get; set; }
    public string token { get; set; }
    public string playerId { get; set; }
    public string userType { get; set; }
    [PrimaryKey, AutoIncrement] public int ID { get; set; }
}

public class PlayerSaveDataAdd : ISqlData
{
    public string arg1 { get; set; }//生命值上限增加占用
    public string arg2 { get; set; }//体力上限增加占用
    public string arg3 { get; set; }
    public string arg4 { get; set; }
    public string arg5 { get; set; }
    public string arg7 { get; set; }
    public string arg8 { get; set; }
    public string arg9 { get; set; }
    [PrimaryKey, AutoIncrement] public int ID { get; set; }
}

public class MissionSaveData : ISqlData
{
    [PrimaryKey, AutoIncrement] public int ID { get; set; }
    public int missionID { get; set; }
    public int station { get; set; }
    public float crossTime { get; set; }
}

public class ServiceSaveData : ISqlData
{
    [PrimaryKey, AutoIncrement] public int ID { get; set; }
    public int targetId { get; set; }
    public string deadLine { get; set; }
}

public class GiftSaveData : ISqlData
{
    [PrimaryKey, AutoIncrement] public int ID { get; set; }
    public int targetId { get; set; }
    public int station { get; set; }
}

public class PropSaveData : ISqlData
{
    [PrimaryKey, AutoIncrement] public int ID { get; set; }
    public int targetID { get; set; }
    public int station { get; set; }
}

public class TeachingSaveData : ISqlData
{
    [PrimaryKey, AutoIncrement] public int ID { get; set; }
    public string teachingName { get; set; }
    public int station { get; set; }
}
