using Module;

public class BattleSettingUI : UIViewBase
{
    public UIBtnBase back;
    protected override void OnChildStart()
    {
        back.AddListener(OnBack);
    }

    private void OnBack()
    {
        OnExit();
    }

    public override void Refresh(params object[] args)
    {
        var list = AttributeHelper.GetAttributeInfo(Player.player.GetSkillAttribute(0), 0);
        var list1 = AttributeHelper.GetAttributeInfo(Player.player.GetSkillAttribute(1), 1);
        
    }
}