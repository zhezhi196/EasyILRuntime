using Module;

namespace SimBehavior
{
    /// <summary>
    /// 玩家狂暴的行为树
    /// </summary>
    public class Rampage : SimpleBehavior
    {
        public override void OnStart(ISimpleBehaviorObject owner, object[] args)
        {
            base.OnStart(owner, args);
            Player.player.AddStation(PlayerStation.Rampage);
        }

        public override TaskStatus OnUpdate()
        {
            ITarget tempTarget = Player.baseMent;
            float distance = Player.baseMent.toPlayerDistance;
            MonsterCtrl monsterCtrl = BattleController.GetCtrl<MonsterCtrl>();
            for (int i = 0; i < monsterCtrl.exitMonster.Count; i++)
            {
                var tempMonster = monsterCtrl.exitMonster[i];
                if (tempMonster.isVisiable)
                {
                    float tempDis = tempMonster.transform.position.Distance(Player.player.transform.position);
                    if (tempDis < distance)
                    {
                        distance = tempDis;
                        tempTarget = tempMonster;
                    }
                }
            }

            if (tempTarget != null)
            {
                Player.player.SwitchTarget(tempTarget);
                Player.player.MoveToPoint(tempTarget.transform.position);
            }
            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            Player.player.RemoveStation(PlayerStation.Rampage);
        }
    }
}