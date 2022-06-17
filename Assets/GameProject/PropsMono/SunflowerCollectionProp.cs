using System.Collections;
using System.Collections.Generic;
using LitJson;
using Module;
using UnityEngine;

/// <summary>
/// 向日葵（收集）
/// </summary>
public class SunflowerCollectionProp : InteractiveToBag
{
    public override InterActiveStyle interactiveStyle => InterActiveStyle.Handle;

    protected override void OnInitLogical(RunLogicalName logical, RunLogicalFlag flag, string senderArg, string[] args)
    {
        base.OnInitLogical(logical, flag, senderArg, args);
        var saveStr = LocalSave.Read(this.creator);
        if (saveStr.IsNullOrEmpty())
        {
            return;
        }
        
        var propSave = JsonMapper.ToObject<PropSave>(saveStr);
        if (propSave.lastRunlogicalName == RunLogicalName.Destroy.ToString())
        {
            //说明已经有存档了，不能在拾取了，直接删除自己
            DestroyThis(true);
        }
    }
    
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
