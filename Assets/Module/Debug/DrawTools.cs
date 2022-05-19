using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Module
{
    public static class DrawTools
    {
        [Conditional("LOG_ENABLE")]
        public static void DrawBounds(Bounds bounds,Quaternion rotation, Color color)
        {
            Vector3 ltf = rotation*(new Vector3(-bounds.extents.x, bounds.extents.y, -bounds.extents.z)) + bounds.center;
            Vector3 lbf = rotation*(new Vector3(-bounds.extents.x, -bounds.extents.y, -bounds.extents.z)) + bounds.center;
            Vector3 rbf = rotation*(new Vector3(bounds.extents.x, -bounds.extents.y, -bounds.extents.z)) + bounds.center;
            Vector3 rtf = rotation*(new Vector3(bounds.extents.x, bounds.extents.y, -bounds.extents.z)) + bounds.center;

            Vector3 ltb = rotation*(new Vector3(-bounds.extents.x, bounds.extents.y, bounds.extents.z)) + bounds.center;
            Vector3 lbb = rotation*(new Vector3(-bounds.extents.x, -bounds.extents.y, bounds.extents.z)) + bounds.center;
            Vector3 rbb = rotation*(new Vector3(bounds.extents.x, -bounds.extents.y, bounds.extents.z)) + bounds.center;
            Vector3 rtb = rotation*(new Vector3(bounds.extents.x, bounds.extents.y, bounds.extents.z)) + bounds.center;

            GameDebug.DrawLine(ltf, lbf, color);
            GameDebug.DrawLine(lbf, rbf, color);
            GameDebug.DrawLine(rbf, rtf, color);
            GameDebug.DrawLine(rtf, ltf, color);
            
            GameDebug.DrawLine(ltb, lbb, color);
            GameDebug.DrawLine(lbb, rbb, color);
            GameDebug.DrawLine(rbb, rtb, color);
            GameDebug.DrawLine(rtb, ltb, color);
            //
            GameDebug.DrawLine(ltf, ltb, color);
            GameDebug.DrawLine(lbf, lbb, color);
            GameDebug.DrawLine(rbf, rbb, color);
            GameDebug.DrawLine(rtf, rtb, color);
        }
        [Conditional("LOG_ENABLE")]
        public static void DrawBounds(Bounds bounds,Quaternion rotation, Color color,float duation)
        {
            Vector3 ltf = rotation*(new Vector3(-bounds.extents.x, bounds.extents.y, -bounds.extents.z)) + bounds.center;
            Vector3 lbf = rotation*(new Vector3(-bounds.extents.x, -bounds.extents.y, -bounds.extents.z)) + bounds.center;
            Vector3 rbf = rotation*(new Vector3(bounds.extents.x, -bounds.extents.y, -bounds.extents.z)) + bounds.center;
            Vector3 rtf = rotation*(new Vector3(bounds.extents.x, bounds.extents.y, -bounds.extents.z)) + bounds.center;

            Vector3 ltb = rotation*(new Vector3(-bounds.extents.x, bounds.extents.y, bounds.extents.z)) + bounds.center;
            Vector3 lbb = rotation*(new Vector3(-bounds.extents.x, -bounds.extents.y, bounds.extents.z)) + bounds.center;
            Vector3 rbb = rotation*(new Vector3(bounds.extents.x, -bounds.extents.y, bounds.extents.z)) + bounds.center;
            Vector3 rtb = rotation*(new Vector3(bounds.extents.x, bounds.extents.y, bounds.extents.z)) + bounds.center;

            GameDebug.DrawLine(ltf, lbf, color,duation);
            GameDebug.DrawLine(lbf, rbf, color,duation);
            GameDebug.DrawLine(rbf, rtf, color,duation);
            GameDebug.DrawLine(rtf, ltf, color,duation);
            
            GameDebug.DrawLine(ltb, lbb, color,duation);
            GameDebug.DrawLine(lbb, rbb, color,duation);
            GameDebug.DrawLine(rbb, rtb, color,duation);
            GameDebug.DrawLine(rtb, ltb, color,duation);
            //
            GameDebug.DrawLine(ltf, ltb, color,duation);
            GameDebug.DrawLine(lbf, lbb, color,duation);
            GameDebug.DrawLine(rbf, rbb, color,duation);
            GameDebug.DrawLine(rtf, rtb, color,duation);
        }

        [Conditional("LOG_ENABLE")]
        public static void DrawText(Vector3 position, string text,int fontSize)
        {
            #if UNITY_EDITOR
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.normal.textColor = Color.red;
            style.fontSize = fontSize;
            UnityEditor.Handles.Label(position, text, style);
            #endif
        }
        /// <summary>
        /// 画扇形
        /// </summary>
        /// <param name="center"></param>
        /// <param name="normal"></param>
        /// <param name="radius"></param>
        /// <param name="maxDig"></param>
        /// <param name="color"></param>
        [Conditional("LOG_ENABLE")]
        public static void DrawSector(Vector3 center, Quaternion rotation, float radius, float maxDig, Color color)
        {
            if (maxDig < 360 && maxDig > 0)
            {
                int step = 5;
                Vector3 first = GetCirclePos(-maxDig * 0.5f, rotation, radius);
                Debug.DrawLine(center, first + center, color);
                for (float i = -maxDig * 0.5f; i <= maxDig * 0.5f; i += step)
                {
                    Vector3 next = GetCirclePos(i, rotation, radius);
                    Debug.DrawLine(center + first, center + next, color);
                    first = next;
                }

                Debug.DrawLine(center, first + center, color);
            }
        }
        
        /// <summary>
        /// 画半圆
        /// </summary>
        /// <param name="position"></param>
        /// <param name="normal"></param>
        /// <param name="radius"></param>
        /// <param name="maxDig"></param>
        /// <param name="color"></param>
        [Conditional("LOG_ENABLE")]
        public static void DrawCircle(Vector3 center, Quaternion rotation, float radius, float maxDig, Color color)
        {
            if (maxDig < 360 && maxDig > 0)
            {
                int step = 5;
                Vector3 first = GetCirclePos(-maxDig * 0.5f, rotation, radius);
                for (float i = -maxDig * 0.5f; i <= maxDig * 0.5f; i += step)
                {
                    Vector3 next = GetCirclePos(i, rotation, radius);
                    Debug.DrawLine(center + first, center + next, color);
                    first = next;
                }
            }
        }
        /// <summary>
        /// 画圆
        /// </summary>
        /// <param name="position"></param>
        /// <param name="normal"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        [Conditional("LOG_ENABLE")]
        public static void DrawCircle(Vector3 center, Quaternion rotation, float radius, Color color)
        {
            int step = 5;
            Vector3 first = GetCirclePos(0, rotation, radius);
            for (float i = 0; i <= 360; i += step)
            {
                Vector3 next = GetCirclePos(i, rotation, radius);
                Debug.DrawLine(center + first, center + next, color);
                first = next;
            }
        }
        
        private static Vector3 GetCirclePos(float angle, Quaternion rotation, float radius)
        {
            angle = angle * Mathf.Deg2Rad;
            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);
            float z = 0;
            return (rotation * new Vector3(x, y, z));
        }
        [Conditional("LOG_ENABLE")]
        public static void DrawCameraView(Transform eye, float viewAngle, float aspect, float distance)
        {
            Vector3[] corners = GetCorners(eye, viewAngle, aspect, distance);
            OnDrawFarView(corners);
            OnDrawConeOfCameraVision(eye.position, corners);
        }

        //获取相机视口四个角的坐标
        //参数 distance  视口距离
        private static Vector3[] GetCorners(Transform eye, float viewAngle, float aspect, float distance)
        {
            Vector3[] corners = new Vector3[4];
            float halfFOV = (viewAngle * 0.5f) * Mathf.Deg2Rad; //一半fov
            
            float height = distance * Mathf.Tan(halfFOV);//distance距离位置，相机视口高度的一半
            float width = height * aspect;//相机视口宽度的一半
            
            //左上
            corners[0] = eye.transform.position - (eye.right * width);//相机坐标 - 视口宽的一半
            corners[0] += eye.up * height;//+视口高的一半
            corners[0] += eye.forward * distance;//+视口距离
            
            // 右上
            corners[1] =eye.position + (eye.right * width);//相机坐标 + 视口宽的一半
            corners[1] += eye.up * height;//+视口高的一半
            corners[1] += eye.forward * distance;//+视口距离
            
            // 左下
            corners[2] = eye.position - (eye.right * width);//相机坐标 - 视口宽的一半
            corners[2] -= eye.up * height;//-视口高的一半
            corners[2] += eye.forward * distance;//+视口距离
            
            // 右下
            corners[3] = eye.position + (eye.right * width);//相机坐标 + 视口宽的一半
            corners[3] -= eye.up * height;//-视口高的一半
            corners[3] += eye.forward * distance;//+视口距离
            return corners;
        }
        
        private static void OnDrawFarView(Vector3[] corners)
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
        private static void OnDrawConeOfCameraVision(Vector3 frompoint,Vector3[] corners)
        {
            // for debugging
            Gizmos.DrawLine(frompoint, corners[1]); // UpperLeft -> UpperRight
            Gizmos.DrawLine(frompoint, corners[3]); // UpperRight -> LowerRight
            Gizmos.DrawLine(frompoint, corners[2]); // LowerRight -> LowerLeft
            Gizmos.DrawLine(frompoint, corners[0]); // LowerLeft -> UpperLeft
        }
    }
}