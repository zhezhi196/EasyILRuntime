namespace Module
{
    public sealed class SimpleSequence : SimpleBehavior
    {
        public int behaviorIndex;
        private object[] args;
        public override void OnStart(ISimpleBehaviorObject owner, object[] args)
        {
            this.args = args;
            base.OnStart(owner,args);
        }

        public ISimpleBehavior currBehavior
        {
            get { return behaviors[behaviorIndex]; }
        }
        public override TaskStatus OnUpdate()
        {
            if (behaviorIndex < behaviors.Count)
            {
                var status = currBehavior.OnUpdate();
                if (status == TaskStatus.Success)
                {
                    currBehavior.OnEnd();
                    behaviorIndex++;
                    currBehavior.OnStart(owner,args);
                    return OnUpdate();
                }
                else if (status == TaskStatus.Failure)
                {
                    return TaskStatus.Failure;
                }
                else
                {
                    return status;
                }
            }
            else
            {
                return TaskStatus.Success;
            }
        }
    }
}