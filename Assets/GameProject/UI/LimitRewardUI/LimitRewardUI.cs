using Module;
using Project.Data;
using UnityEngine.UI;

public class LimitRewardUI : UIViewBase
{
    public UIBtnBase ok;
    public UIBtnBase back;
    public Image icon;
    public Text title;
    public Text remainTime;
    public LimitRewardIcon[] rewardIcon;
    public Text yuanjia;
    public Text price;
    public Image adsIcon;

    private RewardBag rewardBag;
    protected override void OnChildStart()
    {
        base.OnChildStart();
        ok.AddListener(OnOk);
        back.AddListener(OnBack);
        transform.GetChild(0).GetChild(0).transform.localScale = Tools.GetScreenScale();
    }

    public bool isCancle;

    private void OnBack()
    {
        isCancle = true;
        CommonPopup.Popup(Language.GetContent("701"), Language.GetContent("1902"), null,
            new PupupOption(() => { isCancle = false; }, Language.GetContent("703")),
            new PupupOption(() => { OnExit(); }, Language.GetContent("702")));
    }
    

    private void OnOk()
    {
        rewardBag.GetReward(resu =>
        {
            if (resu.result == IapResultMessage.Success)
            {
                OnExit();
                RewardUI.OpenRewardUI(rewardBag.content[0]);
            }
        });
    }

    public override void OnOpenStart()
    {
        base.OnOpenStart();
        BattleController.GetCtrl<LimitRewardCtrl>().enterUI = true;
    }

    public override void Refresh(params object[] args)
    {
        isCancle = false;
        rewardBag= (RewardBag)BattleController.GetCtrl<LimitRewardCtrl>().reward;
        
        if (rewardBag.iap is Currency currency)
        {
            //yuanjia.gameObject.OnActive(true);
            //yuanjia.text = Language.GetContent("1908") + ((IapData) (currency.dbData)).orignPrice;
            price.gameObject.OnActive(true);
            price.text = currency.price;
        }
        else
        {
            //yuanjia.gameObject.OnActive(false);
            price.gameObject.OnActive(false);
        }

        rewardBag.GetIcon(TypeList.Normal, sp =>
        {
            icon.sprite = sp;
            icon.SetNativeSize();
        });
        adsIcon.gameObject.OnActive(rewardBag.iap is AdsIap);
        title.text = rewardBag.GetText(TypeList.Title);
        for (int i = 0; i < rewardIcon.Length; i++)
        {
            if (i < rewardBag.content.Length)
            {
                rewardIcon[i].gameObject.OnActive(true);
                rewardIcon[i].SetReward(rewardBag.content[i]);
            }
            else
            {
                rewardIcon[i].gameObject.OnActive(false);
            }
        }

        if (rewardBag.stationCode == 2)
        {
            adsIcon.gameObject.OnActive(false);
            price.gameObject.OnActive(true);
            price.text = rewardBag.GetText(TypeList.connotGet);
            ok.interactable = false;
        }
        else
        {
            ok.interactable = true;
        }
    }

    protected override void Update()
    {
        base.Update();
        if (rewardBag != null)
        {
            float remin = ((ILimitReward) rewardBag).remainTime;
            if (remin >= 60)
            {
                remainTime.text = remin.ToTimeShow(Language.GetContent("1907"));
            }
            else
            {
                remainTime.text = remin.ToTimeShow(Language.GetContent("1911"));
            }
            if (isCancle)
            {
                CommonPopup.UpdateContent(remin.ToTimeShow(Language.GetContent("1902")));
            }
        }
        else
        {
            remainTime.text = 0.ToTimeShow(Language.GetContent("1911"));
        }


    }
}