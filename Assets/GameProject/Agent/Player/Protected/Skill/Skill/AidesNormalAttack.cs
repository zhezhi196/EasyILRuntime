using Module;
using Module.SkillAction;

namespace PLAYERSKILL
{
    public class AidesNormalAttack : AidesSkillInstance
    {
        private Aides _aides;

        public override bool isReadyRelease
        {
            get
            {
                return _aides.toTargetAnger <= 5 && _aides.toTargetDistance <= maxHurtDistance && base.isReadyRelease;
            }
        }

        public override void OnInit(ISkillObject owner)
        {
            base.OnInit(owner);
            _aides = owner as Aides;
            PushAction(new StationAction("Attack1", 0, owner));
        }

        protected override void OnReleaseStart()
        {
            base.OnReleaseStart();
            _aides.AddStation(SonStation.Attack);
        }

        protected override void OnReleaseEnd(bool complete)
        {
            base.OnReleaseEnd(complete);
            _aides.RemoveStation(SonStation.Attack);
 
        }

        public override void UpdateTry()
        {
            _aides.MoveToPoint(_aides.target.transform.position);
        }
        protected override void OnDispose()
        {
        }
    }
}