/*
 * 脚本名称：UIBtnBaseEditor
 * 项目名称：Bow
 * 脚本作者：黄哲智
 * 创建时间：2018-10-08 14:26:19
 * 脚本作用：
*/

using System.Collections.Generic;
using System.Reflection;
using Module;
using UnityEditor;
using UnityEditor.UI;

namespace EditorModule
{
    [CustomEditor(typeof(UIBtnBase),true)]
    public class UIBtnBaseEditor : ButtonEditor
    {
        private List<SerializedProperty> property = new List<SerializedProperty>();
        
        
        protected override void OnEnable()
        {
            base.OnEnable();
            property.Clear();
            FieldInfo[] field = target.GetType().GetFields();
            for (int i = 0; i < field.Length; i++)
            {
                if (field[i].Name != "clickAction")
                {
                    SerializedProperty s = serializedObject.FindProperty(field[i].Name);
                    if (s != null)
                    {
                        property.Add(s);
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            for (int i = 0; i < property.Count; i++)
            {
                EditorGUILayout.PropertyField(property[i]);
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }

}


