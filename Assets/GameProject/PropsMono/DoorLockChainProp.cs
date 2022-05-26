using Module;
using UnityEngine;
/// <summary>
/// 锁链（射击会断裂）
/// </summary>
public class DoorLockChainProp : PropsBase , IHurtObject
{
    protected override bool OnInteractive(bool fromMonster = false)
    {
        return false;
    }

    public Hurtmaterial hurtMaterial { get; }

    public Damage OnHurt(ITarget target, Damage damage)
    {
        if (damage.damage <= 0 || (damage.weapon & WeaponType.Thrown) != 0 || (damage.weapon & WeaponType.Empty) != 0 || (damage.weapon & WeaponType.MeleeWeapon) != 0)
        {
            return damage;
        }
        
        //TODO 播放特效，范围伤害+硬直
        RunLogicalOnSelf(RunLogicalName.Destroy);
        return damage;
    }
}