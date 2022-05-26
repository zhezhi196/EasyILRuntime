using Module;
using UnityEngine;
/// <summary>
/// 电刑椅
/// </summary>
public class ElectrocutionChairProp : OnlyInteractiveOpenUI
{
    public override bool canInteractive => CanInteractive();

    public int belongNodeId = 0;

    protected override void OnInitLogical(RunLogicalName logical, RunLogicalFlag flag, string senderArg, string[] args)
    {
        base.OnInitLogical(logical, flag, senderArg, args);
        belongNodeId = creator.transform.parent.parent.GetComponent<NodeParent>().node.id;
    }

    
    private bool CanInteractive()
    {
        var ns = BattleController.GetNode(1).station;
        if (BattleController.Instance.ctrlProcedure.mission.dbData.ID <= 17001) 
        {
             //未到达指定关卡，不能交互
            if (ns == NodeStation.Complete)
            {
                return true;
            }
            return false;
        }
        else
        {
            if (ns == NodeStation.Complete || ns == NodeStation.Running)
            {
                return true;
            }
            return false;
        }
    }
}