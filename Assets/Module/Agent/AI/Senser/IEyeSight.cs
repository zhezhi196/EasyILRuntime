using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public interface IEyeSight
    {
        bool isBlind { get; set; }
        Vector3 offset { get; }
        List<IEyeSight> includeSights { get; }
        List<IEyeSight> excludeSights { get; }
        Vector3 center { get; }
        bool ContainPoint(Vector3 point);
        void DrawGizmos(Color color);
    }
}