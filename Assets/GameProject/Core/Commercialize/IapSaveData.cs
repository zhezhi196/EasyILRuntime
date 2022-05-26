using Module;
using SQLite.Attributes;

public class IapSaveData : ISqlData
{
    [PrimaryKey, AutoIncrement] public int ID { get; set; }
    public int iapId { get; set; }
    public string orderId { get; set; }
    public int station { get; set; }
    public string plantOrder { get; set; }
    public string sku { get; set; }
}

public class AdsSaveData : ISqlData
{
    [PrimaryKey, AutoIncrement] public int ID { get; set; }
    public int Count { get; set; }
}