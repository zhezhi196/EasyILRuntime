using Module;
using Project.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameGift
{
    /// <summary>
    /// 闪避反击
    /// 完美闪避后，增加枪械伤害
    /// </summary>
    public class DodgeAttack : Gift
    {
        float arg = 0;
        public Clock buffClock;
        public DodgeAttack(GiftData data, GiftSaveData saveData) : base(data, saveData)
        {
            arg = data.giftArg1;
        }
        public override GameAttribute GetGrowAtt(Weapon weapon, MonsterPart part)
        {
            if (buffClock.isActive)
            {
                return growUpAttribute;
            }
            return base.GetGrowAtt(weapon, part);
        }

        public override void ActivateGife()
        {
            EventCenter.Register(EventKey.PerfectDodge, OnPerfectDodge);
            buffClock = new Clock(arg);
            buffClock.autoKill = false;
        }

        private void OnPerfectDodge()
        {
            //重置计时器
            buffClock.Restart();
        }

        public override void ExitBattle()
        {
            buffClock?.Stop();
            EventCenter.UnRegister(EventKey.PerfectDodge, OnPerfectDodge);
        }
    }
}
