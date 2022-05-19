using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDK
{

    public class InputUACommunication : MonoBehaviour
    {

        public void OnInputShowChange(string isShow)
        {
            Debug.Log("UACommunication OnInputShowChange:" + isShow);
           SDKMgr.GetInstance().MyCommon.OnInputShowChange(isShow);
        }

        public void OnInputValueChange(string value)
        {
            Debug.Log("UACommunication OnInputValueChange:" + value);
            SDKMgr.GetInstance().MyCommon.OnInputValueChange(value);
        }

      

    }
}