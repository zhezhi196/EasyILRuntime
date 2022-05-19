using UnityEngine;

namespace Module
{

    public struct Line
    {
        public Vector3 from;
        public Vector3 to;

        public float distance
        {
            get
            {
                return @from.Distance(to);
            }
        }

        public Line(Vector3 from, Vector3 to)
        {
            this.@from = from;
            this.to = to;
        }

        public Vector3 direction
        {
            get { return (to - @from).normalized; }
        }

        public Vector3 GetPoint(float distance)
        {
            return @from + direction * distance;
        }

        public static bool IsForcus(Line line1, Line line2,out Vector2 intersectPos)
        {
            intersectPos = Vector3.zero;
            Vector3 a = line1.@from;
            Vector3 b = line1.to;
            Vector3 c = line2.@from;
            Vector3 d = line2.to;
            
            Vector3 ab = b - a;
            Vector3 ca = a - c;
            Vector3 cd = d - c;

            Vector3 v1 = Vector3.Cross(ca, cd);

            if (Mathf.Abs(Vector3.Dot(v1, ab)) > 1e-6)
            {
                // 不共面
                return false;
            }

            if (Vector3.Cross(ab, cd).sqrMagnitude <= 1e-6)
            {
                // 平行
                return false;
            }

            Vector3 ad = d - a;
            Vector3 cb = b - c;
            // 快速排斥
            if (Mathf.Min(a.x, b.x) > Mathf.Max(c.x, d.x) || Mathf.Max(a.x, b.x) < Mathf.Min(c.x, d.x)
                                                          || Mathf.Min(a.y, b.y) > Mathf.Max(c.y, d.y) || Mathf.Max(a.y, b.y) < Mathf.Min(c.y, d.y)
                                                          || Mathf.Min(a.z, b.z) > Mathf.Max(c.z, d.z) || Mathf.Max(a.z, b.z) < Mathf.Min(c.z, d.z)
            )
                return false;

            // 跨立试验
            if (Vector3.Dot(Vector3.Cross(-ca, ab), Vector3.Cross(ab, ad)) > 0
                && Vector3.Dot(Vector3.Cross(ca, cd), Vector3.Cross(cd, cb)) > 0)
            {
                Vector3 v2 = Vector3.Cross(cd, ab);
                float ratio = Vector3.Dot(v1, v2) / v2.sqrMagnitude;
                intersectPos = a + ab * ratio;
                return true;
            }

            return false;
        }
    }
}