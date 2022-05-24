using Module;
using Module.SkillAction;

namespace PLAYERSKILL
{
    public class BaseNormalAttack : AidesSkillInstance
    {
        protected override void OnDispose()
        {
        }

        public override void OnInit(ISkillObject owner)
        {
            //PushAction(new AnimationAction("atk03", 0, owner));
        }
    }
}