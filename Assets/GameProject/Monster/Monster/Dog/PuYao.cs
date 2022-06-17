using Module;
using Module.SkillAction;
using Sirenix.OdinInspector;
using UnityEngine;
[SkillDescript("普通狗/扑咬")]
public class PuYao : MonsterSkill
{
    [LabelText("想要释放技能的最大距离")] 
    public float wantDistance = 10;

    [LabelText("开始冲刺距离")]
    public float beginPuDistance = 5;
    [LabelText("冲刺速度加成")]
    public float puSpeedK = 5;
    [LabelText("停止冲刺开始攻击距离")]
    public float maxPuDistance = 2;
    public PredicateAction runAction;
    public string animation = "Puyao";
    public Bounds damageBounds;
    protected bool isHurtPlayer;
    public Vector3 puPoint;
    public override float stopMoveDistance
    {
        get { return maxPuDistance; }
    }

    public override bool isWanted => monster.toPlayerDistance <= wantDistance && monster.toPlayerDistance >= maxPuDistance;

    public Transform damagePoint
    {
        get { return ((Dog) monster).damagePoint; }
    }

    public override void OnInit(ISkillObject owner)
    {
        base.OnInit(owner);
        runAction = new PredicateAction(owner, () => puPoint.Distance(monster.transform.position) <= maxPuDistance);
        PushAction(runAction);
        PushAction(new AnimationAction(animation, 0, owner));
        monster.onAnimationCallback += OnAnimationCallback;
    }
    
    protected override void OnDispose()
    {
        monster.onAnimationCallback -= OnAnimationCallback;
    }

    protected override void OnActionEnter(ISkillAction skillAction)
    {
        if (skillAction != runAction)
        {
            monster.AddStation(addStation);
        }        
    }

    protected override void OnReleaseStart()
    {
        puPoint = Player.player.transform.position;
        LookAtPlayer();
        isHurtPlayer = false;
    }
    public virtual void LookAtPlayer()
    {
        monster.LookAtPoint(Player.player.chasePoint);
    }
    
    protected override void OnReleaseEnd(bool complete)
    {
        monster.RemoveStation(addStation);
        if (isHurtPlayer && complete)
        {
            if (monster.afterStyle == AfterAttackStyle.Duizhi)
            {
                monster.GetAgentCtrl<SimpleBehaviorCtrl>().SwitchBehavior(new Confrontation());
            }
            else if (monster.afterStyle == AfterAttackStyle.Roar)
            {
                monster.Roar(null);
            }
        }
    }

    protected override void OnActionUpdate(ISkillAction arg1, float percent)
    {
        if (arg1 == runAction)
        {
            monster.MoveTo(MoveStyle.Run, puPoint, null);
        }
    }

    protected virtual void OnAnimationCallback(AnimationEvent @event, int index)
    {
        if (@event.animatorClipInfo.clip.name == animation && monster.skillCtrl.currActive == this)
        {
            PlayerDamage damage = GetDamage(index);

            if (monster.HurtPlayer(damage, damageBounds, damagePoint))
            {
                isHurtPlayer = true;
            }
        }
    }
    
    public override void UpdateTry()
    {
        if (monster.target != null)
        {
            monster.MoveTo(MoveStyle.Run, Player.player.chasePoint, ((status, b) =>
            {
                if (b && monster.ContainStation(MonsterStation.IsSeePlayHide))
                {
                    GameDebug.LogError("出来吧小子");
                }
            } ));
        }
    }
}