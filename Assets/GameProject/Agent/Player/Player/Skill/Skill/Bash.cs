using Module;
using Module.SkillAction;

namespace PLAYERSKILL
{
    /// <summary>
    /// 重击
    /// </summary>
    public class Bash: AttackSkill
    {
        public bool isHit;
        public float framePercent = 0.5f;
        public int frame = 3;
        public override string attackAnimation=> "Attack1";
        protected override void OnReleaseStart()
        {
            base.OnReleaseStart();
            isHit = false;
        }

        protected override void OnActionUpdate(ISkillAction arg1, float percent)
        {
            base.OnActionUpdate(arg1, percent);
            if (!isHit && percent >= framePercent)
            {
                isHit = true;
                //Player.player.camera.ShakeField(1, 0);
                TimeHelper.Frame(frame);
            }
        }
    }
}