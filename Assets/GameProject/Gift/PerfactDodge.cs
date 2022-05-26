using Project.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;

namespace GameGift
{
    /// <summary>
    /// 完美闪避
    /// 回复体力
    /// </summary>
    public class PerfactDodge : Gift
    {
        float arg = 0;
        public PerfactDodge(GiftData data, GiftSaveData saveData) : base(data, saveData)
        {
            arg = data.giftArg1;
        }

        public override void ActivateGife()
        {
            EventCenter.Register(EventKey.PerfectDodge, OnPerfectDodge);
        }

        private void OnPerfectDodge()
        {
            //回复体力
            Player.player.ChangeStrength(arg);
        }

        public override void ExitBattle()
        {
            EventCenter.UnRegister(EventKey.PerfectDodge, OnPerfectDodge);
        }
    }
}
