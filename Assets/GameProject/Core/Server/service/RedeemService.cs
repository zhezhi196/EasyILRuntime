using System.Collections.Generic;
using System;
using LitJson;

namespace Service
{
    public class RedeemService : Service<RedeemService>
    {
#if UNITY_IOS
        public bool CanShowCDK = false;
#else
        public bool CanShowCDK = true;
#endif
        public RedeemService()
        {
            ServiceName = "RedeemCodeKongbu2";
        }

        public void InitCDKEnable()
        {
            IsRedeemEnable(null);
        }

        public void Redeem(Dictionary<string, object> param, Action<string> OnSuccess, Action<string> OnFailed)
        {
            param = AddCommonParam(param);
            StartTopic("HD_Redeem", param, (value) =>
                {
                    if (null == value)
                    {
                        OnFailed("unknow error");
                        return;
                    }
                    string objError = (string)value[KEY_ERROR];
                    if (Convert.ToString(objError) != "")
                    {
                        OnFailed(Convert.ToString(objError));
                        return;
                    }
                    JsonData jsonResult = value[KEY_Result];
                    int errCheck = (int)jsonResult["result"];
                    string reward = (string)jsonResult["context"];
                    UnityEngine.Debug.Log(string.Format("CDK check={0} reward={1}", errCheck, reward));

                    if (errCheck == 0)
                    {
                        OnSuccess(reward);
                    }else
                    {
                        OnFailed(errCheck.ToString());
                    }
            });
        }
    
        public void IsRedeemEnable(Action<bool> cb)
        {
            JsonData param = null;
            param = AddCommonParam(param);
            ConfigService.Instance.GetConfig(E_ConfigType.RedeemEnable, (enable, data) =>
            {
                CanShowCDK = enable;
                if (cb != null)
                {
                    cb(CanShowCDK);
                }
            }, () =>
            {
                if (cb != null)
                {
                    cb(CanShowCDK);
                }
            });
        }
    }
}