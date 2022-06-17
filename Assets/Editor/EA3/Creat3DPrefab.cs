using System;
using System.IO;
using Module;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;


public class Creat3DPrefab : OdinEditorWindow
{
    [UnityEditor.MenuItem("Tools/策划工具/创建3D预制")]
    public static void OpenWindow()
    {
        GetWindow<Creat3DPrefab>();
    }
    [ReadOnly]
    public AutoRotateModel model;
    [ReadOnly]
    public GameObject current;
    
    public string foderPath = "/Bundles/Props/Watch/";
    public string loadParent = "UIRoot/3DParent/prefab";
    [BoxGroup("调整"),LabelText("精度")]
    public float jingdu = 0.1f;
    [BoxGroup("调整")]
    public Vector3 position;
    [BoxGroup("调整")]
    public Vector3 rotation;
    [BoxGroup("调整")]
    public float scale;

    public string prefabPath
    {
        get
        {
            return "Assets/" + foderPath + current.gameObject.name + ".prefab";
        }
    }
    [Button("上一个")]
    public void Previous()
    {
        Load(-1);
    }

    [Button("下一个")]
    public void Next()
    {
        Load(1);
    }
    [Button("重新播")]
    public void ResetPlay()
    {
        if (!current.IsNullOrDestroyed())
        {
            current.transform.localEulerAngles = Vector3.zero;
        }
    }

    [Button("生成")]
    public void Creat()
    {
        if (!current.IsNullOrDestroyed())
        {
            current.transform.position = Vector3.zero;
            current.transform.eulerAngles = Vector3.zero;
            PrefabUtility.SaveAsPrefabAsset(current, prefabPath);
        }
    }
    [Button("选择物品")]
    public void Select()
    {
        Selection.activeObject = current;
    }
    private void Update()
    {
        if (!current.IsNullOrDestroyed())
        {
            current.transform.localPosition = Vector3.zero;
            current.transform.GetChild(0).localPosition = position*jingdu;
            current.transform.GetChild(0).localEulerAngles = rotation;
            current.transform.GetChild(0).localScale = Vector3.one * scale;
        }
    }


    private void Load(int next)
    {
        if (model == null)
        {
            model = GameObject.Find(loadParent).GetComponent<AutoRotateModel>();
        }

        GameObject go = model.transform.GetChild(0).gameObject;
        if (go.gameObject.name.Contains("Clone"))
        {
            go.gameObject.OnActive(false);
        }
        
        DirectoryInfo dir = Directory.CreateDirectory(Application.dataPath + foderPath);
        FileInfo[] assets = dir.GetFiles("*.prefab");
        int currentIndex = current.IsNullOrDestroyed() ? -1 : GetCurrentIndex(assets);
        int nextIndex = Mathf.Clamp(currentIndex + next, 0, currentIndex + next + 1);
        
        if (!current.IsNullOrDestroyed())
        {
            GameObject.DestroyImmediate(current);
        }
        
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/{foderPath}/{assets[nextIndex].Name}");
        current = GameObject.Instantiate(prefab, model.transform);
        current.transform.localPosition = Vector3.zero;
        current.transform.localEulerAngles = Vector3.zero;
        current.gameObject.name = assets[nextIndex].Name.Replace(".prefab", String.Empty);
        position = current.transform.GetChild(0).localPosition/jingdu;
        rotation = current.transform.GetChild(0).localEulerAngles;
        scale = current.transform.GetChild(0).localScale.x;
    }

    private int GetCurrentIndex(FileInfo[] assets)
    {
        for (int i = 0; i < assets.Length; i++)
        {
            if (assets[i].Name == current.name+".prefab")
            {
                return i;
            }
        }

        return -1;
    }
}