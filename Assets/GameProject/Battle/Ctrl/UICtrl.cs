using Module;

public class UICtrl: BattleSystem
{
    public RunTimeAction OpenGameUI;
    public override void BattlePrepare()
    {
        UIController.Instance.ClearStack(true, true);
    }

    public override void StartBattle(Mission mission, MissionGraph editorGraph)
    {
        OpenGameUI = new RunTimeAction(() =>
        {
            UIController.Instance.Open("GameUI", UITweenType.None);
            BattleController.Instance.NextFinishAction("OpenGameUI");
            OpenGameUI = null;
        });
    }
}