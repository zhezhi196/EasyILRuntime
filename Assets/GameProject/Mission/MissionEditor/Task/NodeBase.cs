using System.Collections.Generic;
using XNode;

public abstract class NodeBase : Node
{
    public virtual bool IsFail(PredicateType predicate, string[] args)
    {
        switch (predicate)
        {
            case PredicateType.AlwayTrue:
                return true;
            case PredicateType.AlwayFalse:
                return false;
        }

        return false;
    }

    public virtual bool IsSuccess(PredicateType predicate, string[] args)
    {
        switch (predicate)
        {
            case PredicateType.AlwayTrue:
                return true;
            case PredicateType.AlwayFalse:
                return false;
        }

        return true;
    }
    
    private NodeBase[] _nextNode;

    public virtual NodeBase[] nextNode
    {
        get
        {
            if (_nextNode == null)
            {
                var port = GetPort("outPort");
                if (port != null)
                {
                    List<NodeBase> resu = new List<NodeBase>();
                    var nodeList = port.GetConnections();
                    for (int j = 0; j < nodeList.Count; j++)
                    {
                        resu.Add(nodeList[j].node as NodeBase);
                    }

                    _nextNode = resu.ToArray();
                }
            }

            return _nextNode;
        }
    }

}