using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SDK
{
    public class AdSDKIOS : AdSDKBase
    {
#if UNITY_IOS
       // [DllImport("__Internal")]
        // private static extern
        public override void InitAdSDK()
        {
            base.InitAdSDK();
        }

        public override bool IsInterstitialAd(E_InitializeAdType adType)
        {
            return base.IsInterstitialAd(adType);
        }

        public override bool IsRewardedVideoAd()
        {
            return base.IsRewardedVideoAd();
        }

        public override void PlayInterstitialAd(E_InitializeAdType adType,Action intersCallBack)
        {
            base.PlayInterstitialAd(adType,intersCallBack);
        }

        public override void PlayRewardedVideoAd(Action rewardedCallBack)
        {
            base.PlayRewardedVideoAd(rewardedCallBack);
        }

#endif
    }
}