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
        private DateTime startTime;
        private bool fromSdk;

        public string price
        {
            get
            {
#if !UNITY_EDITOR
                if (_price.IsNullOrEmpty())
                {
                    GameDebug.Log("价格没有初始化,走表价格");
                    return "￥" + ((ICurrencyData)dbData).showPrice;
                }

                return _price;
#else
                return ((ICurrencyData)dbData).showPrice;
#endif
            }
            set { _price = value; }
        }

        protected Action<IapResult> onRewardCallback;

        public Currency(ICurrencyData data) : base(data)
        {
        }
        
        public string sku
        {
            get { return ((ICurrencyData)dbData).sku; }
        }

        public override string OnTryGetReward(Action<IapResult> callback, IapResult result, bool skipConsume)
        {
            fromSdk = false;
            base.OnTryGetReward(callback, result, skipConsume);
            string key = EncryptionHelper.MD5Encrypt(sku + TimeHelper.now);
            onRewardCallback = callback;
            result.skipConsume = skipConsume;
            AudioListener.pause = true;
            if (iapState == IapState.Normal)
            {
#if UNITY_EDITOR|| !SDK
                OnSuccsee(null);
#else
                if (!skipConsume)
                {
                    AudioListener.pause = true;
                    GameDebug.LogFormat("调用支付窗口");
                    fromSdk = true;
                    startTime = TimeHelper.now;
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
            if (fromSdk && (TimeHelper.now - startTime).TotalSeconds <= 1f)
            {
                return;
            }
            getCount++;
            OnStateChanged(true, obj);
            AudioListener.pause = false;
        }

        private void OnFail(string obj)
        {
            OnStateChanged(false, obj);
            AudioListener.pause = false;
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