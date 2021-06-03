namespace Module
{
    public interface IAgentCtrl : IAgentObject
    {
        void OnCreat();
        void OnBorn();
        void OnUpdate();
        void Pause(object key);
        void Continue(object key);
        void OnAgentDead();
        void OnDestroy();
    }
}