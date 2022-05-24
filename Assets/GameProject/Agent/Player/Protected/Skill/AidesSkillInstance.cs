using Module;

namespace PLAYERSKILL
{
    public class AidesSkillInstance : Skill, IActiveSkill
    {
        public PlayerAideSkill skillMode;
        public float maxHurtDistance = 15;

        protected override void OnDispose()
        {
        }

        public override void OnInit(ISkillObject owner)
        {
            skillMode = PlayerAideSkill.skillList.Find(fd => fd.dbData.skillName == name);
        }

        public bool isWanted
        {
            get { return isActive && station == SkillStation.Ready; }
        }

        public virtual void UpdateTry()
        {
        }
    }
}