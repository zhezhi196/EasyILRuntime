using UnityEngine;
using UnityEditor;
using BehaviorDesigner.Runtime.Tasks;
using LitJson;
using System;
using SDK;

namespace Service
{
    public class RechargeService : Service<RechargeService>
    {
        private const string Topic_CheckPay = "HD_CheckPayArcher";

        public RechargeService()
        {
            ServiceName = "RechargeArcher2";
            TopicRetryTime.Add(GetTopic(Topic_CheckPay), 120);
        }

        private Func<JsonData, bool> mWaitCb = null;

        /// <summary>
        /// 支付校验处理
        /// </summary>
        /// <param name="isSub"></param>
        /// <param name="itemType"></param>
        /// <param name="itemId"></param>
        /// <param name="price"></param>
        /// <param name="cart_type"></param>
        /// <param name="currency"></param>
        /// <param name="receipt"></param>
        /// <param name="signature">Android是签名，ios是订单号</param>
        /// <param name="cb"></param>
        public void CheckPay(bool isSub, string orderID, string sku, int price, string payTime
            , string cart_type, string token, string signature, string extra
            , Action<bool, bool> cb)
        {
            Debug.Log(string.Format("RechargeService CheckPay[isSub={0}]", isSub));
            //Debug.Log(string.Format("RechargeService CheckPay[receipt={0}]", receipt));
            //Debug.Log(string.Format("RechargeService CheckPay[signature={0}]", signature));
            if (null == cb)
            {
                Debug.LogError("RechargeService CheckPay Error: NO CB");
                return;
            }
            //var temp = sku.Split('.');
            //sku = temp[temp.Length - 1];
            JsonData param = new JsonData();
            //863c74bf4c59cdcf55567a542463f4c2
            param["user_id"] = SystemInfo.deviceUniqueIdentifier;
            param["model"] = Platform.Instance.Model;
            //Android OS 10 / API - 29(QP1A.190711.020 / compiler05092147)
            param["os_version"] = Platform.Instance.OsVersion;
            param["manufacturer"] = SDK.Platform.Instance.Manufacturer;
            param["region"] = SDK.Platform.Instance.Region;

#if UNITY_EDITOR
            param["platform"] = "android";
#elif UNITY_ANDROID
            param["platform"] = "android";
#elif UNITY_IOS
            param["platform"] = "ios";
#endif


            Debug.Log(string.Format("RechargeService CheckPay:[user_id:{0}][{1}][{2}][{3}][{4}][{5}]"
                , Platform.Instance.UserID
                , Platform.Instance.Model
                , Platform.Instance.Model, Platform.Instance.OsVersion
                , Platform.Instance.Manufacturer, param["platform"]));

            param["isSub"] = isSub;
            param["orderId"] = orderID;
            param["sku"] = sku;
            param["amount"] = price;
            param["payTime"] = payTime;
            param["cart_type"] = cart_type;
            param["token"] = token;
            param["extra"] = extra;
#if UNITY_ANDROID
            param["signature"] = signature;
#elif UNITY_IOS
            //param["orderId"] = signature;
#endif
            param = AddCommonParam(param);
            //mWaitCb = (value) =>
            //{
            //    mWaitCb = null;
            //    if ((string)value[KEY_ERROR] != "")
            //    {
            //        string err_msg = (string)value[KEY_ERROR];
            //        Debug.LogError("HD_CheckPay Return:" + err_msg);
            //        if (err_msg.Equals("deadline exceeded"))
            //        {
            //            cb(false, false);
            //            return false;
            //        }
            //        //服务器处理错误
            //        return true;
            //    }
            //    JsonData jsonResult = value[KEY_Result];
            //    bool result = (bool)jsonResult["result"];
            //    cb(result, false);
            //    return true;
            //};
            //SubTopic("CheckPay", mWaitCb);
            //StartTopic("HD_CheckPay", param, null);

            StartTopic(Topic_CheckPay, param, (value) =>
            {
                string err_msg = (string)value[KEY_ERROR];
                if (err_msg != "")
                {
                    Debug.LogError("HD_CheckPay Return:" + err_msg);
                    if (err_msg.Equals("deadline exceeded") || err_msg.Equals("the work queue is full!"))
                    {
                        cb(false, true);
                        return;
                    }
                    else if (err_msg.Equals(ErrNetworkFailed))
                    {
                        cb(false, false);
                        return;
                    }
                    //服务器处理错误
                }
                JsonData jsonResult = value[KEY_Result];
                bool result = false;
                if (null != jsonResult)
                {
                    result = (bool)jsonResult["result"];
                }
                cb(result, false);
            });
        }
    }
}
