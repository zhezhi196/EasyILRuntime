using System;
using Module;
using UnityEngine;

public class GetDangObject: MonoBehaviour,IHurtObject
{
    public BoxCollider collidre;
    public Monster monster;

    public void Update()
    {
        if (monster.skillCtrl.currActive is Blocking blocking)
        {
            collidre.enabled = true;
        }
        else
        {
            collidre.enabled = false;
        }
    }

    public Hurtmaterial hurtMaterial
    {
        get
        {
            return Hurtmaterial.Mental;
        }
    }

    public Damage OnHurt(ITarget target, Damage damage)
    {
        return damage;
    }
}