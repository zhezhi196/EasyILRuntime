using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

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
        [Button]
        public void FindInstanceId(int id,string  t)
        {
            Type type = Type.GetType(t);
            Object[] o = transform.GetComponentsInChildren(type);
            for (int i = 0; i < o.Length; i++)
            {
                if (o[i].GetInstanceID() == id)
                {
                    Debug.Log(gameObject.name, gameObject);
                }
            }
        }
#endif
    }
}