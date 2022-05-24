public class DayProcedure : BattleProcedure
{
    public DayProcedure(Mission mission) : base(mission)
    {
        battleCtrl.Add(new PlayerCtrl());
        battleCtrl.Add(new UICtrl());
        battleCtrl.Add(new SceneCtrl());
        battleCtrl.Add(new MonsterCtrl());
    }

    public override PlayMode mode => PlayMode.Day;

    protected override void SortAction()
    {
        AddAction(BattleController.GetCtrl<SceneCtrl>().loadScene);
        AddAction(BattleController.GetCtrl<SceneCtrl>().unloadScene);
        AddAction(BattleController.GetCtrl<PlayerCtrl>().creatPlayer);
        AddAction(BattleController.GetCtrl<PlayerCtrl>().creatBase);
        AddAction(BattleController.GetCtrl<PlayerCtrl>().bornAll);

        AddAction(BattleController.GetCtrl<MonsterCtrl>().loadMonster);
        AddAction(BattleController.GetCtrl<MonsterCtrl>().bornMonster);
        
        AddAction(BattleController.GetCtrl<UICtrl>().OpenGameUI);
    }

    protected override void SortResultAction(bool result)
    {
    }
}