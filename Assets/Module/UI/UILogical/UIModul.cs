using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Module;
using UnityEngine;

namespace Module
{
    public class UIModul
    {
        public UITweenType tweenType;
        public object[] args;
        public bool isPopup;
        public UIObject from;
        public Clock clock = new Clock();

        public void OnRefresh(UITweenType tweenType, bool isPopup, UIObject from, object[] args)
        {
            this.tweenType = tweenType;
            this.isPopup = isPopup;
            this.args = args;
            this.from = from;
            clock.Restart();
        }

        public void OnDisable()
        {
            clock.Pause();
        }
    }
}
