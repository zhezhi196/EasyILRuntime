using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDK
{
    public class OneSignalBase
    {

        public virtual void InitOneSignal()
        {

        }

        /// <summary>
        /// 推送
        /// </summary>
        /// <param name="language">语言类型</param>
        /// <param name="message">内容</param>
        /// <param name="title">标题</param>
        /// <param name="delay">延迟时间</param>
        public virtual void PostNotification(string language,string message,string title,long delay)
        {

        }

        /// <summary>
        /// 取消通知
        /// </summary>
        public virtual void CancelNotification()
        {

        }
    }
}
