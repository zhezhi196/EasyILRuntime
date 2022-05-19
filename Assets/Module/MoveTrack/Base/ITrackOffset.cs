using UnityEngine;

namespace Module
{
    public interface ITrackOffset
    {
        Vector3 GetTrackOffset(float percent);
    }
}