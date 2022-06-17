using System.Collections.Generic;
using Module;
using UnityEngine;

public class Tumour: AttackMonster
{
    private float[] jump_rollingWeight = new float[2];
    private float[] fog_thornWeight = new float[2];
    private TuCi _tuci;
    private JumpAttack _jump;
    private Rolling _rolling;
    private PoisonousFog _fog;
    private Thorns _thorns;

    public Transform fogFirePoint;
    public GameObject duwu01;
    public GameObject[] rollingEffet;
    public Transform tuciAttackPoint;
    public Transform tuciXuewu;
    public override float rotateToMove => 180;

    public TuCi tuci
    {
        get
        {
            if (_tuci == null) _tuci = (TuCi) (skillCtrl.allSkill.Find(fd => fd is TuCi));
            return _tuci;
        }
    }
    public JumpAttack jump
    {
        get
        {
            if (_jump == null) _jump = (JumpAttack) (skillCtrl.allSkill.Find(fd => fd is JumpAttack));
            return _jump;
        }
    }

    public Rolling rolling
    {
        get
        {
            if (_rolling == null) _rolling = (Rolling) (skillCtrl.allSkill.Find(fd => fd is Rolling));
            return _rolling;
        }
    }
    
    public PoisonousFog fog
    {
        get
        {
            if (_fog == null) _fog = (PoisonousFog) (skillCtrl.allSkill.Find(fd => fd is PoisonousFog));
            return _fog;
        }
    }

    public Thorns thorns
    {
        get
        {
            if (_thorns == null) _thorns = (Thorns) (skillCtrl.allSkill.Find(fd => fd is Thorns));
            return _thorns;
        }
    }

    public override float moveSpeed
    {
        get
        {
            if (skillCtrl != null && skillCtrl.currActive is Rolling rolling) return rolling.chargeSpeed;
            return base.moveSpeed;
        }
    }

    public override void OnTransform(int nextLevel)
    {
        base.OnTransform(nextLevel);
        jump_rollingWeight[0] = jump.dbData.weight;
        jump_rollingWeight[1] = rolling.dbData.weight;

        fog_thornWeight[0] = fog.dbData.weight;
        fog_thornWeight[1] = thorns.dbData.weight;
    }

    protected override void OnMosterEndTimeLine(AttackMonster obj, TimeLineType type)
    {
        base.OnMosterEndTimeLine(obj, type);
        if (type == TimeLineType.GetOut && obj == this)
        {
            Roar(null);
        }
    }

    public override float seePlayerFightTime
    {
        get { return 0; }
    }
    public override bool isSeePlayer => isAlive&& Player.player != null;
    
    public override string GetLayerDefaultAnimation(int layer)
    {
        return "Normal";
    }

    public override IActiveSkill RefreshReadySkill()
    {
        if (fightState == FightState.Fight)
        {
            if (jump.isWanted && rolling.isWanted)
            {
                if (skillCtrl.readyRelease != jump && skillCtrl.readyRelease != rolling)
                {
                    int index = RandomHelper.RandomWeight(jump_rollingWeight);
                    if (index == 0) return jump;
                    if (index == 1) return rolling;
                }
                else
                {
                    return skillCtrl.readyRelease;
                }
            }
            else if (fog.isWanted && thorns.isWanted)
            {
                if (skillCtrl.readyRelease != fog && skillCtrl.readyRelease != thorns)
                {
                    int index = RandomHelper.RandomWeight(fog_thornWeight);
                    if (index == 0) return fog;
                    if (index == 1) return thorns;
                }
                else
                {
                    return skillCtrl.readyRelease;
                }
            }

            return tuci;
        }

        return null;
    }
}