using System;
using System.Collections.Generic;
using System.Linq;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;


public partial class PropsCreator
{
    
    [Space]
    [FoldoutGroup("事件相关")] [LabelText("事件发送")]
    public List<EventSenderEditor> eventSenders;

    [FoldoutGroup("事件相关")] [LabelText("事件接受")] 
    public List<EventReciverEditor> eventRecivers;

#if UNITY_EDITOR

    private void OnSceneObjectChanged()
    {
        if (sceneObjectLabel != null)
        {
            sceneObject.Add(sceneObjectLabel.transform.GetPathInScene());
            sceneObjectLabel = null;
        }
    }

    private MeshFilter[] skin;


    [Button("重新刷新editor")]
    public void OnModelChanged()
    {
        // className = PropsParent.propNameSO.nameList.Find(v => v.chineseName == modelName).className;
        gameObject.name = $"{id} {(isTrigger ? "Normaltrigger" : model.ToString())}";

        if (!staticLoad)
        {
            skin = null;
        }
        else
        {
            EditorDestroyProps();
            OnLoadTypeChanged();
        }
        
        //刷新Progress
        GetComponentInParent<NodeParent>().progressSO.ChangeCreatorId(idBackUp , id);
        idBackUp = id;
    }

    // private static IEnumerable<string> GetModelNames()
    // {
    //     var ret = new List<string>();
    //     for (int i = 0; i < PropsParent.propNameSO.nameList.Count; i++)
    //     {
    //         ret.Add(PropsParent.propNameSO.nameList[i].chineseName);
    //     }
    //     return ret;
    // }

    // private void CheckProgress()
    // {
    //     NodeParent parent = transform.GetComponentInParent<NodeParent>();
    //     string[] spite = parent.prefab.Split('/');
    //     string path = parent.prefab.Substring(0, parent.prefab.Length - spite.Last().Length - 1);
    //     MissionGraph graph = UnityEditor.AssetDatabase.LoadAssetAtPath<MissionGraph>(path + ".asset");
    //     graph.CheckProgress(extuil.progress.index);
    // }

    private void OnLoadTypeChanged()
    {
        if (staticLoad)
        {
            var pre = UnityEditor.AssetDatabase.LoadMainAssetAtPath(
                $"Assets/{ConstKey.GetFolder(AssetLoad.AssetFolderType.Bundle)}/{GetPrefabPath(model.ToString())}");
            props = (UnityEditor.PrefabUtility.InstantiatePrefab(pre) as GameObject)?.GetComponent<PropsBase>();
            if (props != null)
            {
                props.transform.SetParentZero(transform);
                props.creator = this;
                props.gameObject.layer = LayerMask.NameToLayer("UnVisiableProps");
            }
        }
        else
        {
            if (props != null)
            {
                EditorDestroyProps();
            }
        }
    }

    private void EditorDestroyProps()
    {
        transform.ClearChildImmediate();
    }


    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            if (!staticLoad)
            {
                if (props == null)
                {
                    Gizmos.color = Color.yellow;
                    if (!isTrigger)
                    {
                        if (skin.IsNullOrEmpty())
                        {
                            var temp = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(
                                $"Assets/{ConstKey.GetFolder(AssetLoad.AssetFolderType.Bundle)}/Props/Scene/{model}.prefab");
                            if (temp != null)
                            {
                                skin = temp.GetComponentsInChildren<MeshFilter>(true);
                            }
                        }

                        if (!skin.IsNullOrEmpty())
                        {
                            for (int i = 0; i < skin.Length; i++)
                            {
                                Gizmos.DrawMesh(skin[i].sharedMesh, transform.position,
                                    transform.rotation * skin[i].gameObject.transform.rotation);
                            }
                        }
                    }
                }
            }
        }
    }
    
    
#endif
}