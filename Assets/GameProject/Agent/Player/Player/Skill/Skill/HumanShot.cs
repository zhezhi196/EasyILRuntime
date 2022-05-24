using Module;

namespace PLAYERSKILL
{
    public class HumanShot : NormalAttack
    {
        public override void OnInit(ISkillObject owner)
        {
        }

        public override AgentType agentType
        {
            get { return AgentType.Human; }
        }
    }
}