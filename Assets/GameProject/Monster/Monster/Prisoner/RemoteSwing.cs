using Module;
using UnityEngine;
[SkillDescript("远程囚犯/挥击")]
public class RemoteSwing : MeleeAttack
{
    protected override string animation
    {
        get
        {
            return "Wprisoner@attack";
        }
    }

    protected override void OnReleaseStart()
    {
        base.OnReleaseStart();
        RemotePrisoner prison = (RemotePrisoner)monster;
        prison.GetAxi();
    }

    public override bool isWanted
    {
        get { return monster.toPlayerDistance <= maxDistance && monster.toPlayerDistance >= minDistance; }
    }

    public override Transform damagePoint
    {
        get
        {
            return ((Prisoner) monster).damagePoint;
        }
    }
}