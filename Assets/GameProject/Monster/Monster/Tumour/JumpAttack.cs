using Module;
using Module.SkillAction;
using Sirenix.OdinInspector;
using UnityEngine;

[SkillDescript("肉瘤/跳砸")]
public class JumpAttack : JumpSkill
{
    [LabelText("跳跃距离")]
    public float jumpDistance;
    public override Vector3 jumpTarget
    {
        get
        {
            return (monster.transform.position - Player.player.transform.position).normalized * jumpDistance + Player.player.transform.position;
        }
    }

    public override Vector3 damagePoint
    {
        get
        {
            return ((Tumour) monster).centerPoint.position;
        }
    }

    protected override async void OnReleaseEnd(bool complete)
    {
        if (monster.navmesh)
        {
            monster.navmesh.enabled = true;
        }

        await Async.WaitforSeconds(0.5f);
        monster.Roar(null);
        EffectPlay.Play("rouliu_tiaoza", monster.transform.position, monster.transform.eulerAngles, damageRadius);
        monster.HurtPlayer(GetDamage(), damageRadius, damagePoint);
        monster.RemoveStation(this.addStation);
    }
}