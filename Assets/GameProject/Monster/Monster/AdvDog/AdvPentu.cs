using Module;
using UnityEngine;
[SkillDescript("高级狗/喷吐")]
public class AdvPentu : RemoteAttack
{
    private string[] _animation = new[] {"Pentu"};
    
    protected override string[] animation
    {
        get { return _animation; }
    }

    public override void OnReleaseGo(AnimationEvent @event, int index)
    {
        AdvDog dog = monster as AdvDog;
        dog.ThrowBlood(this);
    }
}