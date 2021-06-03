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

        /// <summary>
        /// 获取设备唯一标识码
        /// </summary>
        /// <returns></returns>
        public virtual string GetUDID()
        {
            return "";
        }

        /// <summary>
        /// 获取Android签名文件MD5值
        /// </summary>
        /// <returns></returns>
        public virtual string GetKeyStoreMD5()
        {
            return "";
        }

        /// <summary>
        /// 设置屏幕亮度
        /// </summary>
        /// <param name="brightness">屏幕亮度值(0-255)</param>
        public virtual void SetBrightness(int brightness)
        {

        }
    }
}
