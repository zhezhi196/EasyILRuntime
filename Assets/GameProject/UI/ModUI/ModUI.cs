using DG.Tweening;
using Module;
using UnityEngine;
using UnityEngine.UI;

public class ModUI : UIViewBase
{
    public Text des;
    public UIBtnBase quit;
    public UIBtnBase copy;
    public Text tips;
    public AnimationCurve curve;

    protected override void OnChildStart()
    {
        quit.AddListener(OnQUit);
        copy.AddListener(OnCopy);
    }

    private void OnCopy()
    {
        SDK.SDKMgr.GetInstance().MyCommon.CopyData(model.args[0].ToString());
        tips.DOFade(1,3).SetUpdate(true).SetEase(curve);
    }

    private void OnQUit()
    {
        Application.Quit();
    }

    public override void Refresh(params object[] args)
    {
        tips.SetAlpha(0);
        des.text = string.Format(Language.GetContent("738"), args[0]);
    }
}