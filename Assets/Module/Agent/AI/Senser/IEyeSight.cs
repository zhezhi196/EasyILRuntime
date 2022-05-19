using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public interface IEyeSight
    {
        Vector3 offset { get; }
        List<IEyeSight> includeSights { get; }
        List<IEyeSight> excludeSights { get; }
        Vector3 center { get; }
        bool ContainPoint(Vector3 point);
        void DrawGizmos(Color color);
    }
}