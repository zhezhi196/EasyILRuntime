using UnityEngine;

namespace Module
{
    /// <summary>
    /// 所有可以作为目标的基类
    /// </summary>
    public interface ITarget : IVisiableObject
    {
        Transform transform { get; }
        Vector3 targetPoint { get; }
    }
}