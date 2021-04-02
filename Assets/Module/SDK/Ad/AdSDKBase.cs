using System;

namespace SDK
{

    public class AdSDKBase
    {
      
        public Action<E_AdState> InterstitialAdCallBack = null;
        public Action<E_AdState> RewardedVideoAdCallBack = null;

        /// <summary>
        /// 初始化广告SDK
        /// </summary>
        public virtual void InitAdSDK()
        {

        }

        /// <summary>
        /// 获取插屏是否有缓存
        /// </summary>
        /// <returns>默认静态插屏(E_InitializeAdType.Static)</returns>
        public virtual bool IsInterstitialAd(E_InitializeAdType adType)
        { 
            return true;
        }

        /// <summary>
        /// 获取激励视频是否有缓存
        /// </summary>
        /// <returns></returns>
        public virtual bool IsRewardedVideoAd()
        {
            return true;
        }

        /// <summary>
        /// 显示插屏
        /// </summary>
        /// <returns>默认静态插屏(E_InitializeAdType.Static)</returns>
        /// <param name="intersCallBack">插屏广告完成事件</param>
        /// <returns></returns>
        public virtual void PlayInterstitialAd(E_InitializeAdType adType,Action<E_AdState> intersCallBack)
        {
            if (intersCallBack != null)
            {  
                InterstitialAdCallBack = intersCallBack;
            }
        }

        /// <summary>
        /// 显示激励视频
        /// </summary>
        /// <param name="rewardedCallBack">激励广告发放奖励事件</param>
        /// <returns></returns>
        public virtual void PlayRewardedVideoAd(Action<E_AdState> rewardedCallBack)
        {
            if (rewardedCallBack != null)
            {
                RewardedVideoAdCallBack =  rewardedCallBack;
            }
            
        }

       /// <summary>
       /// 激励广告发放奖励回调
       /// </summary>
        internal void OnRewarded(E_AdState e_AdState)
        {
            if (null != RewardedVideoAdCallBack)
            {
                RewardedVideoAdCallBack(e_AdState);
                RewardedVideoAdCallBack = null;
            }
        }

        /// <summary>
        /// 广告完成回调
        /// </summary>
        internal void OnCompleted(E_AdState e_AdState)
        {
            if (InterstitialAdCallBack != null)
            {
                InterstitialAdCallBack(e_AdState);
                InterstitialAdCallBack = null;
            }
        }

        public virtual void OnAdStateChange(E_AdState state)
        {
            OnRewarded(state);
            OnCompleted(state);
            //switch (state)
            //{
            //    case E_AdState.Rewarded:
            //        //发放奖励
            //        OnRewarded(state);
            //        break;
            //    case E_AdState.DisplayFailed:
            //    case E_AdState.Completed:
            //    case E_AdState.Close:
            //        OnCompleted(state);
            //        break;
            //    default:
            //        break;
            //}
        }
    }
}

