using Module;

public class Parallel : SimpleBehavior
{
    private int index = 1;

    public Monster monster
    {
        get
        {
            return owner as Monster;
        }
    }
    public override void OnStart(ISimpleBehaviorObject owner, object[] args)
    {
        base.OnStart(owner, args);
        index = 1;
        ParallerTo();
    }

    private void ParallerTo()
    {
        var points = monster.GetPatrolPoint(ref index);
        monster.AddStation(MonsterStation.Paraller);
        monster.MoveTo(MoveStyle.Walk, points, (status, complete) =>
        {
            if (complete)
            {
                monster.WaitWhile(monster.currentLevel.editorData.patrolWaitTime==0? monster.currentLevel.dbData.xunTime:monster.currentLevel.editorData.patrolWaitTime, false,ParallerTo);
            }
        });
    }

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Running;
    }
}