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
        public Color maskColor;
        public Color normalColor;

        public void ShowMask()
        {
            bg.color = maskColor;
        }

        public void HideMask()
        {
            bg.color = normalColor;
        }

        //private void Update()
        //{
        //    dot.transform.eulerAngles += TimeHelper.unscaledDeltaTime * speed;
        //}
    }

}
