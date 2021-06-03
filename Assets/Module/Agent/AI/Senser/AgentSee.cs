using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Module
{
    public class AgentSee : AgentSensor
    {
        public static List<ISensorTarget> target = new List<ISensorTarget>();
        private Plane[] viewPlane = new Plane[4];
        private Vector3[] viewCornors = new Vector3[4];
        
        [LabelText("视野角度")]
        public float viewAngle = 90;
        [LabelText("视野距离")]
        public float viewDistance = 10;
        [LabelText("宽高比")] 
        public float aspect = 1;
        
        public List<ISensorTarget> onViewTarget = new List<ISensorTarget>();


        /// <summary>
        /// 当物体进入视野后的回调
        /// </summary>
        public event Action<ISensorTarget> onFousTarget;
        /// <summary>
        /// 当丢失掉物体视野后的回调
        /// </summary>
        public event Action<ISensorTarget> onLoseTarget;
        /// <summary>
        /// 当物体在视野内,每一帧会调这个视角用于监听视野内的物体的状态
        /// </summary>
        public event Action<ISensorTarget> onSenserTarget;

        private void Update()
        {
            var tempTarget = GetTarget();
            for (int i = 0; i < tempTarget.Count; i++)
            {
                var tar = tempTarget[i];
                if (tar.isSenserable && SeeTarget(tar))
                {
                    if (!onViewTarget.Contains(tar))
                    {
                        onFousTarget?.Invoke(tar);
                        AddToTarget(tar);
                    }
                }
                else
                {
                    if (onViewTarget.Contains(tar))
                    {
                        onLoseTarget?.Invoke(onViewTarget[i]);
                        RemoveFromTarget(tar);
                    }
                }
            }
        }

        private void AddToTarget(ISensorTarget tar)
        {
            onViewTarget.Add(tar);
            tar.onDisable += RemoveFromTarget;
        }

        private void RemoveFromTarget(ISensorTarget tar)
        {
            onViewTarget.Remove(tar);
            tar.onDisable -= RemoveFromTarget;
        }

        public List<ISensorTarget> GetTarget()
        {
            return target;
        }

        private bool SeeTarget(ISensorTarget tar)
        {
            Vector3 tarPos = tar.transform.position;
            if (Vector3.Distance(transform.position, tarPos) <= viewDistance)
            {
                RefreshCornors(viewDistance);
                if (GetPlane(viewCornors, tarPos))
                {
                    RaycastHit hit;
                    if (!Physics.Raycast(transform.position, tarPos - transform.position, out hit, viewDistance, layerMask))
                    {
                        onSenserTarget?.Invoke(tar);
                        return true;
                    }
                }
            }

            return false;
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
        
#if UNITY_EDITOR
        #region EditorView

        [Flags]
        public enum SightFlag
        {
            [LabelText("显示锥形线框")]
            ShowCone = 1,
            [LabelText("显示视野范围")]
            ShowSight = 2,
            [LabelText("显示视野内目标")]
            ShowTarget = 4
        }
        public SightFlag flag = SightFlag.ShowCone | SightFlag.ShowSight;
        public Color gimosColor = Color.green;
        public Color targetInView = Color.red;

        private void OnDrawGizmos()
        {
            Gizmos.color = gimosColor;
            if ((flag & SightFlag.ShowCone) != 0)
            {
                Vector3[] corners = GetCorners(viewDistance);
                OnDrawFarView(corners);
                OnDrawConeOfCameraVision(corners);
            }


            if ((flag & SightFlag.ShowTarget) != 0)
            {
                DrawTarget();
            }
        }

        private void DrawTarget()
        {
            Gizmos.color = targetInView;
            for (int i = 0; i < onViewTarget.Count; i++)
            {
                Gizmos.DrawSphere(onViewTarget[i].transform.position, 1.2f);
            }
        }

        public void DrawLineOfSight(Transform transform, Vector3 positionOffset, float fieldOfViewAngle, float viewDistance)
        {
            UnityEditor.Handles.color = new Color(gimosColor.r,gimosColor.g,gimosColor.b,0.05f);
            var halfFOV = fieldOfViewAngle * 0.5f;
            var beginDirection = Quaternion.AngleAxis(-halfFOV, Vector3.up) * transform.forward;
            UnityEditor.Handles.DrawSolidArc(transform.TransformPoint(positionOffset), transform.up, beginDirection, fieldOfViewAngle, viewDistance);
        }


        void OnDrawFarView(Vector3[] corners)
        {
            // for debugging

            Gizmos.DrawLine(corners[0], corners[1]); // UpperLeft -> UpperRight
            Gizmos.DrawLine(corners[1], corners[3]); // UpperRight -> LowerRight
            Gizmos.DrawLine(corners[3], corners[2]); // LowerRight -> LowerLeft
            Gizmos.DrawLine(corners[2], corners[0]); // LowerLeft -> UpperLeft
        }
        
        /// <summary>
        /// 绘制 camera 的视锥 边沿
        /// </summary>
        void OnDrawConeOfCameraVision(Vector3[] corners)
        {
            // for debugging
            Gizmos.DrawLine(transform.position, corners[1]); // UpperLeft -> UpperRight
            Gizmos.DrawLine(transform.position, corners[3]); // UpperRight -> LowerRight
            Gizmos.DrawLine(transform.position, corners[2]); // LowerRight -> LowerLeft
            Gizmos.DrawLine(transform.position, corners[0]); // LowerLeft -> UpperLeft
        }


 
        //获取相机视口四个角的坐标
        //参数 distance  视口距离
        Vector3[] GetCorners(float distance)
        {
            Vector3[] corners = new Vector3[4];
            float halfFOV = (viewAngle * 0.5f) * Mathf.Deg2Rad;//一半fov
            float aspect = this.aspect;//相机视口宽高比
            
            float height = distance * Mathf.Tan(halfFOV);//distance距离位置，相机视口高度的一半
            float width = height * aspect;//相机视口宽度的一半
            
            //左上
            corners[0] = transform.position - (transform.right * width);//相机坐标 - 视口宽的一半
            corners[0] += transform.up * height;//+视口高的一半
            corners[0] += transform.forward * distance;//+视口距离
            
            // 右上
            corners[1] = transform.position + (transform.right * width);//相机坐标 + 视口宽的一半
            corners[1] += transform.up * height;//+视口高的一半
            corners[1] += transform.forward * distance;//+视口距离
            
            // 左下
            corners[2] = transform.position - (transform.right * width);//相机坐标 - 视口宽的一半
            corners[2] -= transform.up * height;//-视口高的一半
            corners[2] += transform.forward * distance;//+视口距离
            
            // 右下
            corners[3] = transform.position + (transform.right * width);//相机坐标 + 视口宽的一半
            corners[3] -= transform.up * height;//-视口高的一半
            corners[3] += transform.forward * distance;//+视口距离
            return corners;
        }

        #endregion
#endif
    }
}