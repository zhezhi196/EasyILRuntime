using Module;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectUI
{
    public class DailyShopItem : ShopItem
    {
        public GameObject adsBuy;
        public Text remainTime;
        public bool isGet
        {
            get { return LocalFileMgr.GetBool("ShopItemDaily" + reward.iap.dbData.ID); }
            set { LocalFileMgr.SetBool("ShopItemDaily" + reward.iap.dbData.ID, value); }
        }

        public override void SetItem(RewardBag reward)
        {
            base.SetItem(reward);
            if (TimeHelper.IsNewDay(reward.iap.dbData.ID.ToString()))
            {
                isGet = false;
            }

            bool get = isGet;
            interactable = !get;
            remainTime.transform.parent.gameObject.OnActive(get);
            adsBuy.OnActive(!get);
        }
        
        protected override void OnChildUpdate()
        {
            if (remainTime.gameObject.activeInHierarchy)
            {
                remainTime.text = string.Format(Language.GetContent("1507"), TimeHelper.remainTomorrow.Hours,
                    TimeHelper.remainTomorrow.Minutes, TimeHelper.remainTomorrow.Seconds);
            }
        }
        
        protected override void DefaultListener()
        {
            reward.GetReward(res =>
            {
                if (res.result == IapResultMessage.Success)
                {
                    isGet = true;
                    RewardUI.OpenRewardUI(reward);
                    SetItem(reward);
                }
            });
        }


    }
}