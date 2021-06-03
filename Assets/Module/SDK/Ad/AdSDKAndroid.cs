using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDK
{
    public class AdSDKAndroid : AdSDKBase
    {
        AndroidJavaClass jc;
        AndroidJavaObject jo;

        public override void InitAdSDK()
        {
            base.InitAdSDK();
            jc = new AndroidJavaClass("com.chenguan.ad.AdManager");
            jo = jc.GetStatic<AndroidJavaObject>("Instance");
            jo.Call("InitAd");
        }

        public override bool IsInterstitialAd(E_InitializeAdType adType)
        {
            return jo.Call<bool>("InterstitialAdEnable");
        }

        public override bool IsRewardedVideoAd()
        {
            return jo.Call<bool>("RewardedVideoAdEnable");
        }

        public override void PlayInterstitialAd(E_InitializeAdType adType,Action<E_AdState> intersCallBack)
        {
            base.PlayInterstitialAd(adType,intersCallBack);
            jo.Call("ShowInterstitialAd");
        }

        public override void PlayRewardedVideoAd(Action<E_AdState> rewardedCallBack)
        {
            base.PlayRewardedVideoAd(rewardedCallBack);
            jo.Call("ShowRewardedVideoAd");
        }
    }
}
