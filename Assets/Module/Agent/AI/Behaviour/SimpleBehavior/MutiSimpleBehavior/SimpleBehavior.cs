using System.Collections.Generic;

namespace Module
{
    public abstract class SimpleBehavior : ISimpleBehavior
    {
        private ISimpleBehaviorObject _owner;
        public ISimpleBehavior parent { get; set; }
        public List<ISimpleBehavior> behaviors { get; }
        public ISimpleBehaviorObject owner
        {
            get { return _owner; }
        }

        public ISimpleBehavior root
        {
            get
            {
                if (parent == null) return this;
                return parent.root;
            }
        }


        public SimpleBehavior(params ISimpleBehavior[] behaviors)
        {
            if (!behaviors.IsNullOrEmpty())
            {
                this.behaviors = new List<ISimpleBehavior>(behaviors);
                for (int i = 0; i < behaviors.Length; i++)
                {
                    behaviors[i].parent = this;
                }
            }
        }

        public virtual void OnStart(ISimpleBehaviorObject owner, object[] args)
        {
            this._owner = owner;
        }

        public virtual TaskStatus OnUpdate()
        {
            return TaskStatus.Running;
        }

        public virtual void OnEnd()
        {
        }

        public virtual void OnDrawGizmos()
        {
            
        }
    }
}