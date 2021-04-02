/*
 * 脚本名称：PopupWin
 * 脚本作者：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2018-03-13 17:06:13
 * 脚本作用：
*/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Module
{
    public class PopupWin : MonoBehaviour
    {
        private struct PopupState
        {
            public string Name;
            public bool isFocus;
            public Action callBack;
        }
	
        [SerializeField] private Text m_title;
        [SerializeField] private Text m_content;
        [SerializeField] private UIBtnBase[] m_btnList;
        [SerializeField] private CanvasGroup m_backGround;
        [SerializeField] private Color[] m_fontColor;

        private List<PopupState> statList = new List<PopupState>();
	
        /// <summary>
        /// 标题
        /// </summary>
        public Text Title
        {
            get { return m_title; }
        }

        /// <summary>
        /// 内容
        /// </summary>
        public Text Content
        {
            get { return m_content; }
        }

        public void Open()
        {
        }

        public void AddListener(string name, bool isFourse, Action Fun)
        {
            PopupState state = new PopupState();
            state.Name = name;
            state.isFocus = isFourse;
            state.callBack = Fun;
            statList.Add(state);
        }

        public void Close()
        {
        }
    }
}