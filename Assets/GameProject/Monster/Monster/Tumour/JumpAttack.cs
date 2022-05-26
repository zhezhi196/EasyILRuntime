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

    public override void JumpComplete()
    {
        base.JumpComplete();
        EffectPlay.Play("rouliu_tiaoza", monster.transform);
    }
}