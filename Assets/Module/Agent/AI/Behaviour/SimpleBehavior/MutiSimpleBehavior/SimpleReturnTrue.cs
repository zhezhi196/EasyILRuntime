namespace Module
{
    public sealed class SimpleReturnTrue: SimpleBehavior
    {
        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
    }
}