using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Module
{
#if UNITY_EDITOR
    [Serializable]
    public class IconAsset
    {
        [HorizontalGroup,HideLabel,ReadOnly]
        public string altasName;
        [HorizontalGroup, HideLabel, InlineButton("Select"),InlineButton("Open")]
        public Sprite icon;

#if UNITY_EDITOR
        private void Select()
        {
            UnityEditor.Selection.activeObject = icon;
        }
        private void Open()
        {
            System.Diagnostics.Process.Start(Pathelper.FullAssetPath(icon));
        }
#endif
    }
#endif
    [Serializable]
    public class IconInfo
    {
        /// <summary>
        /// 对应的key值
        /// </summary>
        public string k;
        /// <summary>
        /// 对应的图集
        /// </summary>
        public string a;

        public override string ToString()
        {
            return k;
        }
    }
}