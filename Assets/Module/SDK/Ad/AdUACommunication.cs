using System;
using System.Collections;
using System.Collections.Generic;
using Module;
using UnityEngine;

namespace SDK
{
    public enum E_InitializeAdType
    {
        /// <summary>
        ///  默认静态插屏
        /// </summary>
        Static = 0,
        /// <summary>
        /// 视频插屏
        /// </summary>
        Video,
        /// <summary>
        /// 场景插屏
        /// </summary>
        Scene
    }

    public enum E_AdState
    {
        /// <summary>
        /// 调用显示
        /// </summary>
        CallShow = 0,
        /// <summary>
        /// 开始显示
        /// </summary>
        Start,
        /// <summary>
        /// 真的开始显示
        /// </summary>
        StartPlay,
        /// <summary>
        /// 点击
        /// </summary>
        Click,
        /// <summary>
        /// 离开应用
        /// </summary>
        LeftApplication,
        /// <summary>
        /// 播放完成
        /// </summary>
        Completed,
        /// <summary>
        /// 发放奖励
        /// </summary>
        Rewarded,
        /// <summary>
        /// 关闭
        /// </summary>
        Close,
        /// <summary>
        /// 显示失败
        /// </summary>
        DisplayFailed,
        /// <summary>
        /// 加载失败
        /// </summary>
        LoadFailed,
        /// <summary>
        /// 加载成功
        /// </summary>
        LoadSuccess
    }

    public class AdUACommunication : MonoBehaviour
    {
        
        public void OnAdStateChange(string state)
        {
            SDKMgr.GetInstance().Log("AdUACommunication --- OnAdStateChange  state = " + state);
            E_AdState adState = (E_AdState)(Convert.ToInt32(state.Trim()));
            SDKMgr.GetInstance().Log("AdUACommunication --- OnAdStateChange  adState = " + adState);
            SDKMgr.GetInstance().MyAdSDK.OnAdStateChange(adState);
        }

        public void Back()
        {
            EventCenter.Dispatch(ConstKey.Back);
        }

    }
}

