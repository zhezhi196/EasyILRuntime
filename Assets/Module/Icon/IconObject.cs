using System;
using UnityEngine;

namespace Module
{
    public enum RewardIconType
    {
        Normal,
        HighDefinition,
        Locked,
    }
    
    public interface IconObject
    {
        /// <summary>
        /// 获取图标
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        void GetIcon(RewardIconType type, Action<Sprite> callback);
    }
}