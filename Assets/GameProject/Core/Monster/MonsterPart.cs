using System;
using System.Collections.Generic;
using Module;
using Project.Data;
using RootMotion.Dynamics;
using UnityEngine;

public enum MonsterPartType
{
    Head,
    Body,
    Arm,
    Leg,
}

public enum MonsterDir
{
    Mid,
    Left,
    Right
}
//20220107潘宇口头阐述 最大硬直伤害在怪物表中只填一个数据,所以部位都读取这个硬直数据,爆头伤害直接武器算好,删除怪物部位表,
public abstract class MonsterPart : MonoBehaviour, IHurtObject,ITarget
{
    private MuscleCollisionBroadcaster _collider;
    private Rigidbody _rig;

    public AttackMonster monster;
    public abstract MonsterPartType partType { get; }
    public MonsterDir partDir;
    public Transform bloodParent;

    public Rigidbody rig
    {
        get
        {
            if(_rig==null) _rig = gameObject.GetComponent<Rigidbody>();
            return _rig;
        }
    }

    private void Awake()
    {
        if (monster == null) monster = transform.GetComponentInParent<AttackMonster>();
    }

    public virtual Hurtmaterial hurtMaterial => Hurtmaterial.Meat;

    public virtual Damage OnHurt(ITarget target, Damage damage)
    {
        if (monster.isAlive)
        {
            if (Mathf.Abs(damage.lagDamage - 0) > 0.001f)
            {
                var paralysisCtrl = monster.GetAgentCtrl<ParalysisCtrl>();
                if (paralysisCtrl != null)
                {
                    paralysisCtrl.AddParalysis(partType, (int) damage.lagDamage, damage);
                    //paralysisCtrl.AddParalysis(partType, 1, damage);
                }
            }


            return monster.OnHurt(this, damage);
        }

        return damage;
    }
    
    public virtual void EditorInit(AttackMonster monster)
    {
        this.monster = monster;
    }

    public bool isVisiable
    {
        get { return true; }
    }

    public Vector3 targetPoint => transform.position;
}