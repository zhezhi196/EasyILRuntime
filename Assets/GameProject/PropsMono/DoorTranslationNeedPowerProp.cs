using System.Collections.Generic;
using Module;
using UnityEngine;

/// <summary>
/// 需要电箱解锁的门(平移)
/// </summary>
public class DoorTranslationNeedPowerProp : DoorTranslation
{
    
    public List<GameObject> ammoIgnoreObj;
    
    protected override void OnInitLogical(RunLogicalName logical, RunLogicalFlag flag, string senderArg, string[] args)
    {
        base.OnInitLogical(logical, flag, senderArg, args);

        if (!ContainStation(PropsStation.Destroyed))
        {
            for (int i = 0; i < ammoIgnoreObj.Count; i++)
            {
                ammoIgnoreObj[i].layer = 2;
            }
        }
    }
    
    public override bool canInteractive => ContainStation(PropsStation.Locked);

    
    protected override bool OnInteractive(bool fromMonster = false)
    {
        return false;
    }
}