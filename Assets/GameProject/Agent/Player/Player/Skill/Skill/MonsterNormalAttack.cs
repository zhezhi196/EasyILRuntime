using Module;
using Module.SkillAction;

namespace PLAYERSKILL
{
    public class MonsterNormalAttack: NormalAttack
    {
        public override AgentType agentType => AgentType.Monster;

        public override void OnInit(ISkillObject owner)
        {
            PushAction(new StationAction("Attack1", 0, owner));
            PushAction(new StationAction("Attack2", 0, owner));
            PushAction(new StationAction("Attack3", 0, owner));
            PushAction(new StationAction("Attack4", 0, owner));
        }
    }
}