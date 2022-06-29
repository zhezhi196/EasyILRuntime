namespace Module
{
    public class SimpleBehaviorCtrl: IAgentCtrl
    {
        private bool _isPause;
        public ISimpleBehaviorObject owner { get; }
        public bool isPause => _isPause;
        
        public ISimpleBehavior currBehavior;


        public SimpleBehaviorCtrl(ISimpleBehaviorObject owner)
        {
            this.owner = owner;
        }

        public void SwitchBehavior(ISimpleBehavior behbavior, params object[] args)
        {
            if (currBehavior != null)
            {
                currBehavior.OnEnd();
            }

            currBehavior = behbavior;
            if (currBehavior != null)
            {
                currBehavior.OnStart(owner, args);
            }

            owner.LogFormat("切换行为树{0}", behbavior);
        }


        public bool OnUpdate()
        {
            if (currBehavior != null && !isPause)
            {
                var status = currBehavior.OnUpdate();
                
                if (status == TaskStatus.Success || status == TaskStatus.Failure)
                {
                    currBehavior.OnEnd();
                    currBehavior = null;
                }
            }

            return true;
        }

        public void Pause()
        {
            _isPause = true;
        }

        public void Continue()
        {
            _isPause = false;
        }

        public void OnAgentDead()
        {
            currBehavior = null;
        }

        public void OnDestroy()
        {
        }

        public T GetAgentCtrl<T>() where T : IAgentCtrl
        {
            return owner.GetAgentCtrl<T>();
        }

        public void EditorInit()
        {
        }

        public void OnDrawGizmos()
        {
            if (currBehavior != null)
            {
                currBehavior.OnDrawGizmos();
            }
        }
    }
}