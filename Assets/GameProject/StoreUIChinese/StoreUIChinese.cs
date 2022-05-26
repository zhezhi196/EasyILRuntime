using System.Collections.Generic;
using Module;
using Project.Data;
using UnityEngine;

public class StoreUIChinese : UIViewBase
{
    public UIBtnBase back;
    public Transform content;
    public ShopItemChinese[] itemList;
    public Transform title;
    
    protected override void OnChildStart()
    {
        back.AddListener(OnBack);
        title.localScale = Tools.GetScreenScale();
    }

    private void OnBack()
    {
        OnExit();
    }
    public override void Refresh(params object[] args)
    {
        List<AdsData> adsData = DataMgr.Instance.GetSqlService<AdsData>().WhereList(fd => fd.adsArg1 == "1"||fd.adsArg1 == "2");
        adsData.Sort((a, b) => a.level.CompareTo(b.level));
        for (int i = 0; i < itemList.Length; i++)
        {
            if (adsData[i] != null)
            {
                int index = i;
                var it = adsData[index];
                RewardBag rewardBag = (RewardBag) Commercialize.GetRewardBag(it);
                itemList[index].gameObject.OnActive(true);
                itemList[index].SetItem(rewardBag);
            }
            
        }
    }
}