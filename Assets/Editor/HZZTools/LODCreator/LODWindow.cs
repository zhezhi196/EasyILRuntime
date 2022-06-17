using System;
using System.Collections.Generic;
using System.IO;
using Module;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

public class LODWindow : OdinEditorWindow
{
    public static Dictionary<LODWindow.SceneItemType, SceneItemTypeInfo> flags = new Dictionary<SceneItemType, SceneItemTypeInfo>()
    {
        { SceneItemType.天花板 ,new SceneItemTypeInfo(13, Hurtmaterial.Stone,StaticEditorFlags.ContributeGI| StaticEditorFlags.OccluderStatic| StaticEditorFlags.OccludeeStatic| StaticEditorFlags.BatchingStatic| StaticEditorFlags.ReflectionProbeStatic)},
        { SceneItemType.地面 ,new SceneItemTypeInfo(12,Hurtmaterial.Stone,StaticEditorFlags.ContributeGI| StaticEditorFlags.OccluderStatic| StaticEditorFlags.OccludeeStatic| StaticEditorFlags.BatchingStatic| StaticEditorFlags.NavigationStatic| StaticEditorFlags.OffMeshLinkGeneration |StaticEditorFlags.ReflectionProbeStatic)},
        { SceneItemType.墙 ,new SceneItemTypeInfo(13,Hurtmaterial.Stone,StaticEditorFlags.ContributeGI| StaticEditorFlags.OccluderStatic| StaticEditorFlags.OccludeeStatic| StaticEditorFlags.BatchingStatic| StaticEditorFlags.NavigationStatic| StaticEditorFlags.OffMeshLinkGeneration |StaticEditorFlags.ReflectionProbeStatic)},
        { SceneItemType.地面物件 ,new SceneItemTypeInfo(12,Hurtmaterial.None,StaticEditorFlags.ContributeGI|  StaticEditorFlags.OccludeeStatic| StaticEditorFlags.BatchingStatic| StaticEditorFlags.NavigationStatic| StaticEditorFlags.OffMeshLinkGeneration |StaticEditorFlags.ReflectionProbeStatic)},
        { SceneItemType.非地面物件 ,new SceneItemTypeInfo(13,Hurtmaterial.None,StaticEditorFlags.ContributeGI|  StaticEditorFlags.OccludeeStatic| StaticEditorFlags.BatchingStatic |StaticEditorFlags.ReflectionProbeStatic)},
    };
    
    public enum SceneItemType
    {
        地面物件,
        非地面物件,
        天花板,
        地面,
        墙,
    }
    [Serializable]
    public struct SceneItemTypeInfo
    {
        [HorizontalGroup(),HideLabel]
        public int layer;
        [HorizontalGroup(),HideLabel]
        public Hurtmaterial hurtMaterial;
        [HorizontalGroup(),HideLabel]
        public StaticEditorFlags flag;

        public SceneItemTypeInfo(int lay, Hurtmaterial hurtMaterial,StaticEditorFlags flag)
        {
            this.layer = lay;
            this.hurtMaterial = hurtMaterial;
            this.flag = flag;
        }
    }
    
    [Serializable]
    public class LODRenderInfo
    {
        private int _meshCount;

        [ShowInInspector]
        public int 面数
        {
            get
            {
                if (_meshCount == 0)
                {
                    MeshFilter[] fiter = 模型.transform.GetComponentsInChildren<MeshFilter>(true);

                    for (int i = 0; i < fiter.Length; i++)
                    {
                        if (fiter[i].sharedMesh != null)
                        {
                            _meshCount += fiter[i].sharedMesh.triangles.Length;
                        }
                    }
                }

                return _meshCount;
            }

        }

        public GameObject 模型;

        public LODRenderInfo(GameObject go)
        {
            this.模型 = go;
        }


    }
    [Serializable]
    public class LODSaveData
    {
        [HorizontalGroup("内容",Width = 0.2f),HideLabel,OnValueChanged("OnSceneItemChanged")]
        public SceneItemType itemType;
        [HorizontalGroup("内容",Width = 0.2f),HideLabel]
        public Hurtmaterial material;
        [HorizontalGroup("内容"),HideLabel]
        public string prefabName;
        [HorizontalGroup("内容"),HideLabel]
        public string savePath;
        [TableList]
        public List<LODRenderInfo> fbx = new List<LODRenderInfo>();

        private void OnSceneItemChanged()
        {
            if (material == Hurtmaterial.None || material == Hurtmaterial.Stone)
            {
                material = flags[itemType].hurtMaterial;
            }
        }
        public void CreatPrefab(LODSaveData saveData,List<float> defaultSettin)
        {
            GameObject go = new GameObject(prefabName);
            LODGroup lod = go.AddComponent<LODGroup>();
            List<LOD> lods = new List<LOD>();
            for (int i = 0; i < fbx.Count; i++)
            {
                var instance = PrefabUtility.InstantiatePrefab(fbx[i].模型) as GameObject;
                instance.transform.SetParentZero(go.transform);
                LOD l = new LOD(defaultSettin[i], instance.transform.GetComponentsInChildren<Renderer>());
                lods.Add(l);
            }

            lod.SetLODs(lods.ToArray());
            string fullPath = Application.dataPath + savePath;
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            SetConfig(itemType, go.transform.GetComponentsInChildren<Transform>(true));
            PrefabUtility.SaveAsPrefabAsset(go, $"{fullPath}/{prefabName}.prefab");
            GameObject.DestroyImmediate(go);
        }

