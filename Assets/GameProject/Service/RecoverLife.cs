using Module;
using Project.Data;

public class RecoverLife : GameService
{
    public RecoverLife(ServiceData data, ServiceSaveData saveData) : base(data, saveData)
    {
    }

    public override float GetReward(float rewardCount, RewardFlag flag)
    {
        Player.player.ChangeHp(rewardCount);
        return rewardCount;
    }
}