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

        private bool isDebug = false;

        #region SDK管理类

        #endregion

        public void InitALLSDK()
        {
            Debug.Log("SDKMgr SDK InitALLSDK");
            if (GameObject.Find("UACommunication") == null)
            {
                GameObject go = new GameObject("UACommunication");
                go.AddComponent<AdUACommunication>();
                go.AddComponent<PayUACommunication>();
                
            }

#if UNITY_EDITOR
            MyAdSDK = new AdSDKBase();
            MyPaySDK = new PaySDKBase();
            MyAnalyticsSDK = new AnalyticsSDKBase();
            MyCommon = new CommonBase();
#elif UNITY_ANDROID
             MyAdSDK = new AdSDKAndroid();
             MyPaySDK = new PaySDKAndroid();
             MyAnalyticsSDK = new AnalyticsSDKAndroid();
             MyCommon = new CommonAndroid();
#elif UNITY_IOS
            MyAdSDK = new AdSDKIOS();
            MyPaySDK = new PaySDKIOS();
            MyAnalyticsSDK = new AnalyticsSDKIOS();
            MyCommon = new CommonIOS();
#endif

            
            MyAdSDK.InitAdSDK();
            MyPaySDK.InitPaySDK();
            MyAnalyticsSDK.InitAnalytics();
            MyCommon.InitCommon();
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
