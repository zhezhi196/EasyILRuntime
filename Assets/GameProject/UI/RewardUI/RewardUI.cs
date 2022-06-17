using DG.Tweening;
using Module;
using UnityEngine;
using UnityEngine.UI;

public class RewardUI : UIViewBase
{
    public static void OpenRewardUI(IRewardObject reward)
    {
        UIController.Instance.Popup("RewardUI", UITweenType.None, reward);
    }
    
    public Text title;
    public Image icon;
    public Text rewardDes;
    public UIBtnBase ok;
    public CanvasGroup canvas;

    public override void OnOpenComplete()
    {
        //AudioPlay.PlayOneShot("Diamond_changes");
    }

    public override void Refresh(params object[] args)
    {
        canvas.alpha = 0;
        IRewardObject reward = (IRewardObject) args[0];
        if (reward != null)
        {
            title.text = reward.GetText(TypeList.Title);
            reward.GetIcon(TypeList.High, sp =>
            {
                icon.sprite = sp;
                icon.SetNativeSize();
                canvas.DOFade(1,0.2f).SetUpdate(true);
            });
            rewardDes.text = reward.GetText(TypeList.GetDes);
        }
    }

    protected override void OnChildStart()
    {
        ok.AddListener(OnOk);
    }

    private void OnOk()
    {
        OnExit();
    }
}