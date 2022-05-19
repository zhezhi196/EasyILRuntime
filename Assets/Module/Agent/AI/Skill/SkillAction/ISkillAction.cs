using System;

namespace Module.SkillAction
{
    public interface ISkillAction
    {
        bool isEnd { get; }
        float percent { get; }
        void OnStart();
        void OnEnd(bool complete);
        void OnPause();
        void OnContinue();
        void Dispose();
        void OnUpdate();
    }
}