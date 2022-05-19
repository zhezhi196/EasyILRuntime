using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public class ConicalEyeSight : IEyeSight
    {
        private Plane[] viewPlane = new Plane[4];
        private Vector3[] viewCornors = new Vector3[4];
        public float aspect;

        public Vector3 offset { get; }
        public List<IEyeSight> includeSights { get; }
        public List<IEyeSight> excludeSights { get; }

        public Vector3 center
        {
            get { return transform.position+offset; }
        }

        public float viewDistance { get; }
        public float viewAngle { get; }
        public Transform transform { get; }

        public ConicalEyeSight(Transform transform, float viewDistance, float viewAngle, float aspect)
        {
            this.transform = transform;
            this.viewAngle = viewAngle;
            this.viewDistance = viewDistance;
            this.aspect = aspect;
        }

        public bool ContainPoint(Vector3 point)
        {
            bool result = true;
            RefreshCornors(viewDistance);
            result = result && GetPlane(viewCornors, point);
            
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
        
        public void RefreshCornors(float distance)
        {
            float halfFOV = (viewAngle * 0.5f) * Mathf.Deg2Rad;//一半fov
            float aspect = this.aspect;//相机视口宽高比
            
            float height = distance * Mathf.Tan(halfFOV);//distance距离位置，相机视口高度的一半
            float width = height * aspect;//相机视口宽度的一半
            
            //左上
            viewCornors[0] = transform.position - (transform.right * width);//相机坐标 - 视口宽的一半
            viewCornors[0] += transform.up * height;//+视口高的一半
            viewCornors[0] += transform.forward * distance;//+视口距离
            
            // 右上
            viewCornors[1] = transform.position + (transform.right * width);//相机坐标 + 视口宽的一半
            viewCornors[1] += transform.up * height;//+视口高的一半
            viewCornors[1] += transform.forward * distance;//+视口距离
            
            // 左下
            viewCornors[2] = transform.position - (transform.right * width);//相机坐标 - 视口宽的一半
            viewCornors[2] -= transform.up * height;//-视口高的一半
            viewCornors[2] += transform.forward * distance;//+视口距离
            
            // 右下
            viewCornors[3] = transform.position + (transform.right * width);//相机坐标 + 视口宽的一半
            viewCornors[3] -= transform.up * height;//-视口高的一半
            viewCornors[3] += transform.forward * distance;//+视口距离
        }
        
        public bool GetPlane(Vector3[] cornor, Vector3 tar)
        {
            viewPlane[0] = new Plane(transform.position, cornor[1], cornor[0]);
            viewPlane[1] = new Plane(transform.position, cornor[3], cornor[1]);
            viewPlane[2] = new Plane(transform.position, cornor[2], cornor[3]);
            viewPlane[3] = new Plane(transform.position, cornor[0], cornor[2]);
            for (int i = 0; i < viewPlane.Length; i++)
            {
                if (!viewPlane[i].GetSide(tar))
                {
                    return false;
                }
            }

            return true;
        }
        public void DrawGizmos(Color color)
        {
            Gizmos.color = color;
            DrawTools.DrawCameraView(transform, viewAngle, aspect, viewDistance);
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