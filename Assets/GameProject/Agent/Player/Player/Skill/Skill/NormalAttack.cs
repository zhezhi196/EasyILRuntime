using System.Collections.Generic;
using Module;
using Module.SkillAction;

namespace PLAYERSKILL
{
    public abstract class NormalAttack : PlayerAction, IActiveSkill
    {
        public float maxTriggerDistance;
        public float attackAngler;

        public bool isWanted
        {
            get { return Player.player.toTargetDistance <= maxTriggerDistance && base.isWanted; }
        }

        protected override void OnReleaseStart()
        {
            base.OnReleaseStart();
            Player.player.AddStation(PlayerStation.Attack);
        }

        protected override void OnReleaseEnd(bool complete)
        {
            base.OnReleaseEnd(complete);
            Player.player.RemoveStation(PlayerStation.Attack);
        }

        protected override void OnDispose()
        {
            
        }
    }
}