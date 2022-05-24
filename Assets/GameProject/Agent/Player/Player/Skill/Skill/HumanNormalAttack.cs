using Module;
using Module.SkillAction;
using PLAYERSKILL;
using UnityEngine;

public class HumanNormalAttack : NormalAttack
{
    public override AgentType agentType => AgentType.Human;
    public StationAction action3;
    public bool isHit;
    public float framePercent = 0.5f;
    public int frame = 3;
    public override void OnInit(ISkillObject owner)
    {
        var attack1 = new StationAction("Attack1", 0, owner);
        attack1.onEvent += OnEvent;
        PushAction(attack1);
        var attack2 = new StationAction("Attack1", 0, owner);
        attack2.onEvent += OnEvent;
        PushAction(attack2);
        var attack3 = new StationAction("Attack1", 0, owner);
        attack3.onEvent += OnEvent;
        action3 = new StationAction("Attack2", 0, owner);
        PushAction(action3);
    }

    private void OnEvent(string name,AnimationEvent arg1, int arg2)
    {
        Damage damage = new Damage() {damage = 1,position = Player.player.currLevel.hurtPoint.position};
        Player.player.HurtMonster((Monster)Player.player.target, damage);
    }

    protected override void OnActionUpdate(ISkillAction arg1, float percent)
    {
        base.OnActionUpdate(arg1, percent);
        if (arg1 == action3)
        {
            if (!isHit && percent >= framePercent)
            {
                isHit = true;
                TimeHelper.Frame(frame);
                
            }
        }
        else
        {
            isHit = false;
        }
    }
}