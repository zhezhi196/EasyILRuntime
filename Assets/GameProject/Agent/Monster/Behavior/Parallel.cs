using Module;

namespace MonsterBehavior
{
    public class Parallel : ISimpleBehavior
    {
        public ISimpleBehavior parent { get; set; }
        public ISimpleBehavior root
        {
            get
            {
                if (parent == null) return this;
                return parent.root;
            }
        }
        public void OnStart(ISimpleBehaviorObject owner, object[] args)
        {
        }

        public TaskStatus OnUpdate()
        {
            return TaskStatus.Running;
        }

        public void OnEnd()
        {
        }

        public void OnDrawGizmos()
        {
            throw new System.NotImplementedException();
        }
    }
}
