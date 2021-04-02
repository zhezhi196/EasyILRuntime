using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SDK
{

    public class AnalyticsSDKIOS : AnalyticsSDKBase
    {

#if UNITY_IOS
      // [DllImport("__Internal")]
        // private static extern

        public override void InitAnalytics()
        {
            base.InitAnalytics();
        }

        public override void OnEvent(string eventName, E_AnalyticsType e_AnalyticsType)
        {
            base.OnEvent(eventName, e_AnalyticsType);
        }

        public override void OnStartLevelEvent(string levelId, E_AnalyticsType e_AnalyticsType)
        {
            base.OnStartLevelEvent(levelId, e_AnalyticsType);
        }

        public override void OnFailLevelEvent(string levelId, E_AnalyticsType e_AnalyticsType)
        {
            base.OnFailLevelEvent(levelId, e_AnalyticsType);
        }

        public override void OnFinishLevelEvent(string levelId, E_AnalyticsType e_AnalyticsType)
        {
            base.OnFinishLevelEvent(levelId, e_AnalyticsType);
        }

#endif
    }
}
