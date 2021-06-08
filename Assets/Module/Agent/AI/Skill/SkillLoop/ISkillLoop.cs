using System;

namespace Module.SkillLoop
{
    public interface ISkillLoop
    {
        float currLoop { get; }
        float totalLoop { get; }
        ISkill skill { get; }
        event Action onStart;
        event Action onEnd;
        event Action onBreak;
        void Init(ISkill owner, float loop);
        void Start();
        void End();
        void Break();
        void Pause();
        void Continue();
        void Dispose();
        void SetListener(Func<bool> listener);
    }
}