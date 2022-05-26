using System;

public class BlackProcedure : BattleProcedure
{
    public BlackProcedure(Mission mission, Action callback) : base(mission, callback)
    {
        AppendCtrl(new SceneCtrl());
        AppendCtrl(new PlayerCtrl());
        AppendCtrl(new PropsCtrl());
    }

    public override GameMode mode => GameMode.Black;
    protected override void SortResultAction(Mission mission, bool result)
    {
        AddAction(BattleController.GetCtrl<PlayerCtrl>().endAction);
    }

    protected override void SortAction()
    {
        AddAction(BattleController.GetCtrl<SceneCtrl>().loadScene);
        AddAction(BattleController.GetCtrl<SceneCtrl>().unloadScene);
        AddAction(BattleController.GetCtrl<PlayerCtrl>().loadPlayer);
        AddAction(BattleController.GetCtrl<PlayerCtrl>().bronPlayer);
        AddAction(BattleController.GetCtrl<PropsCtrl>().loadPros);
        AddAction(BattleController.GetCtrl<SceneCtrl>().LoadComplete);
        AddAction(BattleController.GetCtrl<PlayerCtrl>().bronAnim);
        AddAction(BattleController.GetCtrl<PlayerCtrl>().openGameUi);
    }

}