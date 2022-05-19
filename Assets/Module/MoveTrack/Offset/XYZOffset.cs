using UnityEngine;

namespace Module
{
    public struct XYZOffset : ITrackOffset
    {
        public static XYZOffset GetDefaultOffset(float time)
        {
            AnimationCurve curve = AnimationCurve.Linear(0, 0, time, 0);
            return new XYZOffset(curve, curve, curve);
        }

        public AnimationCurve xOffset;
        public AnimationCurve yOffset;
        public AnimationCurve zOffset;

        public XYZOffset(AnimationCurve x, AnimationCurve y, AnimationCurve z)
        {
            this.xOffset = x;
            this.yOffset = y;
            this.zOffset = z;
        }

        public Vector3 GetTrackOffset(float percent)
        {
            float ox = xOffset.Evaluate(percent);
            float oy = yOffset.Evaluate(percent);
            float oz = zOffset.Evaluate(percent);
            return new Vector3(ox, oy, oz);
        }
    }
}