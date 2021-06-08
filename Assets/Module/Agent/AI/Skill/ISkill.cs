using System;

namespace Module
{
    public interface ISkill
    {
        ISkillObject owner { get; }
        string name { get; }
        bool isActive { get; }
        void SetActive(bool active, bool withAnimation = false);
        bool Release(Action callback);
        bool Break(bool withAnimation);
        void Pause();
        void Continue();
        void Dispose();
    }
}