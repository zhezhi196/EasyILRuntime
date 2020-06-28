using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public enum UITweenType
    {
        Ignore,
        None,
        Fade,
        Left,
        Right,
        Up,
        Down,
        Scale
    }

    public enum OpenFlag
    {
        /// <summary>
        /// ABCD---打开B---ABCDB
        /// </summary>
        Inorder,

        /// <summary>
        /// ABCD---打开B---AB
        /// </summary>
        Insertion,
    }
    
    public static class UIHelper
    {
        private enum OpenOrClose
        {
            Close = 1,
            Open
        }

        public static List<UICtrl> uiQueue = new List<UICtrl>();

        public static UICtrl currentUI
        {
            get { return uiQueue.IsNullOrEmpty() ? null : uiQueue.GetLast(); }
        }

        public static void Open(UIType ui, UITweenType tweenType)
        {
            
        }

        public static void Popup(UIType ui, UITweenType tweenType)
        {
            
        }
        
        public static void Back()
        {
            
        }
        
    }
}
