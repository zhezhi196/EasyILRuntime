using System;
using System.Collections.Generic;
using Module;
using SDK;
using UnityEngine;

namespace Module
{
    public class Currency : Iap, ICurrency
    {
        private string _price;

        public string price
        {
            get
            {
#if !UNITY_EDITOR
                if (_price.IsNullOrEmpty())
                {
                    GameDebug.Log("价格没有初始化,走表价格");
                    if (Language.currentLanguage == SystemLanguage.Chinese ||Language.currentLanguage == SystemLanguage.ChineseSimplified)
                    {
                        return "￥" + dbData.rmb;
                    }
                    else
                    {
                        return "$" + dbData.usd;
                    }
                }

                return _price;
#else
                if (Language.currentLanguage == SystemLanguage.Chinese || Language.currentLanguage == SystemLanguage.ChineseSimplified)
                {
                    return "￥" + dbData.rmb;
                }
                else
                {
                    return "$" + dbData.usd;
                }
#endif
            }
            set { _price = value; }
        }

        protected Action<IapResult> onRewardCallback;

        public Currency(IapDataBase data) : base(data)
        {
        }

        public override string OnTryGetReward(Action<IapResult> callback, IapResult result, bool skipConsume)
        {
            base.OnTryGetReward(callback, result, skipConsume);
            string key = EncryptionHelper.MD5Encrypt(sku + TimeHelper.GetNow());
            onRewardCallback = callback;
            result.skipConsume = skipConsume;
            if (iapState == IapState.Normal)
            {
#if UNITY_EDITOR
                TimeHelper.isPause = true;
                AudioListener.pause = true;
                OnSuccsee(null);
#else
                if (!skipConsume)
                {
                    AudioListener.pause = true;
                    TimeHelper.Pause(this);
                    GameDebug.LogFormat("调用支付窗口");
                    SDKMgr.GetInstance().MyPaySDK.Buy(sku, key, "inApp", OnSuccsee, OnFail);
                }
                else
                {
                    GameDebug.LogFormat("跳过支付,直接支付成功");
                    OnSuccsee(null);
                }

#endif
            }
            else if (iapState == IapState.Invalid)
            {
                GameDebug.LogFormat("iapState: invalid 返回支付失败");
                result.result = IapResultMessage.Fail;
                OnFail(null);
            }
            else if (iapState == IapState.Skip)
            {
                GameDebug.LogFormat("iapState: Skip 跳过支付,直接支付成功");
                OnSuccsee(null);
            }

            return key;
        }

        private void OnSuccsee(string obj)
        {
            getCount++;
            OnStateChanged(true, obj);
        }

        private void OnFail(string obj)
        {
            OnStateChanged(false, obj);
        }

        protected void OnStateChanged(bool success, string plantResult)
        {
            result.result = success ? IapResultMessage.Success : IapResultMessage.Fail;
            if (iapState == IapState.Normal)
            {
                GameDebug.LogFormat("支付状态更改{0}: ", success);
                if (plantResult != null)
                {
                    ///          0              |1  |2   |3    |4         |5
                    /// true(成功)或false(失败)|sku|time|token|Signature|orderID</param>
                    string[] tempStr = plantResult.Split('|');
                    result.plantOrder = tempStr[5];
                }

                AudioListener.pause = false;
                TimeHelper.Continue(this);

                onResultIapBeforeCall?.Invoke(result);
                this.onRewardCallback?.Invoke(result);
                onResultIapAfterCall?.Invoke(result);
                this.onRewardCallback = null;
                this.result = null;
            }
            else
            {
                this.onRewardCallback?.Invoke(result);
                this.onRewardCallback = null;
                this.result = null;
            }
        }

        public override bool CanPay()
        {
            if (iapState == IapState.Normal)
            {
                return Application.internetReachability != NetworkReachability.NotReachable;
            }
            else if (iapState == IapState.Invalid)
            {
                return false;
            }
            else if (iapState == IapState.Skip)
            {
                return true;
            }

            return true;
        }
    }
}