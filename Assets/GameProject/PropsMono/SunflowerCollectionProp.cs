using System.Collections;
using System.Collections.Generic;
using Module;
using UnityEngine;

/// <summary>
/// 向日葵（收集）
/// </summary>
public class SunflowerCollectionProp : InteractiveToBag
{
    public override InterActiveStyle interactiveStyle => InterActiveStyle.Handle;

    protected override bool OnInteractive(bool fromMonster = false)
    {
        // base.OnInteractive(fromMonster);
        RunLogicalOnSelf(RunLogicalName.Destroy);
        entity.GetReward(count, creator.id, creator.matchInfo, 0);
        //增加属性
        int num = int.Parse(entity.GetPropArg(1));
        int id = int.Parse(entity.GetPropArg(2));
        var reward = Commercialize.GetRewardContent(DataMgr.CommonData(id).ToInt(), num);
        reward.GetReward(1, 0);
        RewardUI.OpenRewardUI(reward);
        
        return true;
    }
}