        public void SetConfig(SceneItemType type, params Transform[] go)
        {
            for (int i = 0; i < go.Length; i++)
            {
                GameObjectUtility.SetStaticEditorFlags(go[i].gameObject, flags[type].flag);
                go[i].gameObject.layer = flags[type].layer;
                MeshFilter render = go[i].GetComponent<MeshFilter>();
                if (render != null)
                {
                    MeshCollider collider = go[i].gameObject.AddOrGetComponent<MeshCollider>();
                    if (collider.sharedMesh == null)
                    {
                        collider.sharedMesh = render.sharedMesh;
                    }

                    SceneLayer layer = go[i].gameObject.AddOrGetComponent<SceneLayer>();
                    layer.hurtMaterial = flags[type].hurtMaterial;
                }
            }
        }
    }

    [MenuItem("Tools/美术工具/LOD")]
    public static void OpenWindow()
    {
        GetWindow<LODWindow>();
    }

    [TabGroup("设置")]
    public List<float> defalultSetting = new List<float>(){0.6f,0.2f,0.1f};

    [TabGroup("设置")] public Dictionary<LODWindow.SceneItemType, SceneItemTypeInfo> flagSetting = flags;
    [TabGroup("制作")][LabelText("文件夹")] public List<DefaultAsset> foder = new List<DefaultAsset>();

    [TabGroup("制作")] [LabelText("结果"), ListDrawerSettings(ShowPaging = true,NumberOfItemsPerPage = 10)]
    public List<LODSaveData> result = new List<LODSaveData>();


    [TabGroup("制作")][Button("添加文件夹")]
    public void AddFoder()
    {
        for (int i = 0; i < Selection.objects.Length; i++)
        {
            if (Selection.objects[i] is DefaultAsset asset)
            {
                foder.Add(asset);
            }
        }
    }
    [TabGroup("制作")][Button("生成信息")]
    public void CheckFoder()
    {
        foder.ClearSame();
        result.Clear();
        for (int i = 0; i < foder.Count; i++)
        {
            CheckFoder(foder[i], true);
        }
    }

    private void CheckFoder(DefaultAsset asset, bool lodLabel)
    {
        string foderPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6) +
                           AssetDatabase.GetAssetPath(asset);
        FileInfo[] files = new DirectoryInfo(foderPath).GetFiles("*.FBX");
        Queue<FileInfo> resultInfo = new Queue<FileInfo>();
        for (int i = 0; i < files.Length; i++)
        {
            if (!lodLabel || files[i].Name.ToLower().Contains("lod"))
            {
                resultInfo.Enqueue(files[i]);
            }
        }

        while (resultInfo.Count > 0)
        {
            var temp = resultInfo.Dequeue();
            string infoName = GetInfoName(temp, lodLabel);
            GameObject go = GetGo(temp);
            var info = result.Find(fd=>fd.prefabName== infoName);
            if (info != null)
            {
                info.fbx.Add(new LODRenderInfo(go));
            }
            else
            {
                LODSaveData saveData = new LODSaveData();
                saveData.fbx = new List<LODRenderInfo>() { new LODRenderInfo(go) };
                saveData.prefabName = infoName;
                saveData.savePath = GetSavePath(infoName, temp);
                result.Add(saveData);
            }
        }

        if (lodLabel && result.Count == 0)
        {
            if (EditorUtility.DisplayDialog("提示", "未发现LOD模型,是否忽略LOD标示再次检查", "检查", "取消"))
            {
                CheckFoder(asset, false);
            }
        }
    }

    private string GetSavePath(string name, FileInfo temp)
    {
        return temp.DirectoryName.Substring(Application.dataPath.Length,
            temp.DirectoryName.Length - Application.dataPath.Length).Replace("\\","/")+"/LOD";
    }

    private GameObject GetGo(FileInfo temp)
    {
        string prefabPath = Pathelper.GetAssetPath(temp.FullName);
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        return prefab;
    }

    private string GetInfoName(FileInfo fileInfo,bool lodLabel)
    {
        if (lodLabel)
        {
            int startIndex = fileInfo.Name.ToLower().IndexOf("lod");
            string name = fileInfo.Name.Substring(0, startIndex);
            return name.Trim();
        }
        else
        {
            return fileInfo.Name.Substring(0, fileInfo.Name.Length - 4);
        }
    }
    
    [TabGroup("制作")][Button("制作LOD",ButtonSizes.Large),GUIColor(0,1,0)]
    public void CreatLOD()
    {
        // LODGroup ggg = Selection.activeGameObject.GetComponent<LODGroup>();
        // var gggg = ggg.GetLODs();
        // foreach (var VARIABLE in gggg)
        // {
        //     GameDebug.LogError(VARIABLE.screenRelativeTransitionHeight);
        //     GameDebug.LogError(VARIABLE.renderers);
        //     
        // }
        // return;
        for (int i = 0; i < result.Count; i++)
        {
            result[i].CreatPrefab(result[i],defalultSetting);
        }
    }
}