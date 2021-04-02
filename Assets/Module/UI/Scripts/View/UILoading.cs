/*
 * 脚本名称：LoadingEntity
 * 项目名称：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2018-01-08 23:23:44
 * 脚本作用：
*/

using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Module
{
    public class UILoading : MonoBehaviour
    {
        private float updateTime;
        private int m_loadingCache;
        private CanvasGroup canvasGroup;
        public Text title;

        public void Open()
        {
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddOrGetComponent<CanvasGroup>();
            }

            canvasGroup.alpha = 1;
            this.gameObject.OnActive(true);
            m_loadingCache++;
        }

        public void Close()
        {
            m_loadingCache--;
            if (m_loadingCache <= 0)
            {
                if (canvasGroup == null)
                {
                    canvasGroup = gameObject.AddOrGetComponent<CanvasGroup>();
                }
                canvasGroup.alpha = 0;
                this.gameObject.OnActive(false);
                title.gameObject.OnActive(false);
            }
        }

        public void SetText(string text)
        {
            this.title.gameObject.OnActive(true);
            this.title.text = text;
        }
    }
}