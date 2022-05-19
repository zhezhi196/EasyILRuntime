using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Module
{
    [Serializable]
    public struct TextKey
    {
        [HorizontalGroup,HideLabel]
        public string type;
        [HorizontalGroup,HideLabel,InlineButton("GetDes","文本")]
        public string textKey;
        
#if UNITY_EDITOR

        private void GetDes()
        {
            UnityEditor.EditorUtility.DisplayDialog("文本", Language.GetContent(textKey), "确认");
        }
#endif
    }
}