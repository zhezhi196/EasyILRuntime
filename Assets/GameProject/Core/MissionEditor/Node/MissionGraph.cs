using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;
using Debug = UnityEngine.Debug;
using Tools = Module.Tools;

[CreateAssetMenu(menuName = "HZZ/创建关卡")]
public class MissionGraph : NodeGraph
{
    private TaskStart _statNode;

    public TaskStart startNode
    {
        get
        {
            if (_statNode == null)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (nodes[i] is TaskStart result)
                    {
                        _statNode = result;
                        break;
                    }
                }
            }

            return _statNode;
        }
    }

    public void InitNodeStation(EnterNodeType type)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i] is NodeBase nodeBase)
            {
                var port = nodeBase.GetPort("outPort");
                if (port != null)
                {
                    nodeBase.nextNode.Clear();
                    var nodeList = port.GetConnections();
                    for (int j = 0; j < nodeList.Count; j++)
                    {
                        if (nodeList[j].node is NodeBase nb)
                        {
                            if (nodeBase.nextNode == null) nodeBase.nextNode = new List<NodeBase>();
                            nodeBase.nextNode.Add(nb);
                        }
                    }
                }
            }
        }
        if (type == EnterNodeType.FromSave)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] is TaskNode node)
                {
                    string station = LocalSave.Read(node);
                    if (!station.IsNullOrEmpty())
                    {
                        node.station = station.ToEnum<NodeStation>();
                    }
                }
            }
        }
        else if (type == EnterNodeType.Restart)
        {
            //先全部上锁
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] is TaskNode node)
                {
                    node.LockNode();
                }
            }
            
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] is TaskStart node)
                {
                    for (int j = 0; j < node.nextNode.Count; j++)
                    {
                        if (node.nextNode[j] is TaskNode task)
                        {
                            task.RunningNode();
                            return;
                        }
                    }
                }
            }
        }
    }

    public TaskNode GetFirstNode()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i] is TaskStart start)
            {
                var taskNode = GetRunningNode(start.nextNode);
                if (taskNode is TaskNode tsk)
                {
                    return tsk;
                }
            }
        }

        return null;
    }

    private NodeBase GetRunningNode(List<NodeBase> startNextNode)
    {
        for (int i = 0; i < startNextNode.Count; i++)
        {
            if (startNextNode[i] is TaskNode node)
            {
                if (node.station == NodeStation.Complete)
                {
                    return GetRunningNode(node.nextNode);
                }
                else if (node.station == NodeStation.Running)
                {
                    return startNextNode[i];
                }
            }
        }

        return null;
    }

    public void LoadEditor(EnterNodeType type)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i] is TaskNode node)
            {
                if (node.parentPrefab != null)
                {
                    node.nodeParent = Instantiate(node.parentPrefab, BattleController.Instance.root);
                    IMissionEditor[] all = node.nodeParent.transform.GetComponentsInChildren<IMissionEditor>(true);
                    for (int j = 0; j < all.Length; j++)
                    {
                        all[j].OnEnterBattle(type);
                    }
                }
            }
        }
    }

    // public void LoadScene(TaskNode currNode, EnterNodeType enterType, RunTimeSequence sequence)
    // {
    //     if (enterType == EnterNodeType.Restart || enterType == EnterNodeType.NextNode)
    //     {
    //         // for (int i = 0; i < BattleController.Instance.ctrlProcedure.LoadScene.Count; i++)
    //         // {
    //         //     var index = i;
    //         //     sequence.Add(new RunTimeAction(() => GameScene.UnLoad(BattleController.Instance.ctrlProcedure.LoadScene[index].ToString(), () => sequence.NextAction())));
    //         // }
    //
    //         //BattleController.Instance.ctrlProcedure.LoadScene.Clear();
    //         LoadNodeScene(currNode, sequence);
    //     }
    //     else if (enterType == EnterNodeType.FromSave)
    //     {
    //         var loadScene = new List<SceneName>();
    //         var unloadScene = new List<SceneName>();
    //
    //         for (int i = 0; i < nodes.Count; i++)
    //         {
    //             if (nodes[i] is TaskNode node)
    //             {
    //                 if (node.station == NodeStation.Complete || node.station == NodeStation.Running)
    //                 {
    //                     loadScene.AddRange(node.nodeSetting.loadScene);
    //                     unloadScene.AddRange(node.nodeSetting.unloadScene);
    //                 }
    //             }
    //         }
    //
    //         for (int i = 0; i < unloadScene.Count; i++)
    //         {
    //             loadScene.Remove(unloadScene[i]);
    //         }
    //
    //         for (int i = 0; i < loadScene.Count; i++)
    //         {
    //             int index = i;
    //             sequence.Add(new RunTimeAction(() => GameScene.LoadAdditive(loadScene[index].ToString(), sequence.NextAction)));
    //             //BattleController.Instance.ctrlProcedure.LoadScene.Add(loadScene[i]);
    //         }
    //     }
    // }

    // private static void LoadNodeScene(TaskNode currNode, RunTimeSequence sequence)
    // {
    //     if (!currNode.nodeSetting.unloadScene.IsNullOrEmpty())
    //     {
    //         for (int i = 0; i < currNode.nodeSetting.unloadScene.Count; i++)
    //         {
    //             int index = i;
    //             sequence.Add(new RunTimeAction(() => GameScene.UnLoad(currNode.nodeSetting.unloadScene[index].ToString(), sequence.NextAction)));
    //             //BattleController.Instance.ctrlProcedure.LoadScene.Remove(currNode.nodeSetting.unloadScene[i]);
    //         }
    //     }
    //
    //     if (!currNode.nodeSetting.loadScene.IsNullOrEmpty())
    //     {
    //         for (int i = 0; i < currNode.nodeSetting.loadScene.Count; i++)
    //         {
    //             int index = i;
    //             sequence.Add(new RunTimeAction(() => GameScene.LoadAdditive(currNode.nodeSetting.loadScene[index].ToString(), sequence.NextAction)));
    //             //BattleController.Instance.ctrlProcedure.LoadScene.Add(currNode.nodeSetting.loadScene[i]);
    //         }
    //     }
    // }

    protected override void OnDestroy()
    {
        nodes.Clear();
    }

    // public void CheckProgress(int index)
    // {
    //     for (int i = 0; i < nodes.Count; i++)
    //     {
    //         if (nodes[i] is TaskNode task)
    //         {
    //             IProgressOption[] tar = task.parentPrefab.GetComponentsInChildren<IProgressOption>();
    //             for (int j = 0; j < tar.Length; j++)
    //             {
    //                 if (tar[j].progressOption.index == index)
    //                 {
    //                     Debug.Log($"{tar[j].gameObject.name}");
    //                 }
    //             }
    //         }
    //     }
    // }
