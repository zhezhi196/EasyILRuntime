using System.Collections.Generic;
using System.IO;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

public class TaskNode : NodeBase,ILocalSave
{
    [Input,HideLabel] public int inPort;
    [Output,HideLabel] public int outPort;

    public string localFileName => LocalSave.savePath;
    public string localGroup => "Task";

    public string localUid
    {
        get { return "node_" + id; }
    }


    [FoldoutGroup("Detail")]
    public int id = -1;
    [HideLabel][FoldoutGroup("Detail")]
    public NodeSetting nodeSetting;

    [HideLabel] [FoldoutGroup("Detail")] public TaskPredicate successPredicate=new TaskPredicate(){tag = "成功条件"};

    [HideLabel] [FoldoutGroup("Detail")] public TaskPredicate failPredicate = new TaskPredicate() {tag = "失败条件"};
    
    [HideInInspector]
    public NodeStation station;

    [LabelText("地图信息")]
    public string mapID;
    [ShowInInspector]
    public NodeParent parentPrefab;

    public NodeParent nodeParent;

    public Transform GetPlayerFlashPoint(int index)
    {
        if (nodeParent.playerCreator != null)
        {
            return nodeParent.playerCreator.flashPoints[index];
        }
        return null;
    }

    public NodeBase TryGetNextNode(PredicateType predicate, string[] args)
    {
        if (nextNode.IsNullOrEmpty()) return null;
        for (int i = 0; i < nextNode.Count; i++)
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

    public string GetWriteDate()
    {
        return station.ToString();
    }

    public void RunningNode()
    {
        this.station = NodeStation.Running;
    }

    public void CompleteNode()
    {
        this.station = NodeStation.Complete;
    }

    public void LockNode()
    {
        this.station = NodeStation.Locked;
    }
    
    public override bool IsFail(PredicateType predicate, string[] args)
    {
        if (predicate == PredicateType.AlwayTrue) return true;
        if (predicate == PredicateType.AlwayFalse) return false;
        for (int i = 0; i < failPredicate.enterPredicate.Count; i++)
        {
            if (failPredicate.enterPredicate[i].predicate == predicate && failPredicate.enterPredicate[i].arg.IsSame(args))
            {
                return true;
            }
        }

        return false;
    }

    public override bool IsSuccess(PredicateType predicate, string[] args)
    {
        if (predicate == PredicateType.AlwayTrue) return true;
        if (predicate == PredicateType.AlwayFalse) return false;
        
        for (int i = 0; i < successPredicate.enterPredicate.Count; i++)
        {
            if (successPredicate.enterPredicate[i].predicate == predicate && successPredicate.enterPredicate[i].arg.IsSame(args))
            {
                return true;
            }
        }

        return false;
    }
    
#if UNITY_EDITOR
    [Button]
    public void Edit()
    {
        var path = UnityEditor.AssetDatabase.GetAssetPath(this);
        path = path.Replace("Assets/", string.Empty).Replace(".asset", string.Empty);
        string fullPath = Application.dataPath + "/" + path;
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }

        if (id == -1)
        {
            MissionGraph data = UnityEditor.AssetDatabase.LoadAssetAtPath<MissionGraph>("Assets/"+path+".asset");
            for (int i = 0; i < data.nodes.Count; i++)
            {
                id = GetNodeId(data.nodes);
            }
        }

        parentPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<NodeParent>($"Assets/{path}/{name}.prefab");
        NodeParent instance = null;
        if (parentPrefab == null)
        {
            instance = new GameObject(name).AddOrGetComponent<NodeParent>();
            instance.node = this;
            instance.prefab = $"Assets/{path}/{name}.prefab";
            parentPrefab = UnityEditor.PrefabUtility.SaveAsPrefabAsset(instance.gameObject, $"Assets/{path}/{name}.prefab").GetComponent<NodeParent>();
        }
        else
        {
            var pre = (NodeParent)UnityEditor.PrefabUtility.InstantiatePrefab(parentPrefab);
            UnityEditor.PrefabUtility.UnpackPrefabInstance(pre.gameObject,UnityEditor. PrefabUnpackMode.OutermostRoot,
                UnityEditor. InteractionMode.AutomatedAction);
        }

        UnityEditor.AssetDatabase.Refresh();
    }

    private int GetNodeId(List<Node> dataNodes)
    {
        int id = 1;
        
        while (dataNodes.Contains(ed =>
        {
            if (ed is TaskNode n)
            {
                return n.id == id;
            }

            return false;
        }))
        {
            id++;
        }

        return id;
    }
    
    public override void OnRemove()
    {
        if (parentPrefab != null)
        {
            UnityEditor.AssetDatabase.DeleteAsset(UnityEditor.AssetDatabase.GetAssetPath(parentPrefab));
            UnityEditor.AssetDatabase.Refresh();
        }
    }

    [Button("读取当前雾效")]
    private void ReadFog()
    {

        nodeSetting.frog = new fogSetting[1];
        nodeSetting.frog[0] = new fogSetting();
        nodeSetting.frog[0].fadeCurve = new AnimationCurve(new Keyframe(0, 0, 2, 2), new Keyframe(1, 1, 0, 0));
        nodeSetting.frog[0].fog = RenderSettings.fog;
        nodeSetting.frog[0].fogColor = RenderSettings.fogColor;
        nodeSetting.frog[0].fogMode = RenderSettings.fogMode;
        nodeSetting.frog[0].Start = RenderSettings.fogStartDistance;
        nodeSetting.frog[0].End = RenderSettings.fogEndDistance;
        nodeSetting.frog[0].density = RenderSettings.fogDensity;
    }
#endif

}
