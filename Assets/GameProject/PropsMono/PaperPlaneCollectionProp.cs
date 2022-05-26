using System.Collections;
using System.Collections.Generic;
using Module;
using UnityEngine;

/// <summary>
/// 纸飞机（收集）
/// </summary>
public class PaperPlaneCollectionProp : InteractiveToBag
{
    public override InterActiveStyle interactiveStyle => InterActiveStyle.Handle;
    protected override bool OnInteractive(bool fromMonster = false)
    {
        // base.OnInteractive(fromMonster);
        RunLogicalOnSelf(RunLogicalName.Destroy);
        entity.GetReward(count, creator.id, creator.matchInfo, 0);
        //增加属性，读表
        int num = int.Parse(entity.GetPropArg(1));
        int id = int.Parse(entity.GetPropArg(2));
        var reward = Commercialize.GetRewardContent(DataMgr.CommonData(id).ToInt(), num);
        reward.GetReward(1, 0); //这里调用GameService增加玩家属性
        
        RewardUI.OpenRewardUI(reward);
        return true;
    }
}
