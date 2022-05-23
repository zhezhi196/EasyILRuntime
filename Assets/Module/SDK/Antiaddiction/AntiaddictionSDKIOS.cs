using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SDK
{
    public class AntiaddictionSDKIOS : AntiaddictionSDKBase
    {
#if UNITY_IOS
        #region IOS桥接方法
        [DllImport("__Internal")]
        private static extern void Apple_Init();
        [DllImport("__Internal")]
        private static extern void Apple_ShowRealNameDialog();
        [DllImport("__Internal")]
        private static extern void Apple_LogOut();
        [DllImport("__Internal")]
        private static extern bool Apple_IsRegistered();
        [DllImport("__Internal")]
        private static extern void Apple_CheckAbled();
        [DllImport("__Internal")]
        private static extern bool Apple_CanPay(int money);
        [DllImport("__Internal")]
        private static extern void Apple_Pay(int money);
        [DllImport("__Internal")]
        private static extern int Apple_MaxAmount();
        [DllImport("__Internal")]
        private static extern int Apple_SignleAmount();
        [DllImport("__Internal")]
        private static extern int Apple_LeftAmount();
        [DllImport("__Internal")]
        private static extern int Apple_MaxTime();
        [DllImport("__Internal")]
        private static extern int Apple_LeftTime();
        [DllImport("__Internal")]
        private static extern int Apple_GetUserType();
        [DllImport("__Internal")]
        private static extern int Apple_GetLastTimeUserType();
        #endregion

        public override void Init()
        {
            Apple_Init();

        }

        public override void ShowRealNameDialog(Action<string> loginState)
        {
            base.ShowRealNameDialog(loginState);
            Apple_ShowRealNameDialog();
        }

        public override void LoginOut()
        {
            Apple_LogOut();
        }

        public override bool IsRegistered()
        {
            return Apple_IsRegistered();
        }

        public override void CheckAbled()
        {
            Apple_CheckAbled();
        }

        public override bool CanPay(int money)
        {
            return Apple_CanPay(money);
        }

        public override void Pay(int money)
        {
            Apple_Pay(money);
        }

        public override int MaxAmount()
        {
            return Apple_MaxAmount();
        }

        public override int SignleAmount()
        {
            return Apple_SignleAmount();
        }
        public override int LeftAmount()
        {
            return Apple_LeftAmount();
        }

        public override int MaxTime()
        {
            return Apple_MaxTime();
        }

        public override int LeftTime()
        {
            return Apple_LeftTime();
        }

        public override int GetUserType()
        {
            return Apple_GetUserType();
        }

        public override int GetLastTimeUserType()
        {
            return Apple_GetLastTimeUserType();
        }
#endif
    }
}
