using Module;


public interface ILimitReward : IRewardObject
{
    bool isActive { get; }
    float remainTime { get; }
    void TryTrigger();
    void OnUpdate();
    void OnExitBattle();
}