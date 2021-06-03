namespace Module
{
    public interface IAgentAction
    {
        T GetArgs<T>(int index);
    }
}