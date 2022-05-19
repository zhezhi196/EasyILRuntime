using System;
using UnityEngine;

namespace Module
{
    public interface IMoveTrack
    {
        /// <summary>
        /// 是否激活
        /// </summary>
        bool isActive { get; }
        
        /// <summary>
        /// 经过若干次偏移到达终点
        /// </summary>
        float loop { get; set; }

        /// <summary>
        /// 是否一直移动,无尽头
        /// </summary>
        bool noEnd { get; set; }
        
        /// <summary>
        /// 基点运动方程
        /// </summary>
        IEquation equation { get; set; }
        
        /// <summary>
        /// 偏移
        /// </summary>
        ITrackOffset offset { get; set; }

        /// <summary>
        /// 当到达终点时的回调
        /// </summary>
        event Action onComplete;

        /// <summary>
        /// 每帧传进去detaTime,获取对应的位置
        /// </summary>
        /// <param name="detaTime"></param>
        /// <returns></returns>
        Vector3 UpdatePosition(float detaTime);

        /// <summary>
        /// 每帧传进去detaTime,修改终点位置,获得现在应该的位置
        /// </summary>
        /// <param name="detaTime"></param>
        /// <returns></returns>
        Vector3 UpdatePosition(float detaTime, Vector3 endValue);

        void ChangeEndValue(Vector3 endValue);
    }
}