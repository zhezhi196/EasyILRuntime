using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class NodeEditorInfo : MonoBehaviour
{
    public PlayerCreator[] playerCreator;
    public MonsterCreator[] monsterCreators;
    public PropsCreator[] propsCreators;
    public string assetPath;
    [Button]
    private void EditorInit()
    {
        playerCreator = transform.GetComponentsInChildren<PlayerCreator>(true);
        monsterCreators = transform.GetComponentsInChildren<MonsterCreator>(true);
        propsCreators = transform.GetComponentsInChildren<PropsCreator>(true);
    }

    public void Save()
    {
#if UNITY_EDITOR
        PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, assetPath, InteractionMode.AutomatedAction);
        PrefabUtility.SaveAsPrefabAsset(gameObject, assetPath);
        AssetDatabase.Refresh();
#endif
    }
}