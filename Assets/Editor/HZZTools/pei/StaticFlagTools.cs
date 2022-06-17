using System.Collections;
using System.Collections.Generic;
using Module;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor.AI;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class StaticFlagTools : OdinEditorWindow
{

    StaticEditorFlags flags;
    private bool refeshing = false;
    static EditorWindow window;
    [MenuItem("Tools/裴亚龙专用/Static设置")]
    public static void OpenWindow()
    {
        window = GetWindow<StaticFlagTools>("Static设置");
        window.Show();
    }
    [OnValueChanged("OnFlagsChange")]
    public bool ContributeGI = false;
    [OnValueChanged("OnFlagsChange"),LabelText("遮挡物")]
    public bool OccluderStatic = false;
    [OnValueChanged("OnFlagsChange"),LabelText("被遮挡物")]
    public bool OccludeeStatic = false;
    [OnValueChanged("OnFlagsChange")]
    public bool BatchingStatic = false;
    [OnValueChanged("OnFlagsChange")]
    public bool NavigationStatic = false;
    [OnValueChanged("OnFlagsChange")]
    public bool OffMeshLinkGeneration = false;
    [OnValueChanged("OnFlagsChange")]
    public bool ReflectionProbeStatic = false;
    [OnValueChanged("OnFlagsChange")]
    public Hurtmaterial hurtMaterial;
    [Button(ButtonSizes.Large),LabelText("烘寻路"),TabGroup("烘焙")]
    private void BakeNav()
    {
        NavMeshBuilder.BuildNavMeshAsync();
    }
    
    [Button(ButtonSizes.Large),LabelText("烘遮挡剔除"),TabGroup("烘焙")]
    private void BakeIOcc()
    {
        StaticOcclusionCulling.GenerateInBackground();
    }
    [Button(ButtonSizes.Large),LabelText("设置打击物"),TabGroup("烘焙")]
    public void SetHurtMaterial()
    {
        Collider[] collider = GameObject.FindObjectsOfType<Collider>();
        for (int i = 0; i < collider.Length; i++)
        {
            var sceneLayer = collider[i].gameObject.GetComponent<SceneLayer>();
            if (sceneLayer == null)
            {
                sceneLayer = collider[i].gameObject.AddComponent<SceneLayer>();
                sceneLayer.hurtMaterial = Hurtmaterial.Stone;
            }
        }
    }
    [Button(ButtonSizes.Large),LabelText("查询未设置的层"),TabGroup("烘焙")]
    public void CheckLayer()
    {
        Transform[] all = GameObject.FindObjectsOfType<Transform>();
        List<Object> temp = new List<Object>();
        
        for (int i = 0; i < all.Length; i++)
        {
            if (all[i].gameObject.layer == 0)
            {
                Renderer render = all[i].gameObject.GetComponent<Renderer>();
                if (render != null)
                {
                    temp.Add(all[i].gameObject);
                    Debug.Log(all[i].gameObject.name, all[i].gameObject);
                }
            }
        }

        Selection.objects = temp.ToArray();
    }

    
    [Button(ButtonSizes.Large),LabelText("PlayerBlock"),TabGroup("设置")]
    private void PlayerBlock()
    {
        ContributeGI = false;
        OccluderStatic = false;
        OccludeeStatic = false;
        BatchingStatic = false;
        NavigationStatic = true;
        OffMeshLinkGeneration = true;
        ReflectionProbeStatic = false;
        OnFlagsChange(MaskLayer.playerBlock);
    }

    [Button(ButtonSizes.Large),LabelText("天花板"),TabGroup("设置")]
    private void OnGroup4()
    {
        ContributeGI = true;
        OccluderStatic = true;
        OccludeeStatic = true;
        BatchingStatic = true;
        NavigationStatic = false;
        OffMeshLinkGeneration = false;
        ReflectionProbeStatic = true;
        hurtMaterial = Hurtmaterial.Stone;
        OnFlagsChange(13);
    }


    [Button(ButtonSizes.Large), LabelText("地面"), TabGroup("设置")]
    private void OnGroup5()
    {
        ContributeGI = true;
        OccluderStatic = true;
        OccludeeStatic = true;
        BatchingStatic = true;
        NavigationStatic = true;
        OffMeshLinkGeneration = true;
        ReflectionProbeStatic = true;
        hurtMaterial = Hurtmaterial.Stone;
        OnFlagsChange(12);
    }
    
    [Button(ButtonSizes.Large), LabelText("墙"), TabGroup("设置")]
    private void OnGroup51()
    {
        ContributeGI = true;
        OccluderStatic = true;
        OccludeeStatic = true;
        BatchingStatic = true;
        NavigationStatic = true;
        OffMeshLinkGeneration = true;
        ReflectionProbeStatic = true;
        hurtMaterial = Hurtmaterial.Stone;
        OnFlagsChange(13);
    }

    [Button(ButtonSizes.Large),LabelText("地面小物件"),TabGroup("设置")]
    private void OnGroup2()
    {
        ContributeGI = true;
        OccluderStatic = false;
        OccludeeStatic = true;
        BatchingStatic = true;
        NavigationStatic = true;
        OffMeshLinkGeneration = true;
        ReflectionProbeStatic = true;
        hurtMaterial = Hurtmaterial.Stone;
        OnFlagsChange(13);
    }
    [Button(ButtonSizes.Large),LabelText("天上小物件"),TabGroup("设置")]

    private void OnGroup3()
    {
        ContributeGI = true;
        OccluderStatic = false;
        OccludeeStatic = true;
        BatchingStatic = true;
        NavigationStatic = false;
        OffMeshLinkGeneration = false;
        ReflectionProbeStatic = true;
        hurtMaterial = Hurtmaterial.Stone;
        OnFlagsChange(13);
    }

    private void OnSelectionChange()
    {
        RefreshFlags();
    }

    public void RefreshFlags()
    {
        refeshing = true;
        if (Selection.objects.IsNullOrEmpty())
        {
            flags = 0;
        }
        else
        {
            if (Selection.objects[0] is GameObject gg)
            {
                flags = GameObjectUtility.GetStaticEditorFlags(gg);
            }
        }
        ContributeGI = flags.HasFlag(StaticEditorFlags.ContributeGI);
        OccluderStatic = flags.HasFlag(StaticEditorFlags.OccluderStatic);
        BatchingStatic = flags.HasFlag(StaticEditorFlags.BatchingStatic);
        NavigationStatic = flags.HasFlag(StaticEditorFlags.NavigationStatic);
        OccludeeStatic = flags.HasFlag(StaticEditorFlags.OccludeeStatic);
        OffMeshLinkGeneration = flags.HasFlag(StaticEditorFlags.OffMeshLinkGeneration);
        ReflectionProbeStatic = flags.HasFlag(StaticEditorFlags.ReflectionProbeStatic);
        refeshing = false;
    }

    public void OnFlagsChange()
    {
        OnFlagsChange(0);
    }

    public void OnFlagsChange(int layer)
    {
        if (Selection.objects.IsNullOrEmpty()|| refeshing)
            return;
        StaticEditorFlags newFlags = 0;
        if (ContributeGI)
            newFlags = newFlags | StaticEditorFlags.ContributeGI;
        if (OccluderStatic)
            newFlags = newFlags | StaticEditorFlags.OccluderStatic;
        if (NavigationStatic)
            newFlags = newFlags | StaticEditorFlags.NavigationStatic;
        if (OccludeeStatic)
            newFlags = newFlags | StaticEditorFlags.OccludeeStatic;
        if (BatchingStatic)
            newFlags = newFlags | StaticEditorFlags.BatchingStatic;
        if (OffMeshLinkGeneration)
            newFlags = newFlags | StaticEditorFlags.OffMeshLinkGeneration;
        if (ReflectionProbeStatic)
            newFlags = newFlags | StaticEditorFlags.ReflectionProbeStatic;
        for (int j = 0; j < Selection.objects.Length; j++)
        {
            GameObject go = (Selection.objects[j] as GameObject);
            if (go != null)
            {
                Transform[] allChildren = go.GetComponentsInChildren<Transform>(true);
                for (int i = 0; i < allChildren.Length; i++)
                {
                    GameObjectUtility.SetStaticEditorFlags(allChildren[i].gameObject, newFlags);
                    allChildren[i].gameObject.layer = layer;
                }
            }
        }
        
        RefreshFlags();
        for (int i = 0; i < Selection.objects.Length; i++)
        {
            if (Selection.objects[i] is GameObject go)
            {
                Collider collider = go.GetComponent<Collider>();
                if (collider != null)
                {
                    SceneLayer sceneLayer = go.AddOrGetComponent<SceneLayer>();
                    sceneLayer.hurtMaterial = hurtMaterial;
                }
            }
        }
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
}
