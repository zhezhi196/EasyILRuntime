using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public class DistanceEyeSight : IEyeSight
    {
        public float distance { get; }
        public float viewAngle { get; }

        public bool isBlind { get; set; }
        public Vector3 offset { get; }
        public List<IEyeSight> includeSights { get; }
        public List<IEyeSight> excludeSights { get; }

        public Vector3 center
        {
            get { return transform.position+offset; }
        }

        public Transform transform { get; }

        public DistanceEyeSight(Transform center, float distance, float angle)
        {
            this.transform = center;
            this.distance = distance;
            this.viewAngle = angle;
        }

        public bool ContainPoint(Vector3 point)
        {
            if (isBlind) return false;
            Vector3 slicePos = point - center;
            Vector3 tarDir = new Vector3(slicePos.x, 0, slicePos.z);
            float angle = Vector3.Angle(tarDir, transform.forward);
            float currDistance = center.Distance(point);
            bool result = currDistance <= distance && angle <= this.viewAngle * 0.5f;
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
            Quaternion rotation = Quaternion.LookRotation(Vector3.up, transform.right);
            DrawTools.DrawSector(transform.position, rotation, distance, viewAngle, color);
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