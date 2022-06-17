using System.Reflection;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace EditorModule
{
    [CustomEditor(typeof(Poisionous), true)]
    public class PoisionousEditor : OdinEditor
    {
        private FieldInfo[] field;
        private object[] fieldStr;
        protected BoxBoundsHandle m_BoundsHandle = new BoxBoundsHandle();
        public Poisionous trigger;

        protected override void OnEnable()
        {
            trigger = (Poisionous) target;
            m_BoundsHandle.SetColor(Color.green);
            m_BoundsHandle.center = trigger.bounds.center;
            m_BoundsHandle.size = trigger.bounds.size;
            if (trigger.bounds.center == Vector3.zero)
            {
                trigger.bounds.center = Vector3.zero;
            }

            if (trigger.bounds.size == Vector3.zero)
            {
                trigger.bounds.size = Vector3.one;
            }
        }

        protected void OnSceneGUI()
        {
            // var rotation = trigger.transform.rotation;
            // //创建平移 旋转 缩放矩阵
            // Matrix4x4 rotatedMatrix = Handles.matrix * Matrix4x4.TRS(trigger.transform.position, rotation, Vector3.one);
            // //局部坐标*mat = 世界坐标
            // //世界坐标*mat的逆 = 局部坐标
            // //获得boundshandle新坐标下的位置
            // if (m_BoundsHandle.center != Vector3.zero)
            // {
            //     trigger.bounds.center = m_BoundsHandle.center;
            // }
            //
            // if (m_BoundsHandle.size != Vector3.zero)
            // {
            //     trigger.bounds.size = m_BoundsHandle.size;
            // }
            //
            // // trigger.bounds.size = m_BoundsHandle.size;
            // EditorGUI.BeginChangeCheck();
            // //DrawingScope
            // //用于自动设置和恢复 Handles.color 和/或 Handles.matrix 的可处置 Helper 结构。
            // //此结构允许您暂时设置代码块内的 Handles.color 和/ 或 Handles.matrix 的值，并在退出作用域时自动将它们恢复为之前的值。
            // using (new Handles.DrawingScope(rotatedMatrix))
            // {
            //     m_BoundsHandle.DrawHandle();
            // }
            //
            // if (EditorGUI.EndChangeCheck())
            // {
            //     // record the target object before setting new values so changes can be undone/redone
            //     Undo.RecordObject(trigger, "Change Bounds");
            //     // copy the handle's updated data back to the target object
            //     //trigger.transform.localScale = m_BoundsHandle.size;
            // }
        }
    }
}