using System;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Module
{
    [Serializable]
    public class FontInfo
    {
        private static IConfig _info;

        public static FontInfo[] info
        {
            get
            {
                if (_info == null)
                {
                    _info = GameObject.Find("GamePlay").GetComponent<IConfig>();
                }

                return _info.config.font;
            }
        }

        [HorizontalGroup,HideLabel]
        public SystemLanguage lan;
        [HorizontalGroup,HideLabel]
        public UnityEngine.Font font;
    }

}