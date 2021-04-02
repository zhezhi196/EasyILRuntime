using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDK
{
    public class CommonBase
    {

        public virtual void InitCommon() { }

        /// <summary>
        /// 隐藏Android闪屏
        /// </summary>
        public virtual void HideSplash()
        {

        }

        /// <summary>
        /// 是否开始日志
        /// </summary>
        /// <returns></returns>
        public virtual bool IsSDKLog() { return true; }

        /// <summary>
        /// 评价
        /// </summary>
        public virtual void OpenAPPMarket()
        {

        }

        /// <summary>
        /// 隐私政策
        /// </summary>
        public virtual void ShowPrivacyPolicy()
        {

        }
    }
}
