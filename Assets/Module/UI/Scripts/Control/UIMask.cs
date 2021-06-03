using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Module
{
    public class UIMask : MonoBehaviour
    {
        public Image bg;
        public Transform dot;
        public Vector3 speed;
        public Color maskColor;
        public Color normalColor;
        public bool showMask;
        
        [ShowInInspector]
        public List<object> freezeList
        {
            get { return UICommpont.GetFreezeList(); }
        }

        public void ShowMask()
        {
            bg.color = maskColor;
            dot.gameObject.SetActive(true);
            showMask = true;
        }

        public void HideMask()
        {
            bg.color = normalColor;
            dot.gameObject.SetActive(false);
            showMask = false;
        }

        //private void Update()
        //{
        //    dot.transform.eulerAngles += TimeHelper.unscaledDeltaTime * speed;
        //}
    }

}
