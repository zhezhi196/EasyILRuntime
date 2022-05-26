using Module;
using Project.Data;

public class AddMaxStrength : GameService
{
    public AddMaxStrength(ServiceData data, ServiceSaveData saveData) : base(data, saveData)
    {
    }
    
    public override float GetReward(float rewardCount, RewardFlag flag)
    {
        //增加精力上限
        Player.player.AddMaxStrength((int)rewardCount);
        return rewardCount;
    }
}