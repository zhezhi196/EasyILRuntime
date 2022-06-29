using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public class Spherical : IEyeSight
    {
        public bool isBlind { get; set; }
        public Vector3 offset { get; }
        public List<IEyeSight> includeSights { get; }
        public List<IEyeSight> excludeSights { get; }

        public Vector3 center
        {
            get { return transform.position+offset; }
        }

        public float radius { get; }
        public Transform transform;

        public Spherical(Transform center, float radius)
        {
            this.transform = center;
            this.radius = radius;
        }
        public Spherical(Transform center, float radius,Vector3 offset)
        {
            this.transform = center;
            this.radius = radius;
            this.offset = offset;
        }

        public bool ContainPoint(Vector3 point)
        {
            if (isBlind) return false;
            float dis = center.Distance(point);
            bool result = dis <= radius;
            if (!includeSights.IsNullOrEmpty())
            {
                for (int i = 0; i < includeSights.Count; i++)
                {
                    result = result || includeSights[i].ContainPoint(point);
                }

                if (result)
                {
                    if (!excludeSights.IsNullOrEmpty())
                    {
                        for (int i = 0; i < excludeSights.Count; i++)
                        {
                            result = result && !excludeSights[i].ContainPoint(point);
                        }
                    }
                }
            }

            return result;
        }

        public void DrawGizmos(Color color)
        {
            if(isBlind) return;
            Gizmos.color = color;
            Gizmos.DrawWireSphere(center, radius);
            if (!includeSights.IsNullOrEmpty())
            {
                for (int i = 0; i < includeSights.Count; i++)
                {
                    includeSights[i].DrawGizmos(color);
                }
            }
        }
    }
}