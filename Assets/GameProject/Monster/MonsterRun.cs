using Module;
using Module.SkillAction;
using Sirenix.OdinInspector;
using UnityEngine;

public class MonsterRun : MonsterSkill
{
    [SerializeField,LabelText("离目标多远停止奔跑")]
    private float _stopMoveDistance;
    [LabelText("最小判定开始奔跑距离")]
    public float minDistance;
    [LabelText("最大判定开始奔跑距离")]
    public float maxDistance;

    public override bool isWanted
    {
        get { return monster.toPlayerDistance >= minDistance && monster.toPlayerDistance <= maxDistance; }
    }

    public override void OnInit(ISkillObject owner)
    {
        base.OnInit(owner);
        PushAction(new PredicateAction(owner, () => monster.toPlayerDistance <= _stopMoveDistance));
        //minDistance = dbData.mosterArg1.ToFloat();
    }
    
    protected override void OnDispose()
    {
        
    }

    protected override void OnActionUpdate(ISkillAction arg1, float percent)
    {
        base.OnActionUpdate(arg1, percent);
        monster.MoveTo(MoveStyle.Run, Player.player.chasePoint, null);
    }

    public override float stopMoveDistance
    {
        get { return _stopMoveDistance; }
    }

}