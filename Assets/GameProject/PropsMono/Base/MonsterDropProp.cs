using System;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 怪物掉落的特殊道具，可以被交互入包，默认隐藏，通过事件移动到怪物脚下
/// </summary>
public class MonsterDropProp : InteractiveToBag
{
    // [LabelText("怪物id")]
    // public int monsterCreatorId;
    [ReadOnly]
    public Vector3 showPos;

    public override string writeData
    {
        get
        {
            return $"{showPos.x},{showPos.y},{showPos.z}";
        }
    }


    public override void Init(PropsInitLogical logical, PropEntity entity, string customData)
    {
        base.Init(logical, entity, customData);
        if (!customData.IsNullOrEmpty())
        {
            var shit = customData.Split(',');
            showPos = new Vector3(shit[0].ToFloat(),shit[1].ToFloat(),shit[2].ToFloat());
        }
    }


    protected override void OnInitLogical(RunLogicalName logical, RunLogicalFlag flag, string senderArg, string[] args)
    {
        base.OnInitLogical(logical, flag, senderArg, args);
        if (logical == RunLogicalName.MonsterDrop)
        {
            //移动到规定位置
            transform.position = showPos;
            
            //显示光柱
            AssetLoad.Instantiate("Effect/Prefabs/WupinLoop.prefab",transform, (v) =>
            {
                v.Result.transform.position = showPos;
            });
        }
    }

    protected override void OnRunPerformance(RunLogicalName logical, RunLogicalFlag flag, IEventCallback sender, string senderArg,
        string[] args)
    {
        base.OnRunPerformance(logical, flag, sender, senderArg, args);
        if (logical == RunLogicalName.MonsterDrop)
        {
            //怪死了设置出现位置
            showPos = ((MonsterCreator) sender).monster.transform.position;
            transform.position = showPos;
            //显示光柱
            AssetLoad.Instantiate("Effect/Prefabs/WupinLoop.prefab",transform, (v) =>
            {
                v.Result.transform.position = showPos;
            });
        }
    }
}