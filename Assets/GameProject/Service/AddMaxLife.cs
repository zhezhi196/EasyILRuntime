using Module;
using Project.Data;

public class AddMaxLife : GameService
{
    public AddMaxLife(ServiceData data, ServiceSaveData saveData) : base(data, saveData)
    {
    }
    
    public override float GetReward(float rewardCount, RewardFlag flag)
    {
        //增加血量上限
        Player.player.AddMaxHp((int)rewardCount);
        return rewardCount;
    }
}