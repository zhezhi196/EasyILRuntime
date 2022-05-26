using System;
using Module;
using UnityEngine;
using UnityEngine.AI;

public class Dog: AttackMonster
{
    public Transform damagePoint;

    public override float moveSpeed
    {
        get
        {
            if (skillCtrl != null && skillCtrl.currActive is PuYao pu)
            {
                return base.moveSpeed * pu.puSpeedK;
            }
            else
            {
                return base.moveSpeed;
            }
        }
    }

    public override float rotateSpeed
    {
        get
        {
            if (moveSpeed == 0) return 0;
            if (fightState == FightState.Normal) return currentLevel.attribute.rotateSpeed * 0.3f;
            return currentLevel.attribute.rotateSpeed;
        }
    }

    public override float rotateToMove => 30;

    public override string GetLayerDefaultAnimation(int layer)
    {
        if (layer == 0)
        {
            return "Normal";
        }
        else
        {
            return null;
        }
    }
    
    // public override float GetLayerFadeTime(int type, string name)
    // {
    //     if (name == "AssDie" || name == "ExcDog")
    //     {
    //         return 0;
    //     }
    //     else
    //     {
    //         return base.GetLayerFadeTime(type, name);
    //     }
    // }
    
    public override float GetTranslateTime(string name)
    {
        if (name == "AssDie" || name == "ExcDog")
        {
            return 0;
        }
        else
        {
            return base.GetTranslateTime(name);
        }
    }

    protected override void Update()
    {
        base.Update();

    }

    public override bool MoveTo(MoveStyle type, Vector3 point, Action<NavMeshPathStatus, bool> callback)
    {
        if (fightState > FightState.Normal && type >= MoveStyle.Walk)
        {
            type = MoveStyle.Run;
        }
        return base.MoveTo(type, point, callback);
    }
}