using Sirenix.OdinInspector;

[NodeWidth(250)]
[NodeTint(0.3f, 0.1f, 0)]
public class TaskEnd: NodeBase
{
    [Input,HideLabel] public int inPort;
    public override NodeBase[] nextNode => null;
}