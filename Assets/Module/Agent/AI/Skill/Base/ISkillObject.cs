using System.Collections.Generic;

namespace Module
{
    public interface ISkillObject : IAgentObject
    {
        bool canReleaseSkill { get; }
        IActiveSkill RefreshReadySkill();
        void OnReleasingSkill(ISkill skill);
    }
}