using UnityEngine;
using XNode;
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

    public TaskNode FirstNode()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i] is TaskStart start)
            {
                return start.firstNode;
            }
        }

        return null;
    }

    public void LoadEditorData()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i] is TaskNode taskNode)
            {
                taskNode.gameEditorInfo = Instantiate(taskNode.editorPrefab, Vector3.zero, Quaternion.identity, BattleController.Instance.root);
            }
        }
    }
}