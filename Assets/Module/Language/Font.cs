using System;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Module
{
    public class Font: MonoSingleton<Font>
    {
        private static UnityEngine.Font _arial;

        public static UnityEngine.Font arial
        {
            get
            {
                if (_arial == null) _arial = Resources.GetBuiltinResource<UnityEngine.Font>("Arial.ttf");
                return _arial;
            }
        }
        
        public FontInfo[] info;
        
#if UNITY_EDITOR
        private static UnityEngine.Font _font1;

        public static UnityEngine.Font font1
        {
            get
            {
                if (_font1 == null)
                    _font1 = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Font>("Assets/Bundles/Font/afterdisaster.ttf");
                return _font1;
            }
        }
        
        private static UnityEngine.Font _font2;

        public static UnityEngine.Font font2
        {
            get
            {
                if (_font2 == null)
                    _font2 = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Font>("Assets/Bundles/Font/GenShinGothic-Bold.ttf");
                return _font2;
            }
        }

        [Button]
        public void Set()
        {
            info = new[]
            {
                new FontInfo() {lan = SystemLanguage.ChineseSimplified, font = font2},
                new FontInfo() {lan = SystemLanguage.Chinese, font = font2},
                new FontInfo() {lan = SystemLanguage.English, font = font1},
                new FontInfo() {lan = SystemLanguage.Russian, font = font2},
                new FontInfo() {lan = SystemLanguage.Spanish, font = font1},
                new FontInfo() {lan = SystemLanguage.Portuguese, font = font1}
            };
        }
#endif
    }
}