namespace Module
{
    public sealed class SimpleParallel : SimpleBehavior
    {
        public override TaskStatus OnUpdate()
        {
            for (int i = 0; i < behaviors.Count; i++)
            {
                var status = behaviors[i].OnUpdate();
                if (status == TaskStatus.Failure)
                {
                    return TaskStatus.Failure;
                }
            }

            return TaskStatus.Running;
        }

    }
}