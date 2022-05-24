using GameBuff;
using Module;
using Module.SkillAction;

namespace PLAYERSKILL
{
    /// <summary>
    /// 不屈
    /// </summary>
    public class Unyielding : BuffSkill
    {
        public override string attackAnimation => "Tiaoxin";
        public GameAttribute att;
        public override void OnInit(ISkillObject owner)
        {
            base.OnInit(owner);
            att = AttributeHelper.GetAttributeByType(skillModle.dbData)[0];
        }

        protected override void OnReleaseStart()
        {
            base.OnReleaseStart();
            BuffOption option = new BuffOption();
            option.layCount = 1;
            option.totalTime = skillModle.dbData.buffTime;
            owner.GetAgentCtrl<BuffCtrl>().AddBuff<GameAttributeBuff>(BuffType.Restart, option, att);
        }
    }
}