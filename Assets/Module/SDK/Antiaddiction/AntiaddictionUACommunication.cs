using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDK
{
    public class AntiaddictionUACommunication : MonoBehaviour
    {
        public void AntiaddictionLoginState(string state)
        {
            SDKMgr.GetInstance().Log("AntiaddictionLoginState ---- state = " + state);
            SDKMgr.GetInstance().MyAntiaddictionSDKBase.OnLoginState(state);
        }

        
    }
}
