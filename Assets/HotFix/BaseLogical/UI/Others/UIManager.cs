using System.Collections;
using System.Collections.Generic;
using HotFix;
using Module;
using UnityEngine;

namespace HotFix
{
    public class UIManager : Manager
    {
        protected override int runOrder { get; }

        protected override string processDiscription
        {
            get { return "正在初始化UI"; }
        }

        protected override void BeforeInit()
        {
            EventCenter.Register(EventKey.Escape, UIObject.Back);
        }

        protected override void Init(RunTimeSequence runtime)
        {
            UIConfig.Init();
            Voter voter = new Voter(UIObject.UIObjects.Count);
            voter.OnComplete(() =>
            {
                Debug.Log("所有UI已预加载完毕");
                runtime.NextAction();
            });
            
            foreach (KeyValuePair<UIType, UIObject> keyValuePair in UIObject.UIObjects)
            {
                keyValuePair.Value.Init(voter);
            }
        }
    }
    
    
}