#if UNITY_EDITOR
    [Button]
    public void ResetData()
    {
        string path = UnityEditor.AssetDatabase.GetAssetPath(this);

        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i] is TaskNode task)
            {
                string prefabPath =
                    $"{path.Replace(".asset", string.Empty)}/{task.name.Replace("(Clone)", string.Empty)}.prefab";
                task.parentPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<NodeParent>(prefabPath);
                task.parentPrefab.prefab = prefabPath;
            }
        }
    }
#endif
    [Button]
    public void CheckProgress()
    {
        // List<PropsCreator> result = new List<PropsCreator>();
        // for (int i = 0; i < nodes.Count; i++)
        // {
        //     if (nodes[i] is TaskNode task)
        //     {
        //         // BattleController.GetCtrl<ProgressCtrl>().curProgressIndex;
        //         // task.nodeParent.progressSO.GetCreatorListByIndex();
        //         PropsCreator[] propsCreator = task.parentPrefab.transform.GetComponentsInChildren<PropsCreator>(true);
        //         for (int j = 0; j < propsCreator.Length; j++)
        //         {
        //             if (propsCreator[j].progressOption.index != -1)
        //             {
        //                 result.Add(propsCreator[j]);
        //             }
        //         }
        //     }
        // }
        //
        // result.Sort((a, b) => a.extuil.progress.index.CompareTo(b.extuil.progress.index));
        // StringBuilder builder = new StringBuilder();
        // for (int i = 0; i < result.Count; i++)
        // {
        //     builder.Append(result[i].extuil.progress.index + " ==> " + result[i].id + "  " + result[i].model + "\n");
        // }
        //
        // using (StreamWriter writer = new StreamWriter($"{Application.persistentDataPath}/工具/progress.txt"))
        // {
        //     writer.Write(builder);
        //     Process.Start($"{Application.persistentDataPath}/工具/progress.txt");
        // }
    }
}