using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Module
{
    public class UIMask : MonoBehaviour
    {
        [HideInInspector]
        public Image image;
        
        public Text content;
        
#if Debug_Log
        public void Awake()
        {
            image = GetComponent<Image>();
            content = transform.Find("Text").GetComponent<Text>();
            content.gameObject.SetActive(true);
        }
        
        private void Update()
        {
            content.text = string.Empty;
            for (int i = 0; i < UIComponent.freezeList.Count; i++)
            {
                content.text += (UIComponent.freezeList[i]+"\n");
            }
        }
#endif

    }
}
