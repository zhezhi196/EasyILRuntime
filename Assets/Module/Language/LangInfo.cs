using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Module
{
    [Serializable]
    public struct LangInfo
    {
        [HorizontalGroup(),HideLabel]
        public SystemLanguage l;
        [HorizontalGroup(),HideLabel]
        public string c;

        public override string ToString()
        {
            return c;
        }
    }
}