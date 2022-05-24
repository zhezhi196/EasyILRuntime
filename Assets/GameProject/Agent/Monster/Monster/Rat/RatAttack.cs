using System.Collections;
using Module;
using Module.SkillAction;
using UnityEngine;

namespace GameMonster
{
    public class RatAttack : MonsterSkill, IActiveSkill
    {
        public float hurtDistance;
        public float hurtAngle;

        public override bool isReadyRelease
        {
            get { return monster.toTargetDistance <= hurtDistance && base.isReadyRelease; }
        }

        public override void OnInit(ISkillObject owner)
        {
            base.OnInit(owner);
            var temp = new StationAction("Attack1", 0, owner);
            temp.onEvent += OnEvent;
            PushAction(temp);
        }

        private void OnEvent(string name,AnimationEvent obj,int index)
        {
            if (index == 0)
            {
                if (monster.target == Player.player)
                {
                    Player.player.PreAvoid();
                }
            }
            else if (index == 1)
            {
                Player.player.OnHurt(new MonsterDamage() {damage = 1});
            }
        }

        protected override void OnReleaseStart()
        {
            monster.AddStation(MonsterStation.Attack);
        }

        protected override void OnReleaseEnd(bool complete)
        {
            monster.RemoveStation(MonsterStation.Attack);
        }

        public bool isWanted
        {
            get { return true; }
        }

        public void UpdateTry()
        {
            monster.MoveToPoint(monster.target.transform.position, null);
        }
    }
}