namespace Module
{
    public interface IMoveAgent: ICoroutine
    {
        bool canMove { get; }
    }
}