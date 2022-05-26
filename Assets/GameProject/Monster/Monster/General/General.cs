using Module;
using Sirenix.OdinInspector;
using UnityEngine;

public class General : AttackMonster
{
    public override float seePlayerFightTime
    {
        get { return 0; }
    }
    public override bool isSeePlayer => isAlive&& Player.player != null;

    public override float moveSpeed
    {
        get
        {
            if (skillCtrl != null && skillCtrl.currActive is Tank tank) return tank.chargeSpeed;
            return base.moveSpeed;
        }
    }

    public Transform handPoint;
    
    [TabGroup("挂点")] 
    public GameObject weapon;
    [TabGroup("挂点")] 
    public GeneralWeapon flyWeapon;

    public Transform attackPoint;

    public override string GetLayerDefaultAnimation(int layer)
    {
        return "Normal";
    }

    public override IActiveSkill RefreshReadySkill()
    {
        return skillCtrl.allSkill[0] as IActiveSkill;
        
    }
}