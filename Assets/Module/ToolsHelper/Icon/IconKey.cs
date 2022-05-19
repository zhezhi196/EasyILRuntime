using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Module
{
    [Serializable]
    public struct IconKey
    {
        [HorizontalGroup,HideLabel]
        public string type;
        [HorizontalGroup,HideLabel]
        public string iconKey;

    }
}