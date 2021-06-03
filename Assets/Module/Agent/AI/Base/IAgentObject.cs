namespace Module
{
    public interface IAgentObject
    {
        T GetAgentCtrl<T>() where T : IAgentCtrl;
    }
}