using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace ModuleEditor
{
    public class SkillEditorData
    {
        public int id;
        public string name;
        public List<SkillEditorTimeNode> timeLine;
    }

    public class SkillEditorTimeNode
    {
        [DisplayAsString]
        public int index;
        
        [CustomValueDrawer("OnTimeLineChange")]
        public double timePercent;

        private double OnTimeLineChange(double value, GUIContent label)
        {
            return EditorGUILayout.Slider(label, (float)value, 0, 1);
        }
    }
}