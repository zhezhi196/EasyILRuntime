using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Module
{
    public class UIStack: MonoBehaviour
    {
#if UNITY_EDITOR
        [ShowInInspector]
        public List<string> pool
        {
            get
            {
                List<string> temp = new List<string>();
                for (int i = 0; i < UIController.Instance.winList.Count; i++)
                {
                    temp.Add(UIController.Instance.winList[i].winName);
                }

                return temp;
            }
        }
#endif
    }
}