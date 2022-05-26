using System.Collections.Generic;
using Module;
using Project.Data;
using SDK;

public static class Commercialize
{
    public static AsyncLoadProcess Init(AsyncLoadProcess process)
    {
        process.IsDone = false;
        List<IapData> allData = DataMgr.Instance.GetSqlService<IapData>().tableList;
        for (int i = 0; i < allData.Count; i++)
        {
            Iap.GetIap(allData[i]);
        }

        //如果不是editor,调用sdk读取价格.表中的价格没有用了
#if !UNITY_EDITOR
        if (!Channel.isChina)
        {
            SDKMgr.GetInstance().MyPaySDK.QuerySkuDetails("inApp", str =>
            {
                GameDebug.Log("商品inapp价格字符串: " + str);
                if (!str.IsNullOrEmpty())
                {
                    string[] p = str.Split('|');
                    Iap iap = Iap.GetIapBySku(p[1]);
                    Currency currency = iap as Currency;
                    if (currency != null)
                    {
                        GameDebug.LogFormat("{0}物品价格{1}", p[1], p[0]);
                        currency.price = p[0];
                    }
                    else
                    {
                        GameDebug.LogFormat("sku: {0} 商品不是Currency", p[1]);
                    }
                }
            });

            SDKMgr.GetInstance().MyPaySDK.QuerySkuDetails("subs", str =>
            {
                GameDebug.Log("商品subs价格字符串: " + str);
                if (!str.IsNullOrEmpty())
                {
                    string[] p = str.Split('|');
                    Iap iap = Iap.GetIapBySku(p[1]);
                    Subscribe currency = iap as Subscribe;
                    if (currency != null)
                    {
                        currency.price = p[0];
                    }
                }
            });
        }

#endif
        process.SetComplete();
        return process;
    }

    public static bool HaveAds(AdsType adsType)
    {
        if (SDKMgr.GetInstance().MyAdSDK == null) return false;
        if (adsType == AdsType.Interstitial)
        {
            return SDKMgr.GetInstance().MyAdSDK.IsInterstitialAd(E_InitializeAdType.Static);
        }
        else if (adsType == AdsType.Reward)
        {
            return SDKMgr.GetInstance().MyAdSDK.IsRewardedVideoAd();
        }

        return false;
    }
    
      /// <summary>
        /// 根据IapData生成一个IapBag的列表
        /// </summary>
        /// <param name="data"></param>
        /// <param name="countChanged"></param>
      /// <returns></returns>
      public static IRewardBag GetRewardBag(ISqlData data)
      {
          if (data is IRwardData iapData)
          {
              return new RewardBag(iapData);
          }
          else if (data is JessicaData jessicData)
          {
              return new GameReward(jessicData);
          }

          return null;
      }


    /// <summary>
    /// 根据IapData生成一个IapBag的列表
    /// </summary>
    /// <param name="data"></param>
    /// <param name="countChanged"></param>
    /// <returns></returns>
    public static IRewardBag GetRewardBag(int id)
    {
        int header = DataMgr.Instance.GetDbType(id);
        if (header == 16) //商城礼包
        {
            IapData iapData = DataMgr.Instance.GetSqlService<IapData>().WhereID(id);
            return GetRewardBag(iapData);
        }
        else if (header == 30) //广告
        {
            AdsData adaData = DataMgr.Instance.GetSqlService<AdsData>().WhereID(id);
            return GetRewardBag(adaData);
        }
        else if (header == 31) //杰西卡
        {
            JessicaData jessicaData = DataMgr.Instance.GetSqlService<JessicaData>().WhereID(id);
            return GetRewardBag(jessicaData);
        }
        
        //RewardBag bag=new RewardBag(iapData,);
        return null;
    }

    public static RewardContent GetRewardContent(int id, float count)
    {
        IRewardObject rew = GetReward(id);
        return new RewardContent(rew, count);
    }

    public static IRewardObject GetReward(int id)
    {
        if (id == 0) return null;
        int spite = DataMgr.Instance.GetDbType(id);
        if (spite == 20)
        {
            return PropEntity.GetEntity(id);
        }
        else if (spite == 22)
        {
            return WeaponManager.allSkins[id];
        }
        else if (spite == 21)
        {
            return GameService.GetService(id);
        }
        else if (spite == 13)
        {
            return WeaponManager.weaponAllEntitys[id];
        }
        else if (spite == 17)
        {
            return Mission.missionList.Find(fd => fd.dbData.ID == id);
        }

        return null;
    }

    public static void OpenStore()
    {
        if (Channel.isChina)
        {
            //UIController.Instance.Open("StoreUI", UITweenType.None);
            UIController.Instance.Open("StoreUIChinese", UITweenType.None);
        }
        else
        {
            UIController.Instance.Open("StoreUI", UITweenType.None);
        }
    }

    public static void QueryPurchases()
    {
        if(Channel.isChina) return;
        SDKMgr.GetInstance().MyPaySDK.QueryPurchases("inApp", resp =>
        {
            if (!resp.IsNullOrEmpty())
            {
                string[] tempStr = resp.Split('|');
                GameDebug.LogFormat("获取到掉包: {0}", resp);
                string orderId = tempStr[0];
                string sku = tempStr[1];
                SDKMgr.GetInstance().MyPaySDK.ConsumeOrder(sku, sk =>
                {
                    Iap iap = Iap.GetIapBySku(sku);
                    var reward = (IapBag)GetRewardBag(iap.dbData.ID);
                    reward.GetReward(res =>
                        {
                            //todo 获取弹窗
                            //RewardUI.OpenRewardUI(res.reward);
                        },
                        IapRewardFlag.Free | IapRewardFlag.NoLoading | IapRewardFlag.NoUpdateDB);
                    IapSaveData saveData = DataMgr.Instance.GetSqlService<IapSaveData>()
                        .Where(item => item.sku == sku);
                    saveData.station = 1;
                    saveData.plantOrder = orderId;
                    DataMgr.Instance.GetSqlService<IapSaveData>().Update(saveData);
                });
            }
        });
    }
}
