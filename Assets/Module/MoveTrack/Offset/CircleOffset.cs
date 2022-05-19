using UnityEngine;

namespace Module
{
    public struct CircleOffset : ITrackOffset
    {
        public float radius;
        private Quaternion convertQua;
        public CircleOffset(float radius, Vector3 axi)
        {
            this.radius = radius;
            convertQua = Quaternion.Euler(axi.normalized * 90);
        }

        public Vector3 GetTrackOffset(float percent)
        {
            float angal = 2 * Mathf.PI * percent;
            Vector3 absV3 = new Vector3(Mathf.Cos(angal), 0, Mathf.Sin(angal)) * radius;
            return convertQua * absV3;
        }
    }
}