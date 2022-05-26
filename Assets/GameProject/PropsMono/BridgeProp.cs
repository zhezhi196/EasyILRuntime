using Module;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 桥，没啥用
/// </summary>
public class BridgeProp : OnlyReceiveEventProp 
{
    public NavMeshSurface surface;
    protected override void OnInitLogical(RunLogicalName logical, RunLogicalFlag flag, string senderArg, string[] args)
    {
        base.OnInitLogical(logical, flag, senderArg, args);
        
        // surface.BuildNavMesh();
    }
}