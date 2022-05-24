using Module;

namespace GameMonster
{
    public class Rat : Monster
    {
        public override float stopMoveDistance
        {
            get
            {
                SkillCtrl skill = GetAgentCtrl<SkillCtrl>();
                if (skill.readyRelease != null)
                {
                    return ((RatAttack) skill.readyRelease).hurtDistance;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}