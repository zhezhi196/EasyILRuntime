using System;
using System.Collections.Generic;
using Module;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
[ExecuteInEditMode]
public class PropsParent : MonoBehaviour
{
    public Vector2Int idPool;
    // public static PropNames propNameSO;
    //
    // #if UNITY_EDITOR
    // [UnityEditor.Callbacks.DidReloadScripts]
    //     private static void OnScriptsReloaded()
    //     {
    //         if (propNameSO == null)
    //         {
    //             AssetLoad.PreloadAsset<PropNames>("Props/PropNames.asset", (v) =>
    //             {
    //                 propNameSO = v.Result;
    //             });
    //         }
    //     }
    // #endif
    //
    //
    // private void Awake()
    // {
    //     OnScriptsReloaded();
    // }

#if UNITY_EDITOR    
    [Button]
    private void SetDefaultStation()
    {
        
    }
    [Button]
    private void SetUnloadUnDestroy()
    {
        PropsCreator[] creators = transform.GetComponentsInChildren<PropsCreator>(true);
        for (int i = 0; i < creators.Length; i++)
        {
            string path = $"Assets/Bundles/{PropsCreator.GetPrefabPath(creators[i].model.ToString())}";
            var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<PropsBase>(path);
            if (prefab.GetType().IsChild(typeof(InteractiveToBag)) || prefab is NormalTrigger)
            {
                GameDebug.LogError($"{prefab.GetType()} false");
                creators[i].unloadUnDestroy = false;
            }
            else
            {
                GameDebug.LogError($"{prefab.GetType()} true");
                creators[i].unloadUnDestroy = true;
            }
        }
    }
    private void Update()
    {
        transform.position = Vector3.zero;
        transform.eulerAngles = Vector3.zero;
    }


    // [HideLabel,HorizontalGroup("1",200)]
    // public PropsModelName n;
    // [Button("查询某类型道具"),HorizontalGroup("1",100)]
    // private void SearchAllPropWithType()
    // {
    //     List<GameObject> result = new List<GameObject>(); 
    //     for (int i = 0; i < transform.childCount; i++)
    //     {
    //         var creator = transform.GetChild(i).GetComponent<PropsCreator>();
    //         if (creator == null)
    //         {
    //             continue;
    //         }
    //
    //         if (creator.model == n)
    //         {
    //             result.Add(creator.gameObject);
    //         }
    //     }
    // }
    // [Button]
    // private void ResetSceneObject()
    // {
    //     PropsCreator[] creators = transform.GetComponentsInChildren<PropsCreator>();
    //     for (int i = 0; i < creators.Length; i++)
    //     {
    //         string name = creators[i].scenePath;
    //         if (!name.IsNullOrEmpty())
    //         {
    //             GameObject go = GameObject.Find(name);
    //             creators[i].transform.position = go.transform.position;
    //             creators[i].transform.rotation = go.transform.rotation;
    //             
    //         }
    //     }
    // }
#endif
}