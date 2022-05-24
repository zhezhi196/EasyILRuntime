using Module;
using UnityEngine.UI;

public class ShowSkillUI : UIViewBase
{
    public Image icon;
    public Text frontDes;
    public Text backDes;
    public UIBtnBase back;
    public Text title;
    public Text des;

    protected override void OnChildStart()
    {
        base.OnChildStart();
        back.AddListener(OnBack);
    }

    private void OnBack()
    {
        OnExit();
    }

    public override void Refresh(params object[] args)
    {
        PlayerSkill skill = (PlayerSkill) args[0];
        title.text = skill.GetText(DataType.Title);
        skill.GetIcon(DataType.Normal, sp => icon.sprite = sp);
        frontDes.text = skill.frontFragment.getFragmentCount + "/" + skill.frontFragment.targetFragment;
        backDes.gameObject.OnActive(skill.backFragment.targetFragment != 0);
        backDes.text = skill.backFragment.getFragmentCount + "/" + skill.backFragment.targetFragment;
        des.text = skill.GetText(DataType.Des);
    }
}