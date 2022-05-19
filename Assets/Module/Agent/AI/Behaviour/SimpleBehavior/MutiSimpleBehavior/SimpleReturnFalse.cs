namespace Module
{
    public sealed class SimpleReturnFalse: SimpleBehavior
    {
        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Failure;
        }
    }
}