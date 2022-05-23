using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SDK
{
    public class CommonIOS : CommonBase
    {
#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void Apple_OpenAPPMarket();
        [DllImport("__Internal")]
        private static extern void Apple_ShowPrivacyPolicy();
        [DllImport("__Internal")]
        private static extern bool Apple_IsSDKLog();
        [DllImport("__Internal")]
        private static extern void Apple_ExitGame();
        public override void InitCommon()
        {
            base.InitCommon();
        }
        public override void HideSplash()
        {
            base.HideSplash();
        }

        public override void OpenAPPMarket()
        {
            SDKMgr.GetInstance().Log("CommonIOS --- OpenAPPMarket");
            Apple_OpenAPPMarket();
        }

        public override bool IsSDKLog()
        {
            return Apple_IsSDKLog();
        }

        public override void ShowPrivacyPolicy()
        {
            SDKMgr.GetInstance().Log("CommonIOS --- ShowPrivacyPolicy");
            Apple_ShowPrivacyPolicy();
        }

        public override void SetBrightness(int brightness)
        {
            base.SetBrightness(brightness);
        }

        public override string GetUDID()
        {
            return base.GetUDID();
        }

        public override void OpenInput(string text, Action<string> onValueChange, Action<bool> onKeybordShowChange)
        {
            base.OpenInput(text, onValueChange, onKeybordShowChange);
        }

        public override string GetKeyStoreMD5()
        {
            return base.GetKeyStoreMD5();
        }

        public override void ExitGame()
        {
            Apple_ExitGame();
        }

#endif
    }
}
