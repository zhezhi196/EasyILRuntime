using System.Collections.Generic;
using Module;
using Module.SkillAction;
using UnityEngine;

public abstract class AttackSkill : PlayerSkillInstance, IActiveSkill
{
    public abstract string attackAnimation { get; }
    public PlayerStation addStation = PlayerStation.Attack;

    
    public float minTriggerDistance;
    public float maxTriggerDistance;
    public float minHurtDistance;
    public float maxHurtDistance;

    public override void OnInit(ISkillObject owner)
    {
        base.OnInit(owner);
        var attackAction = new StationAction(attackAnimation, 0, owner);
        attackAction.onEvent += OnEvent;
        PushAction(attackAction);
        cd = new TimeAction(skillModle.dbData.cd, owner);
    }

    private void OnEvent(string arg1, AnimationEvent arg2, int arg3)
    {
        Player.player.PreAttack(skillModle.dbData.maxHurtCount, GetDamage(arg3));
    }

    public Damage[] GetDamage(int index)
    {
        Damage[] result = new Damage[2];
        string[] spiteDamage = skillModle.dbData.damage.Split(ConstKey.Spite0);
        float plus = spiteDamage[index].ToFloat();
        var d = Damage.CreatHpDamage(Player.player, Player.player.finalAtt, plus);
        result[0] = d;
        string[] lagDamage = skillModle.dbData.lag.Split(ConstKey.Spite0);
        var d2 = Damage.CreatLagDamage(lagDamage[index].ToInt());
        result[1] = d2;
        return result;
    }
    
    public bool isWanted
    {
        get
        {
            return Player.player.toTargetDistance <= maxTriggerDistance && Player.player.toTargetDistance >= minTriggerDistance && base.isWanted;
        }
    }
    
    protected override void OnReleaseStart()
    {
        base.OnReleaseStart();
        Player.player.AddStation(addStation);
    }
    protected override void OnReleaseEnd(bool complete)
    {
        base.OnReleaseEnd(complete);
        Player.player.RemoveStation(addStation);
    }
}