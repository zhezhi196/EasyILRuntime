using Sirenix.OdinInspector;

[NodeWidth(100)][NodeTint(0.3f, 0.5f, 0)][DisallowMultipleNodes()]
public class TaskStart : NodeBase
{
    [Output,HideLabel]
    public int outPort;

    private TaskNode _firstNode;

    public TaskNode firstNode
    {
        get
        {
            if (_firstNode == null)
            {
                var port = GetPort("outPort");
                if (port != null)
                {
                    var nodeList = port.GetConnections();
                    for (int j = 0; j < nodeList.Count; j++)
                    {
                        if (nodeList[j].node is TaskNode nb)
                        {
                            _firstNode = nb;
                        }
                    }
                }
            }

            return _firstNode;
        }
    }
}