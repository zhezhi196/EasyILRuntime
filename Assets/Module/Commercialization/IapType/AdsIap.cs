using System;
using Module;
using SDK;
using UnityEngine;

namespace Module
{
    public enum AdsType
    {
        Reward,
        Interstitial
    }
    public class AdsIap : Iap
    {
        public AdsType adsType;
        public E_InitializeAdType initializeAdType;

        protected internal AdsIap(IapDataBase data, AdsType adsType,E_InitializeAdType initializeType) : base(data)
        {
            this.adsType = adsType;
            if (adsType == AdsType.Interstitial)
            {
                this.initializeAdType = initializeType;
            }
        }

        public override async void OnTryGetReward(Action<IapResult> callback, IapResult result,bool skipConsume)
        {
            base.OnTryGetReward(callback,result,skipConsume);
#if UNITY_EDITOR
            await Async.WaitforSecondsRealTime(1);
            OnStateChanged(E_AdState.Rewarded);
#else
            if (CanPay())
            {
                if (!skipConsume)
                {
                    AudioListener.pause = true;
                    TimeHelper.Pause();
                    SDK.SDKMgr.GetInstance().MyAdSDK.PlayRewardedVideoAd(OnStateChanged);
                }
                else
                {
                    OnStateChanged(E_AdState.Rewarded);
                }

            }
#endif
        }

        public override bool CanPay()
        {
            if (adsType == AdsType.Interstitial)
            {
                return SDK.SDKMgr.GetInstance().MyAdSDK.IsInterstitialAd(initializeAdType);
            }
            else if (adsType == AdsType.Reward)
            {
                return SDK.SDKMgr.GetInstance().MyAdSDK.IsRewardedVideoAd();
            }

            return false;
        }

        protected void OnStateChanged(E_AdState state)
        {
            if (IsCompleteState(state))
            {
                getCount++;
                result.result = ConvertResultMessage(state);
                this.onRewardCallback?.Invoke(result);
                this.onRewardCallback = null;
                this.result = null;
                AudioListener.pause = false;
                TimeHelper.Continue();
            }
        }
        
        private IapResultMessage ConvertResultMessage(E_AdState state)
        {
            switch (state)
            {
                case E_AdState.Rewarded:
                    return IapResultMessage.Success;
                case E_AdState.DisplayFailed:
                    return IapResultMessage.Fail;
                case E_AdState.LoadFailed:
                    return IapResultMessage.NetworkError;
                case E_AdState.LeftApplication:
                case E_AdState.Close:
                    return IapResultMessage.Cancle;
            }

            return default;
        }

        private bool IsCompleteState(E_AdState state)
        {
            return state == E_AdState.Completed || state == E_AdState.Rewarded || state == E_AdState.Close ||
                   state == E_AdState.DisplayFailed;
        }
    }
}