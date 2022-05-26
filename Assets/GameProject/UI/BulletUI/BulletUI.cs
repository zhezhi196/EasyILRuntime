using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;
using Project.Data;
using UnityEngine.UI;
using DG.Tweening;

public class BulletUI : UIViewBase
{
    public int[] dataIDs;
    public ScrollRect itemRoot;
    private CanvasGroup canvasGroup;
    public UIBtnBase backBtn;
    private bool initComplete = false;
    protected override void OnChildStart()
    {
        canvasGroup = itemRoot.GetComponent<CanvasGroup>();
        backBtn.AddListener(OnBack);
        List<AdsData> allData = new List<AdsData>();
        for (int i = 0; i < dataIDs.Length; i++)
        {
            //DataMgr.CommonData(33006);
            allData.Add(DataMgr.Instance.GetSqlService<AdsData>().WhereID(DataMgr.CommonData(dataIDs[i]).ToInt()));
        }
        Voter voter = new Voter(allData.Count, () => {

            initComplete = true;
        });
        for (int i = 0; i < allData.Count; i++)
        {
            RewardBag rewardBag = (RewardBag)Commercialize.GetRewardBag(allData[i]);
            //SetItem(rewardBag);
            AssetLoad.LoadGameObject<BulletUIItem>("UI/BulletUI/ShopItem.prefab", itemRoot.content, (item, o) =>
            {
                item.SetItem(rewardBag);
                voter.Add();
            });
        }
        itemRoot.transform.localScale = Tools.GetScreenScale();
    }

    public async override void OnOpenStart()
    {
        await Async.WaitUntil(() => initComplete, gameObject);
        await Async.WaitForEndOfFrame(gameObject);
        canvasGroup.DOFade(1, 0.5f).SetUpdate(true).SetId(gameObject);
    }

    public override void OnCloseComplete()
    {
        canvasGroup.alpha = 0;
    }

    private void SetItem( RewardBag resultBag)
    {
        AssetLoad.LoadGameObject<BulletUIItem>("UI/BulletUI/ShopItem.prefab", itemRoot.content, (item,o) =>
         {
             item.SetItem(resultBag);
         });
    }

    private void OnBack()
    {
        OnExit();
    }

    private void OnDestroy()
    {
        Async.StopAsync(gameObject);
        DOTween.Kill(gameObject);
    }
}
