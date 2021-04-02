using System;
using Module;
using SDK;
using UnityEngine;

namespace Module
{
    public class Currency : Iap, ICurrency
    {
        public string price { get; set; }
        public Currency(IapDataBase data) : base(data)
        {
        }

        public override async void OnTryGetReward(Action<IapResult> callback, IapResult result,bool skipConsume)
        {
            base.OnTryGetReward(callback, result,skipConsume);
#if UNITY_EDITOR
            await Async.WaitforSecondsRealTime(1);
            OnSuccsee(null);
#else
            if (CanPay())
            {
                if (!skipConsume)
                {
                    AudioListener.pause = true;
                    TimeHelper.Pause();
                    SDKMgr.GetInstance().MyPaySDK.Buy(dbData.sku, "inApp", OnSuccsee, OnFail);
                }
                else
                {
                    OnSuccsee(null);
                }
            }
            else
            {
                result.result = IapResultMessage.NetworkError;
                OnFail(null);
            }
#endif
        }

        private void OnSuccsee(string obj)
        {
            getCount++;
            OnStateChanged(true);
        }
        
        private void OnFail(string obj)
        {
            OnStateChanged(false);
        }

        protected void OnStateChanged(bool success)
        {
            result.result = success ? IapResultMessage.Success : IapResultMessage.Fail;
            this.onRewardCallback?.Invoke(result);
            this.onRewardCallback = null;
            this.result = null;
            AudioListener.pause = false;
            TimeHelper.Continue();
            
        }

        public override bool CanPay()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }

    }
}