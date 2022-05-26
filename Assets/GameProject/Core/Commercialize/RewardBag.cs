using System;
using System.Collections.Generic;
using Module;
using Project.Data;
using UnityEngine;

public class RewardBag : IapBag
{
    public static Action<RewardBag> OnGetRewardBag;
    #region 构造函数

    public int payCount
    {
        get { return LocalFileMgr.GetInt("IapPayCount" + iap.dbData.ID); }
        set { LocalFileMgr.SetInt("IapPayCount" + iap.dbData.ID, value); }
    }

    public override int stationCode
    {
        get
        {
            if (iap.dbData is IapData data)
            {
                if (data.maxPay > 0 && payCount >= data.maxPay)
                {
                    return 1;
                }
            }

            return base.stationCode;
        }
    }

    public RewardBag(IapSqlData sqlData) : base(sqlData)
    {
        string[] propID = null;
        string[] propsCount = null;
        if (sqlData is IRwardData iapData)
        {
            propID = iapData.rewardID.Split(ConstKey.Spite0);
            propsCount = iapData.rewardCount.Split(ConstKey.Spite0);
        }

        if (!propID.IsNullOrEmpty())
        {
            _content = new RewardContent[propID.Length];
            for (int i = 0; i < propID.Length; i++)
            {
                content[i] = Commercialize.GetRewardContent(propID[i].ToInt(), propsCount[i].ToLong());
            }
        }
    }

    public RewardBag(Iap iap) : base(iap)
    {
        string[] propID = null;
        string[] propsCount = null;
        if (iap.dbData is IRwardData iapData)
        {
            propID = iapData.rewardID.Split(ConstKey.Spite0);
            propsCount = iapData.rewardCount.Split(ConstKey.Spite0);
        }

        _content = new RewardContent[propID.Length];
        for (int i = 0; i < propID.Length; i++)
        {
            content[i] = Commercialize.GetRewardContent(propID[i].ToInt(), propsCount[i].ToLong());
        }
    }
    
    #endregion

    private static async void WaitClose(IapResult res, IapRewardFlag flag, string freezeId)
    {
        await Async.WaitforSecondsRealTime(30, res);

        if (!HasFlag(flag, IapRewardFlag.NoLoading))
        {
            UICommpont.UnFreezeUI(freezeId);
        }

        BattleController.Instance.Continue(freezeId);
    }

    public override void GetIcon(string type, Action<Sprite> callback)
    {
        if (iap.dbData is IRwardData adsData)
        {
            if (!adsData.icon.IsNullOrEmpty())
            {
                SpriteLoader.LoadIcon(adsData.icon, callback);
            }
            else
            {
                if (content.Length == 1)
                {
                    content[0].GetIcon(type, callback);
                }
                else
                {
                    GameDebug.LogError($"IapData{adsData.ID}奖励数量{content.Length},无法获取图标");
                }
            }
        }
    }
    

    public override int index
    {
        get
        {
            if (iap.dbData is IRwardData iapdata)
            {
                return iapdata.level;
            }

            return 0;
        }
    }

