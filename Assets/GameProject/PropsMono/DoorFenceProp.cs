using System.Collections.Generic;
using Module;
using UnityEngine;

/// <summary>
/// 栅栏门
/// </summary>
public class DoorFenceProp : DoorUnlockTranslationProp
{
    public List<GameObject> ammoIgnoreObj;
    
    protected override void OnInitLogical(RunLogicalName logical, RunLogicalFlag flag, string senderArg, string[] args)
    {
        base.OnInitLogical(logical, flag, senderArg, args);

        if (!ContainStation(PropsStation.Destroyed))
        {
            for (int i = 0; i < ammoIgnoreObj.Count; i++)
            {
                ammoIgnoreObj[i].layer = 21;
            }
        }

    }
}