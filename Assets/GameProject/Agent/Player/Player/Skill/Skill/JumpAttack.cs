using DG.Tweening;
using Module;
using Module.SkillAction;
using UnityEngine;

namespace PLAYERSKILL
{
    /// <summary>
    /// 跳斩
    /// </summary>
    public class JumpAttack : AttackSkill, IMonsterHurtObject
    {
        public float jumpTime = 1;
        public AnimationCurve curve;
        public ISkillAction jumpAction;
        public LineTrack line;
        public float pushDistance = 1.5f;
        private int bulletTimeIndex = 0;

        public override string attackAnimation => "Jump";

        protected override void OnReleaseStart()
        {
            base.OnReleaseStart();
            line = new LineTrack(Player.player.transform.position, Player.player.target.transform.position, jumpTime);
            AnimationCurve xlineCurve = AnimationCurve.Linear(0, 0, 1, 0);
            line.offset = new XYZOffset(xlineCurve, curve, xlineCurve);
            line.onComplete += JumpComplete;
            line.onUpdate += JumpUpdate;
            bulletTimeIndex = 0;
        }
        
        private void JumpUpdate(float obj)
        {
            if (obj >= 0.33f && bulletTimeIndex == 0)
            {
                TimeHelper.SetTimeScale(0.1f, 0.05f);
                bulletTimeIndex = 1;
            }
            else if (obj >= 0.45f && bulletTimeIndex == 1)
            {
                TimeHelper.ResetTimeScale();
                bulletTimeIndex = 2;
            }
        }

        private void JumpComplete()
        {
            MonsterCtrl ctrl = BattleController.GetCtrl<MonsterCtrl>();
            for (int i = 0; i < ctrl.exitMonster.Count; i++)
            {
                float dis = ctrl.exitMonster[i].transform.position.HorizonDistance(Player.player.transform.position);
                if (dis <= maxHurtDistance)
                {
                    Vector3 dir = line.endPoint - Player.player.transform.position;
                    if (dir == Vector3.zero)
                    {
                        dir = Player.player.transform.forward;
                    }
                    ctrl.exitMonster[i].Push(dir, pushDistance, null);
                    var damage = GetDamage(0, ctrl.exitMonster[i].transform.position);
                    Player.player.HurtMonster(ctrl.exitMonster[i], damage);
                    EffectPlay.Play("Zhendi", Player.player.transform.position, Vector3.zero);
                }
            }
        }
        
        protected override void OnActionUpdate(ISkillAction arg1, float percent)
        {
            Player.player.transform.position = line.UpdatePosition(Player.player.GetDelatime(false));;
        }
    }
}