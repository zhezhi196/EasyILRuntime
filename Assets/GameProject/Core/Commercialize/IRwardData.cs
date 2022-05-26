using Module;

public interface IRwardData:IapSqlData
{
    int ID { get; set; }
    string title { get; set; }
    string des { get; set; }
    string getDes { get; set; }
    string icon { get; set; }
    int level { get; set; }
    string rewardID { get; set; }
    string rewardCount { get; set; }
    int deadTune { get; set; }
}