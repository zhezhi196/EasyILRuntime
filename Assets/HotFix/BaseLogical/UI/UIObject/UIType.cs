using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotFix
{
    public class UIType
    {
        public string name;
        public string prefabPath;
        public string componentFoder;
        public bool preLoad;
        public Type ctrlType;
        public Type modulType;
        public Type viewType;

        public UIType(Type ctrlType, Type modulType, Type viewType, string name)
        {
            this.ctrlType = ctrlType;
            this.modulType = modulType;
            this.viewType = viewType;
            this.name = name;
            UIObject ui = new UIObject(this);
            UIObject.UIObjects.Add(this, ui);
        }

        public void InitAttribute(string prefabPath, string compoentFoder, bool preLoad)
        {
            this.prefabPath = prefabPath;
            this.componentFoder = compoentFoder;
            this.preLoad = preLoad;
        }
    }


}
