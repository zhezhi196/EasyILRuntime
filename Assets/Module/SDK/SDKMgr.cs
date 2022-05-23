using System;
using UnityEngine;

namespace SDK
{
    public class SDKMgr
    {
        private static SDKMgr instance;
        public static SDKMgr GetInstance()
        {
            if (instance == null)
            {
                instance = new SDKMgr();
            }
            return instance;
        }

        public AdSDKBase MyAdSDK { get; private set; }
        public PaySDKBase MyPaySDK { get; private set; }
        public AnalyticsSDKBase MyAnalyticsSDK { get; private set; }
        public CommonBase MyCommon { get; private set; }
        public OneSignalBase MyOneSignal { get; private set; }
        public AntiaddictionSDKBase MyAntiaddictionSDKBase { get; private set; }

        private bool isDebug = false;

        #region SDK管理类

        #endregion

        public void InitALLSDK()
        {
            Debug.Log("SDKMgr SDK InitALLSDK");
            GameObject go = GameObject.Find("UACommunication");
            if (go == null)
            {
                go = new GameObject("UACommunication");
            }
            if (!go.GetComponent<AdUACommunication>())
                go.AddComponent<AdUACommunication>();
            if (!go.GetComponent<PayUACommunication>())
                go.AddComponent<PayUACommunication>();
            if (!go.GetComponent<InputUACommunication>())
                go.AddComponent<InputUACommunication>();
            if (!go.GetComponent<AntiaddictionUACommunication>())
                go.AddComponent<AntiaddictionUACommunication>();

#if UNITY_EDITOR|| !SDK
            MyAdSDK = new AdSDKBase();
            MyPaySDK = new PaySDKBase();
            MyAnalyticsSDK = new AnalyticsSDKBase();
            MyCommon = new CommonBase();
            MyOneSignal = new OneSignalBase();
            MyAntiaddictionSDKBase = new AntiaddictionSDKBase();
#elif UNITY_ANDROID
             MyAdSDK = new AdSDKAndroid();
             MyPaySDK = new PaySDKAndroid();
             MyAnalyticsSDK = new AnalyticsSDKAndroid();
             MyCommon = new CommonAndroid();
             MyOneSignal = new OneSignalAndroid();
             MyAntiaddictionSDKBase = new AntiaddictionSDKAndroid();
#elif UNITY_IOS
            MyAdSDK = new AdSDKIOS();
            MyPaySDK = new PaySDKIOS();
            MyAnalyticsSDK = new AnalyticsSDKIOS();
            MyCommon = new CommonIOS();
            MyOneSignal = new OneSignalIOS();
            MyAntiaddictionSDKBase = new AntiaddictionSDKIOS();
#endif


            MyAdSDK.InitAdSDK();
            MyPaySDK.InitPaySDK();
            MyAnalyticsSDK.InitAnalytics();
            MyCommon.InitCommon();
            MyOneSignal.InitOneSignal();
            MyAntiaddictionSDKBase.Init();
            isDebug =  MyCommon.IsSDKLog();
            Debug.Log("Unity ---  isDebug = " + isDebug);
        }

        public void Log(string ss)
        {
            if (isDebug)
            {
                Debug.Log("Unity --- SDK --- : " + ss);
            }
          
        }

       

    }

}
