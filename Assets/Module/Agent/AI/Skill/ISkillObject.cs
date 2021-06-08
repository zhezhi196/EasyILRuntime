using System.Collections.Generic;

namespace Module
{
    public interface ISkillObject : ITimeCtrl, ISwitchStation, IAgentObject
    {
        bool IsSkillValid(string skill);
        List<ISkill> CreatSkill();
        bool CanBreakSkill(string name);
    }
}