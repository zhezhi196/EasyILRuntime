namespace Module
{
    public enum TaskStatus
    {
        Inactive,
        Failure,
        Success,
        Running,
    }
    public interface ISimpleBehavior
    {
        ISimpleBehavior parent { get; set; }
        ISimpleBehavior root { get; }
        void OnStart(ISimpleBehaviorObject owner, object[] args);
        TaskStatus OnUpdate();
        void OnEnd();
        void OnDrawGizmos();
    }
}