using DG.Tweening;
using Module;

/// <summary>
/// 需要匹配背包道具的物品，比如门需要钥匙道具，门要继承这个
/// </summary>
public class OpenBagMatch: OnlyInteractive
{
    protected override bool OnInteractive(bool fromMonster = false)
    {
        if (ContainStation(PropsStation.Locked))
        {
            UIController.Instance.Open("BagUI", UITweenType.None, this);
            return false;
        }
        
        return true;
    }
    
    public override void MatchSuccess()
    {
        RunLogicalOnSelf(RunLogicalName.RemoveLock);
        creator.isGet = true;
    }
}