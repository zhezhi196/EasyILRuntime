using Project.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameGift
{
    /// <summary>
    /// 蓄力攻击
    /// </summary>
    public class ChargeAttack : Gift
    {
        public ChargeAttack(GiftData data, GiftSaveData saveData) : base(data, saveData)
        {
        }

        public override void ActivateGife()
        {
            Weapon weapon = Player.player?.weaponManager.FindWeapon(WeaponType.MeleeWeapon);
            //设置锤子可以蓄力攻击
            if (weapon != null)
            {
                (weapon as WeaponHammer).canCharge = true;
            }
        }
    }
}
