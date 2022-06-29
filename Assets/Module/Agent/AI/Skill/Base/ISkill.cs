using System;
using Module.SkillAction;

namespace Module
{
    public interface ISkill : ILogObject, IconObject, ITextObject,IRandomWeight
    {
        ISkillAction cd { get; set; }
        bool isReadyRelease { get; }
        SkillStation station { get; }
        string name { get; }
        bool isActive { get; set; }
        void Release(Action<bool> callback);
        void Pause();
        void Continue();
        bool Break(BreakSkillFlag flag);
        void OnUnload();
        bool OnUpdate();
    }
}