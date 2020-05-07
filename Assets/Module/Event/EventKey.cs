using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public static class EventKey
    {
        public const string BundleInitStart = "BundleInitStart";
        public const string BundleProcess = "BundleProcess";
        public const string BundleInitComplete = "BundleComplete";
        
        public const string Update = "Event_Update";
        public const string FixedUpdate = "Event_FixedUpdate";
        public const string LateUpdate = "Event_LateUpdate";
        public const string Escape = "Event_Escape";
        public const string OnApplicationPause = "Event_OnApplicationPause";
        public const string OnApplicationFocus = "Event_OnApplicationFocus";
        public const string OnApplicationQuit = "Event_OnApplicationQuit";
        
        public const string OnShowUI = "OnShowUI";
    }


}
