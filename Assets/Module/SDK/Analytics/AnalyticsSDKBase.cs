using SDK;
using System.Collections;
using System.Collections.Generic;
using Module;
using UnityEngine;

public class AnalyticsSDKBase
{
    /// <summary>
    /// 初始化
    /// </summary>
   public virtual void InitAnalytics()
    {

    }
    
    public virtual void OnEvent(string eventId,E_AnalyticsType e_AnalyticsType)
    {
    }

    /// <summary>
    /// 购买事件
    /// </summary>
    /// <param name="eventId">事件标识符</param>
    /// <param name="price">价格</param>
    /// <param name="currencyType">货币类型</param>
    /// <param name="amount">数量</param>
    /// <param name="itemType">物品类型</param>
    /// <param name="itemId">物品id</param>
    /// <param name="cartType">购买位置</param>
    /// <param name="receipt">交易收据</param>
    /// <param name="store">商店(目前只能传google_play)</param>
    /// <param name="signature">交易收据签名</param>
    public virtual void OnPurchaseEvent(string eventId, float price,string currencyType, int amount, string itemType, string itemId, string cartType, string receipt, string store, string signature,E_AnalyticsType e_AnalyticsType)
    {

    }

    /// <summary>
    /// 关卡统计事件
    /// </summary>
    /// <param name="status">关卡状态 (Start(0) -- 开始  Fail(1) -- 失败  Complete(2) -- 完成)</param>
    /// <param name="chapter">章节</param>
    /// <param name="model">模式</param>
    /// <param name="stageLevel">关卡id</param>
    public virtual void OnLevelEvent(int status, string chapter, string model, string stageLevel,E_AnalyticsType e_AnalyticsType)
    {

    }

}
