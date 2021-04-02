using System;
using System.Runtime.InteropServices;

namespace SDK
{
    public class PaySDKIOS : PaySDKBase
    {
#if UNITY_IOS
        // [DllImport("__Internal")]
        // private static extern
        public override void InitializePaySDK()
        {
            base.InitializePaySDK();
        }

        public override void Buy(string sku, string type, Action<string> succeed, Action<string> failed)
        {
            base.Buy(sku, type, succeed, failed);
        }

        public override bool IsSubscribe()
        {
            return base.IsSubscribe();
        }

        public override void QuerySkuDetails()
        {
            base.QuerySkuDetails();
        }

#endif
    }
}
