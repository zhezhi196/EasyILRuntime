using Module;

public interface IGameRewardData: ISqlData
{
    string title { get; set; }
    string Icon { get; set; }
    string des { get; set; }
    string rewardId { get; set; }
    string rewardCount { get; set; }
    int partsCost { get; set; }
    int alloyCost { get; set; }
    int memoryCost { get; set; }
    float off { get; set; }
}