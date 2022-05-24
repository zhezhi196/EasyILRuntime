using System.IO;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;

public enum NodeStation
{
    Locked,
    Running,
    Complete
}
public class TaskNode : NodeBase
{
    [Input,HideLabel] public int inPort;
    [Output,HideLabel] public int outPort;
    [HideLabel] public NodeEditorInfo editorPrefab;
    [HideInInspector] public NodeEditorInfo gameEditorInfo;
    [LabelText("设置")]
    public NodeSetting nodeSetting;
    [HideInInspector]
    public NodeStation station;
    
#if UNITY_EDITOR
    [Button]
    public void CreatNewInfo()
    {
        if (editorPrefab == null)
        {
            GameObject go = new GameObject(name);
            var editor = go.AddComponent<NodeEditorInfo>();
            string foderPath = $"{Application.dataPath}/Bundles/LevelEditor/{graph.name}";
            if (!Directory.Exists(foderPath))
            {
                Directory.CreateDirectory(foderPath);
            }

            editor.assetPath = $"Assets/Bundles/LevelEditor/{graph.name}/{name}.prefab";
            GameObject prefab = UnityEditor.PrefabUtility.SaveAsPrefabAsset(go, editor.assetPath);
            editorPrefab = prefab.GetComponent<NodeEditorInfo>();
            UnityEditor.AssetDatabase.Refresh();
        }
        else
        {
            var newOne = Instantiate(editorPrefab.gameObject, Vector3.zero, Quaternion.identity);
            Tools.RemoveClone(newOne);
        }
    }

    public override void OnRemove()
    {
        if (editorPrefab != null)
        {
            UnityEditor.AssetDatabase.DeleteAsset(UnityEditor.AssetDatabase.GetAssetPath(editorPrefab));
            UnityEditor.AssetDatabase.Refresh();
        }
    }
#endif
    public NodeBase TryGetNextNode(PredicateType predicate, string[] args)
    {
        if (nextNode.IsNullOrEmpty()) return null;
        for (int i = 0; i < nextNode.Length; i++)
        {
            if (nextNode[i].IsSuccess(predicate, args))
            {
                var result = nextNode[i];
                CompleteNode();
                return result;
            }
        }

        return null;
    }

    public void RunningNode()
    {
        this.station = NodeStation.Running;
    }
    public void CompleteNode()
    {
        this.station = NodeStation.Complete;
    }
}