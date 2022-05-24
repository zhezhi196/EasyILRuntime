using System.Collections.Generic;
using BehaviorDesigner.Editor;
using Module;
using UnityEditor;

namespace EditorModule
{
    [CustomEditor(typeof(AgentBehaviorTree), true)]
    public class AgentBehaviorTreeInspector : ExternalBehaviorTreeInspector
    {
        private List<SerializedProperty> property = new List<SerializedProperty>();

        private void OnEnable()
        {
            property.Add(serializedObject.FindProperty("StartWhenEnabled"));
            property.Add(serializedObject.FindProperty("pauseWhenDisabled"));
            property.Add(serializedObject.FindProperty("restartWhenComplete"));
            property.Add(serializedObject.FindProperty("resetValuesOnRestart"));
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