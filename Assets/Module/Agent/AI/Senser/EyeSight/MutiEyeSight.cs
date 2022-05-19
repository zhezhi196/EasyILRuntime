using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public class MutiEyeSight : IEyeSight
    {
        public Vector3 offset { get; }
        public List<IEyeSight> includeSights { get; }
        public List<IEyeSight> excludeSights { get; }

        public Vector3 center
        {
            get
            {
                if(includeSights.IsNullOrEmpty()) return Vector3.zero;
                Vector3 v = includeSights[0].center;
                for (int i = 1; i < includeSights.Count; i++)
                {
                    v = (v + includeSights[i].center) * 0.5f;
                }
                return v+offset;
            }
        }

        public bool ContainPoint(Vector3 point)
        {
            bool result = false;
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