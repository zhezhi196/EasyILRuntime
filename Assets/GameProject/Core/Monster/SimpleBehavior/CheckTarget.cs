using Module;
using UnityEngine;

public class CheckTarget : SimpleBehavior
{
    private TaskStatus status;
    private Vector3 target;

    public AttackMonster monster
    {
        get { return (AttackMonster) owner; }
    }

    public override void OnStart(ISimpleBehaviorObject owner, object[] args)
    {
        base.OnStart(owner, args);
        status = TaskStatus.Running;
        target = (Vector3) args[0];
        //禅道 原地播查看动画
        monster.Check(() =>
        {
            
            this.status = TaskStatus.Success;
        });
    }

    public override TaskStatus OnUpdate()
    {
        return status;
    }
}