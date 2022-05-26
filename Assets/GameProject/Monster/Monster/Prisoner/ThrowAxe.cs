using Module;
using Module.SkillAction;
using UnityEngine;

[SkillDescript("远程囚犯/飞斧")]
public class ThrowAxe : RemoteAttack
{
    protected override string[] animation { get; } = {"Wprisoner@HaveWeapon", "Wprisoner@NotWeapon"};

    public override void OnReleaseGo(AnimationEvent @event, int index)
    {
        RemotePrisoner prison = (RemotePrisoner) monster;
        if (@event.animatorClipInfo.clip.name == "Wprisoner@HaveWeapon")
        {
            prison.ThrowAxi(this);
        }
        else if (@event.animatorClipInfo.clip.name == "Wprisoner@NotWeapon")
        {
            prison.GetAxi();
        }
    }
}