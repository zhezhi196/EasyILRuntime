namespace Module.SkillAction
{
    public class EmptyAction : ISkillAction
    {
        public bool isEnd => true;
        public float percent => 1;

        public void OnStart()
        {
        }

        public void OnEnd(bool complete)
        {
        }

        public void OnPause()
        {
        }

        public void OnContinue()
        {
        }

        public void Dispose()
        {
        }

        public void OnUpdate()
        {
        }
    }
}