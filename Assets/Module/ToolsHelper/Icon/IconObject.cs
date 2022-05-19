using System;
using UnityEngine;

namespace Module
{
    public interface IconObject
    {
        /// <summary>
        /// 获取图标
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        void GetIcon(string type, Action<Sprite> callback);
    }
}