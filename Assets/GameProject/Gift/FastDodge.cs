using Project.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameGift
{
    /// <summary>
    /// 灵巧身法
    /// 增加完美闪避判定时间
    /// </summary>
    public class FastDodge : Gift
    {
        float arg = 0;
        public FastDodge(GiftData data, GiftSaveData saveData) : base(data, saveData)
        {
            arg = data.giftArg1;
        }
        public override void ActivateGife()
        {
            Player.player.dodgeTimeAdd.value += arg;
        }
    }
}