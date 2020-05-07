using System;
using System.Collections.Generic;
using System.Reflection;
using Module;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomEditor(typeof(ViewReference))]
public class ViewReferenceEditor : Editor
{
    private ViewReference Target;
    private int tarIndex;

    private void OnEnable()
    {
        Target = (ViewReference) target;
        for (int i = 0; i < UnityStart.hotFixType.Count; i++)
        {
            if (Target.targetType == UnityStart.hotFixType[i])
            {
                tarIndex = i;
                break;
            }
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.BeginHorizontal();
        tarIndex = EditorGUILayout.Popup("Target", tarIndex, UnityStart.hotFixType.ToArray());
        if(UnityStart.hotFixType.Count==0) return;
        Target.targetType = UnityStart.hotFixType[tarIndex];
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Refresh"))
        {
            RefreshDll();
        }
        if (GUILayout.Button("Save"))
        {
            Object aa = PrefabUtility.GetCorrespondingObjectFromSource(target);
            string path = AssetDatabase.GetAssetPath(aa);
            PrefabUtility.SaveAsPrefabAsset(Target.gameObject, path);
        }

        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < Target.stringList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            (Target.stringList[i]).value = EditorGUILayout.TextField(Target.stringList[i].key, (Target.stringList[i]).value);
            EditorGUILayout.EndHorizontal();
        }
        for (int i = 0; i < Target.intList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            (Target.intList[i]).value = EditorGUILayout.LongField(Target.intList[i].key, (Target.intList[i]).value);
            EditorGUILayout.EndHorizontal();
        }
        for (int i = 0; i < Target.floatList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            (Target.floatList[i]).value = EditorGUILayout.DoubleField(Target.floatList[i].key, (Target.floatList[i]).value);
            EditorGUILayout.EndHorizontal();
        }
        for (int i = 0; i < Target.boolList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            (Target.boolList[i]).value = EditorGUILayout.Toggle(Target.boolList[i].key, (Target.boolList[i]).value);
            EditorGUILayout.EndHorizontal();
        }
        for (int i = 0; i < Target.animationList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            (Target.animationList[i]).value = EditorGUILayout.CurveField(Target.animationList[i].key, (Target.animationList[i]).value);
            EditorGUILayout.EndHorizontal();
        }
        for (int i = 0; i < Target.vector3List.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            (Target.vector3List[i]).value = EditorGUILayout.Vector3Field(Target.vector3List[i].key, (Target.vector3List[i]).value);
            EditorGUILayout.EndHorizontal();
        }
        for (int i = 0; i < Target.colorList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            (Target.colorList[i]).value = EditorGUILayout.ColorField(Target.colorList[i].key, (Target.colorList[i]).value);
            EditorGUILayout.EndHorizontal();
        }
        for (int i = 0; i < Target.objectList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            (Target.objectList[i]).value = (GameObject)EditorGUILayout.ObjectField(Target.objectList[i].key, (Target.objectList[i]).value,typeof(GameObject));
            EditorGUILayout.EndHorizontal();
        }

    }
    
    private void RefreshDll()
    {
        FieldInfo[] property = GetAllFiled();
        List<InspectorStringData> stringData = new List<InspectorStringData>();
        List<InspectorLongData> intData = new List<InspectorLongData>();
        List<InspectorDoubleData> floatData = new List<InspectorDoubleData>();
        List<InspectorBoolData> boolData = new List<InspectorBoolData>();
        List<InspectorAnimationCurveData> animationData = new List<InspectorAnimationCurveData>();
        List<InspectorVector3Data> vector3Data = new List<InspectorVector3Data>();
        List<InspectorColorData> colorData = new List<InspectorColorData>();
        List<InspectorObjectData> objectData = new List<InspectorObjectData>();
        
        for (int i = 0; i < property.Length; i++)
        {
            Type t = property[i].FieldType;

            if (t == typeof(string))
            {
                stringData.Add(new InspectorStringData(property[i].Name, string.Empty));
            }
            else if (t == typeof(int)|| t==typeof(long))
            {
                intData.Add(new InspectorLongData(property[i].Name, 0));
            }
            else if (t == typeof(float)|| t==typeof(double))
            {
                floatData.Add(new InspectorDoubleData(property[i].Name, 0d));
            }
            else if (t == typeof(bool))
            {
                boolData.Add(new InspectorBoolData(property[i].Name, false));
            }
            else if (t == typeof(AnimationCurve))
            {
                animationData.Add(new InspectorAnimationCurveData(property[i].Name, new AnimationCurve()));
            }
            else if (t == typeof(Vector3)|| t==typeof(Vector2))
            {
                vector3Data.Add(new InspectorVector3Data(property[i].Name, Vector3.zero));
            }
            else if (t == typeof(Color))
            {
                colorData.Add(new InspectorColorData(property[i].Name, Color.white));
            }
            else if (t.IsChild(typeof(Object)))
            {
                objectData.Add(new InspectorObjectData(property[i].Name, null));
            }
            else if (t.GetRoot().FullName == "Module.ViewBehaviour" && !t.IsAbstract)
            {
                objectData.Add(new InspectorObjectData(property[i].Name, null));
            }
        }
        
        

        RefreshList(stringData, Target.stringList);
        RefreshList(intData, Target.intList);
        RefreshList(floatData, Target.floatList);
        RefreshList(boolData, Target.boolList);
        RefreshList(animationData, Target.animationList);
        RefreshList(vector3Data, Target.vector3List);
        RefreshList(colorData, Target.colorList);
        RefreshList(objectData, Target.objectList);
    }
    
    private void RefreshList<T>(List<T> target,List<T> original) where T :InspectorData
    {
        for (int i = 0; i < original.Count; i++)
        {
            T temp = target.Find((s => s.key == original[i].key));
            if (temp == null)
            {
                original.RemoveAt(i);
            }
        }

        for (int i = 0; i < target.Count; i++)
        {
            T temp = original.Find((data => data.key == target[i].key));
            if (temp == null)
            {
                original.Add(target[i]);
            }
        }
    }
    
    private FieldInfo[] GetAllFiled()
    {
        return Assembly.Load("HotFix").GetTypes().Find((type => type.FullName == "HotFix." + Target.targetType)).GetFields();
    }
}