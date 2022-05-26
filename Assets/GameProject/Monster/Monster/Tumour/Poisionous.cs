using System;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;

public class Poisionous : MonoBehaviour,IPoolObject
{
    [ReadOnly]
    public bool isFire;
    [ReadOnly]
    public int hurtCount;
    [ReadOnly]
    public float exitTime;
    [ReadOnly]
    public RemoteAttack skill;
    
    public Bounds bounds;
    public float maxExitTime;
    public float interval;
    public Transform rotationPoint;


    public void Fire(Vector3 dir, RemoteAttack skill)
    {
        transform.forward = dir;
        this.skill = skill;
        this.exitTime = 0;
        hurtCount = 0;
        isFire = true;
    }

    public void FixedUpdate()
    {
        if (isFire)
        {
            exitTime += TimeHelper.fixedDeltaTime;
            if (exitTime <= maxExitTime)
            {
                if (exitTime >= interval * hurtCount)
                {
                    skill.monster.HurtPlayer(skill.GetDamage(), bounds, rotationPoint);
                    hurtCount++;
                }
            }
            else
            {
                ObjectPool.ReturnToPool(this);
            }
        }

    }

    public ObjectPool pool { get; set; }
    public void ReturnToPool()
    {
        isFire = false;
    }

    public void OnGetObjectFromPool()
    {
    }
}