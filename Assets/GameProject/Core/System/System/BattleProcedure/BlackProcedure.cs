using System;

public class BlackProcedure : BattleProcedure
{
    public BlackProcedure(Mission mission, Action callback) : base(mission, callback)
    {
        AppendCtrl(new SceneCtrl());
        AppendCtrl(new BlackPlayerCtrl());
        AppendCtrl(new PropsCtrl());
        AppendCtrl(new GiftCtrl());
        AppendCtrl(new TimelineCtrl());
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
        AddAction(BattleController.GetCtrl<BlackPlayerCtrl>().loadPlayer);
        AddAction(BattleController.GetCtrl<BlackPlayerCtrl>().bronPlayer);
        AddAction(BattleController.GetCtrl<GiftCtrl>().initGift);
        AddAction(BattleController.GetCtrl<PropsCtrl>().loadPros);
        AddAction(BattleController.GetCtrl<SceneCtrl>().LoadComplete);
        AddAction(BattleController.GetCtrl<BlackPlayerCtrl>().bronAnim);
        AddAction(BattleController.GetCtrl<BlackPlayerCtrl>().openGameUi);
    }

}