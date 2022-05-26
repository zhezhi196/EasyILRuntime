using System;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;
/// <summary>
/// 油桶（射击会爆炸）
/// </summary>
public class OilDrumProp : OnlyReceiveEventProp , IHurtObject
{
    [LabelText("爆炸范围")]
    public float radius;

    [LabelText("爆炸伤害")]
    public float damage;
    
    public Hurtmaterial hurtMaterial { get; }

    public Damage OnHurt(ITarget target, Damage damage)
    {
        if (damage.damage <= 0 || (damage.weapon & WeaponType.Bow) != 0 || (damage.weapon & WeaponType.Thrown) != 0 || (damage.weapon & WeaponType.Empty) != 0)
        {
            return damage;
        }
        
        
        //TODO 播放特效，范围伤害+硬直
        BOOOOOM();
        RunLogicalOnSelf(RunLogicalName.Destroy);
        return damage;
    }

    private void OnDrawGizmos()
    {
        DrawTools.DrawCircle(transform.position,Quaternion.identity,radius,Color.cyan);
    }

    private void BOOOOOM()
    {
        GameObject o = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        if (o.GetComponent<Collider>() != null)
        {
            o.GetComponent<Collider>().isTrigger = true;
        }
        o.transform.localScale = new Vector3(radius,radius,radius);
        Destroy(o,3f);
        
        var d = new Damage() {damage = this.damage, weapon = WeaponType.OilDrum};
        
        var monsterParent = transform.GetComponentInParent<MonsterParent>();
        var creators = monsterParent.GetComponentsInChildren<MonsterCreator>();
        for (int i = 0; i < creators.Length; i++)
        {
            if (Vector3.Distance(transform.position, creators[i].monster.transform.position) <= radius)
            {
                if (creators[i].monster is Monster monster)
                {
                    monster.OnHurt(d);
                }
            }
        }
    }
}