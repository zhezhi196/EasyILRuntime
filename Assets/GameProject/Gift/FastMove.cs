using Project.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameGift
{
    /// <summary>
    /// 可以进行暗杀
    /// </summary>
    public class FastMove : Gift
    {
        public FastMove(GiftData data, GiftSaveData saveData) : base(data, saveData)
        {
        }

        public override void ActivateGife()
        {
            //可以暗杀
            Player.player.canAssMonster = true;
        }
    }
}
