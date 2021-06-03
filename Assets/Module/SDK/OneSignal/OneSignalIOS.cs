using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SDK
{
    public class OneSignalIOS : OneSignalBase
    {

#if UNITY_IOS
        // [DllImport("__Internal")]
        // private static extern

        public override void InitOneSignal()
        {
            base.InitOneSignal();
        }

        public override void PostNotification(string language, string message, string title, long delay)
        {
            base.PostNotification(language, message, title, delay);
        }

        public override void CancelNotification()
        {
            base.CancelNotification();
        }

#endif
    }
}
