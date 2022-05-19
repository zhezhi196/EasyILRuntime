using UnityEngine;

namespace Module
{
    public struct MultipleOffset : ITrackOffset
    {
        public ITrackOffset[] offset;
        public MultipleOffset(params ITrackOffset[] offset)
        {
            this.offset = offset;
        }

        public Vector3 GetTrackOffset(float percent)
        {
            Vector3 result = Vector3.zero;
            for (int i = 0; i < offset.Length; i++)
            {
                result += offset[i].GetTrackOffset(percent);
            }

            return result;
        }
    }
}