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
        protected Action<IapResult> onRewardCallback;

        protected internal AdsIap(IapDataBase data, AdsType adsType,E_InitializeAdType initializeType) : base(data)
        {
            this.adsType = adsType;
            if (adsType == AdsType.Interstitial)
            {
                this.initializeAdType = initializeType;
            }
        }


        public override string OnTryGetReward(Action<IapResult> callback, IapResult result, bool skipConsume)
        {
            base.OnTryGetReward(callback, result, skipConsume);
            this.onRewardCallback = callback;
            result.skipConsume = skipConsume;
            if (iapState == IapState.Normal)
            {
#if UNITY_EDITOR
                TimeHelper.isPause = true;
                AudioListener.pause = true;
                OnStateChanged(E_AdState.Rewarded);
#else

                if (!skipConsume)
                {
                    AudioListener.pause = true;
                    TimeHelper.Pause(this);
                    GameDebug.LogFormat("调用支付窗口");
                    SDK.SDKMgr.GetInstance().MyAdSDK.PlayRewardedVideoAd(OnStateChanged);
                }
                else
                {
                    GameDebug.LogFormat("跳过支付,直接支付成功");
                    OnStateChanged(E_AdState.Rewarded);
                }
#endif
            }
            else if (iapState == IapState.Invalid)
            {
                GameDebug.LogFormat("iapState: invalid 返回支付失败");
                OnStateChanged(E_AdState.DisplayFailed);
            }
            else if (iapState == IapState.Skip)
            {
                GameDebug.LogFormat("iapState: Skip 跳过支付,直接支付成功");
                OnStateChanged(E_AdState.Rewarded);
            }

            return null;
        }

        public override bool CanPay()
        {
            if (iapState == IapState.Normal)
            {
                if (adsType == AdsType.Interstitial)
                {
                    return SDK.SDKMgr.GetInstance().MyAdSDK.IsInterstitialAd(initializeAdType);
                }
                else if (adsType == AdsType.Reward)
                {
                    return SDK.SDKMgr.GetInstance().MyAdSDK.IsRewardedVideoAd();
                }
            }
            else if (iapState == IapState.Invalid)
            {
                return false;
            }
            else if (iapState == IapState.Skip)
            {
                return true;
            }


            return false;
        }

        protected void OnStateChanged(E_AdState state)
        {
            GameDebug.LogFormat("广告状态回调: {0}" , state);

            if (IsCompleteState(state))
            {
                result.result = ConvertResultMessage(state);
                if (iapState == IapState.Normal)
                {
                    AudioListener.pause = false;
                    TimeHelper.Continue(this);
                    if (result.result == IapResultMessage.Success)
                    {
                        getCount++;
                    }
                    onResultIapBeforeCall?.Invoke(result);
                }

                this.onRewardCallback?.Invoke(result);
                if (iapState == IapState.Normal)
                {
                    onResultIapAfterCall?.Invoke(result);
                }
                this.onRewardCallback = null;
                this.result = null;
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
            }

            return default;
        }

        private bool IsCompleteState(E_AdState state)
        {
            return state == E_AdState.Rewarded || state == E_AdState.Close || state == E_AdState.DisplayFailed;
        }
    }
}