    public override void GetReward(Action<IapResult> callback, IapRewardFlag flag = (IapRewardFlag) 0)
    {
#if !UNITY_EDITOR
        if (iap is AdsIap ad && ad.CanPay() && (flag & IapRewardFlag.Free) == 0)
        {
            if (!canGetReward) return;
            canGetReward = new BoolField(false);
            WaitAds();
        }
#endif
        IapResult result = new IapResult(this);
        GameDebug.LogFormat("支付状态{0}", iapState);

        if (iapState == IapState.Normal)
        {
            string freezeId = "Iap" + iap.dbData.ID;
            //是否跳过支付
            bool skip = HasFlag(flag, IapRewardFlag.Free) || NoAdsService.CanIgnoreAds(iap);
            if (!iap.CanPay() && !skip)
            {
                GameDebug.LogFormat("无法支付,直接返回失败");
                result.result = IapResultMessage.Fail;
                callback?.Invoke(result);
                PayFailTips(this);
                return;
            }

            //有没有loading
            if (!HasFlag(flag, IapRewardFlag.NoLoading))
            {
                UICommpont.FreezeUI(freezeId, true);
            }

            //有没有pause
            if (!HasFlag(flag, IapRewardFlag.NoPause))
            {
                BattleController.Instance.Pause(freezeId);
                WaitClose(result, flag, freezeId);
            }

            if (!HasFlag(flag, IapRewardFlag.NoAnalysis))
            {
                AnalyticsEvent.SendEvent(AnalyticsType.AdsBegin, iap.dbData.ID.ToString(),false);
            }


            //获取奖励,返回一个订单id,这个是我自己生成的
            string orderId = iap.OnTryGetReward(res =>
            {
                if (res.result == IapResultMessage.Success)
                {
                    GameDebug.LogFormat("获取奖励: {0}", iap.dbData.ID);
                    GetReward(product, HasFlag(flag, IapRewardFlag.NoAudio) ? RewardFlag.NoAudio : 0);
                    if (!res.skipConsume)
                    {
                        OnGetRewardBag?.Invoke(this);
                    }
                    //有没有打点
                    if (!HasFlag(flag, IapRewardFlag.NoAnalysis))
                    {
                        AnalyticsEvent.SendEvent(AnalyticsType.AdsEnd, iap.dbData.ID.ToString(),false);
                    }

                    if (res.reward.iap is AdsIap)
                    {
                        BattleController.Instance.Save(0);
                    }

                    payCount++;
                }
                else if (res.result == IapResultMessage.Fail)
                {
                    GameDebug.LogFormat("弹出支付失败");
                    PayFailTips(this);
                }

                //需不需写入数据表
                if (res.orderId != null && !HasFlag(flag, IapRewardFlag.NoUpdateDB))
                {
                    IapSaveData saveData = DataMgr.Instance.GetSqlService<IapSaveData>().Where(item => item.orderId == res.orderId);
                    saveData.station = 1;
                    saveData.plantOrder = res.plantOrder;
                    DataMgr.Instance.GetSqlService<IapSaveData>().Update(saveData);
                }

                if (!HasFlag(flag, IapRewardFlag.NoLoading))
                {
                    UICommpont.UnFreezeUI(freezeId);
                }

                if (!HasFlag(flag, IapRewardFlag.NoPause))
                {
                    BattleController.Instance.Continue(freezeId);
                }

                callback?.Invoke(result);
                //停止30秒就强制关loading的计时
                Async.StopAsync(res);
            }, result, skip);
            //需不需要写入数据库
            if (orderId != null && !HasFlag(flag, IapRewardFlag.NoUpdateDB))
            {
                result.orderId = orderId;
                //如果是editor,直接写入数据库状态为1,因为这段代码先执行,上面那句代码后执行
                if (result.reward.iap is Currency currency)
                {
#if UNITY_EDITOR|| !SDK
                    DataMgr.Instance.GetSqlService<IapSaveData>().Insert(new IapSaveData() {orderId = orderId, iapId = iap.dbData.ID, station = 1, sku = currency.sku});
#else
                    DataMgr.Instance.GetSqlService<IapSaveData>().Insert(new IapSaveData() {orderId = orderId, iapId = iap.dbData.ID, station = 0,sku = currency.sku});
#endif
                }
                else if (result.reward.iap is AdsIap ads && !result.skipConsume)
                {
                    var service = DataMgr.Instance.GetSqlService<AdsSaveData>();
                    if (service.tableList.IsNullOrEmpty())
                    {
                        service.Insert(new AdsSaveData());
                    }
                    else
                    {
                        service.tableList[0].Count++;
                        service.Update(service.tableList[0]);
                    }
                }

            }
        }
        else if (iapState == IapState.Invalid)
        {
            result.result = IapResultMessage.Fail;
            callback?.Invoke(result);
        }
        else if (iapState == IapState.Skip)
        {
            GameDebug.LogFormat("iapState: skip 直接获取奖励");
            GetReward(product, HasFlag(flag, IapRewardFlag.NoAudio) ? RewardFlag.NoAudio : 0);
            result.result = IapResultMessage.Success;
            callback?.Invoke(result);
        }
    }

    public static BoolField canGetReward;

    private static async void WaitAds()
    {
        await Async.WaitforSecondsRealTime(15);
        canGetReward = new BoolField(true);
    }
    
    public static void PayFailTips(IapBag bag)
    {
        if (bag.iap is AdsIap)
        {
            UIController.Instance.Popup("AdsFail", UITweenType.None);
        }
        else if (bag.iap is Currency)
        {
            UIController.Instance.Popup("PayFailUI", UITweenType.None, Language.GetContent("707"));
        }
    }

    private static bool HasFlag(IapRewardFlag flag, IapRewardFlag tar)
    {
        return (flag & tar) != 0;
    }

    public override string GetText(string type)
    {
        if (iap.dbData is IRwardData adsData)
        {
            switch (type)
            {
                case TypeList.Title:
                    if (!adsData.title.IsNullOrEmpty())
                    {
                        return Language.GetContent(adsData.title);
                    }
                    else
                    {
                        if (content.Length == 1)
                        {
                            return content[0].GetText(type);
                        }
                        else
                        {
                            GameDebug.LogError($"IapData{adsData.ID}奖励数量{content.Length},无法获取文本");
                            return "文本缺失";
                        }
                    }
                    
                case TypeList.Des:
                    if (!adsData.des.IsNullOrEmpty())
                    {
                        return Language.GetContent(adsData.des);
                    }
                    else
                    {
                        if (content.Length == 1)
                        {
                            return content[0].GetText(type);
                        }
                        else
                        {
                            GameDebug.LogError($"IapData{adsData.ID}奖励数量{content.Length},无法获取文本");
                            return "文本缺失";
                        }
                    }
                case TypeList.rewardCount:
                    if (content.Length == 1)
                    {
                        return ConstKey.Cheng + content[0].finalCount.value;
                    }
                    else
                    {
                        GameDebug.LogError($"IapData{adsData.ID}奖励数量{content.Length},无法获取文本");
                        return "";
                    }
                case TypeList.GetDes:
                    if (!adsData.getDes.IsNullOrEmpty())
                    {
                        List<string> count = new List<string>();
                        for (int i = 0; i < content.Length; i++)
                        {
                            count.Add(content[i].finalCount.ToString());
                        }
                        return string.Format(Language.GetContent(adsData.getDes),count.ToArray());
                    }
                    break;
            }
        }
        if (!content.IsNullOrEmpty())
        {
            List<string> temp = new List<string>();

            for (int i = 0; i < content.Length; i++)
            {
                temp.Add(content[i].GetText(type));
            }

            return string.Join(",", temp.ToArray());
        }

        return string.Empty;
    }
}