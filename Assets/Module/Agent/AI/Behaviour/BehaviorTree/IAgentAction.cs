namespace Module
{
    public interface IAgentAction: ILogObject
    {
        object GetArgs(int index);
    }
}