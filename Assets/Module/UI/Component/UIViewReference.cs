using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public class UIViewReference : ViewReference
    {
        public float tweenInterval = 0.4f;
        public AnimationCurve tweenCurve=new AnimationCurve(new Keyframe(0,0),new Keyframe(1,1));
        public float delay;
    }
}
