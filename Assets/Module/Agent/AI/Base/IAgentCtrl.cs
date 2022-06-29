namespace Module
{
    public interface IAgentCtrl
    {
        bool isPause { get; }
        bool OnUpdate();
        void Pause();
        void Continue();
        void OnAgentDead();
        void OnDestroy();
        T GetAgentCtrl<T>() where T : IAgentCtrl;

        void EditorInit();
        void OnDrawGizmos();
    }
}