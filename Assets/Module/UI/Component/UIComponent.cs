using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Module;
using UnityEngine;
using UnityEngine.UI;

namespace Module
{
    public static class UIComponent
    {
        private static Transform _backgroundTrasnform;
        private static Transform _normalTransform;
        private static Transform _forwardTrasnform;
        private static Transform _popupTransform;
        private static UIMask _mask;
        private static Loading m_loading;
        
        public static List<string> freezeList = new List<string>();
        public static bool isFreezed;

        public static void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                EventCenter.Dispatch(EventKey.Escape);
            }
        }
        public static Loading Loading
        {
            get
            {
                if (m_loading == null)
                {
                    m_loading = GameObject.Find("GamePlay/UIRoot/Canvas/Others/Loading").GetComponent<Loading>();
                }

                return m_loading;
            }
        }
        
        public static Transform BackgroundTrasnform
        {
            get
            {
                if (_backgroundTrasnform == null)
                {
                    _backgroundTrasnform = GameObject.Find("GamePlay/UIRoot/Canvas/BackGround").transform;
                }

                return _backgroundTrasnform;
            }
        }

        public static Transform NormalTransform
        {
            get
            {
                if (_normalTransform == null)
                {
                    _normalTransform = GameObject.Find("GamePlay/UIRoot/Canvas/Normal").transform;
                }

                return _normalTransform;
            }
        }

        public static Transform ForwardTrasnform
        {
            get
            {
                if (_forwardTrasnform == null)
                {
                    _forwardTrasnform = GameObject.Find("GamePlay/UIRoot/Canvas/Forward").transform;
                }

                return _forwardTrasnform;
            }
        }

        public static Transform PopupTransform
        {
            get
            {
                if (_popupTransform == null)
                {
                    _popupTransform = GameObject.Find("GamePlay/UIRoot/Canvas/Popup").transform;
                }

                return _popupTransform;
            }
        }

        public static UIMask Mask
        {
            get
            {
                if (_mask == null)
                {
                    _mask = GameObject.Find("GamePlay/UIRoot/Canvas/Others/Mask").GetComponent<UIMask>();
                }

                return _mask;
            }
        }

        public static void Freeze(string key)
        {
            if (!freezeList.Contains(key))
            {
                freezeList.Add(key);
            }

            isFreezed = true;
            Mask.gameObject.SetActive(true);
        }

        public static void UnFreeze(string key)
        {
            freezeList.Remove(key);
            if (freezeList.Count == 0)
            {
                isFreezed = false;
                Mask.gameObject.SetActive(false);
            }
        }

        
        public static float SetLoading(string key, string discription, float progress)
        {
            float lastValue = 0;
            float minus = 0;
            foreach (KeyValuePair<string, float> keyValuePair in Loading.processDic)
            {
                if (keyValuePair.Key != key)
                {
                    lastValue = keyValuePair.Value;
                }
                else
                {
                    minus = progress * (keyValuePair.Value - lastValue);
                    break;
                }
            }

            float result = lastValue + minus;
            return Loading.SetLoading(result, discription, true, 0.5F);
        }
    }
}
