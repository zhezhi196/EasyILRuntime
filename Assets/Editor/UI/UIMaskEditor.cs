using System.Collections;
using System.Collections.Generic;
using Module;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIMask))]
public class UIMaskEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        for (int i = 0; i < UIComponent.freezeList.Count; i++)
        {
            EditorGUILayout.LabelField(UIComponent.freezeList[i]);
        }
    }
}
