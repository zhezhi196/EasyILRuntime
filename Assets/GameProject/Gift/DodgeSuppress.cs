using Project.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;
namespace GameGift
{
    /// <summary>
    /// 闪避压制
    /// 近战攻击，变为超重击
    /// </summary>
    public class DodgeSuppress : Gift
    {
        float arg = 0;
        public Clock buffClock;
        WeaponHammer weapon;
        public DodgeSuppress(GiftData data, GiftSaveData saveData) : base(data, saveData)
        {
            arg = data.giftArg1;
        }
        public override void ActivateGife()
        {
            weapon = Player.player.weaponManager.FindWeapon(WeaponType.MeleeWeapon) as  WeaponHammer;
            buffClock = new Clock(arg);
            buffClock.autoKill = false;
            buffClock.onComplete += OnClockComplete;
            EventCenter.Register(EventKey.PerfectDodge, OnPerfectDodge);
        }

        private void OnPerfectDodge()
        {
            //重置计时器
            buffClock.Restart();
            weapon.strongAttack = true;
        }

        private void OnClockComplete()
        {
            weapon.strongAttack = false;
        }
        public override void ExitBattle()
        {
            buffClock?.Stop();
            EventCenter.UnRegister(EventKey.PerfectDodge, OnPerfectDodge);
        }
    }
}
