using System;
using System.Collections;
using System.Collections.Generic;
using ILRuntime.Runtime.Intepreter;
using Module;
using UnityEngine;
using xasset;

namespace HotFix
{
    public class ILBridgeHotFix : ILBridge
    {
        public void Init(BridgeBase modulBridge)
        {
            RunTimeSequence sequence = new RunTimeSequence();
            Manager.Initialize(sequence,modulBridge).OnComplete(OnHotFixFinish);
            sequence.BeginAction();
            if (Configuration.isEditor)
            {
                EventCenter.Register(EventKey.Update,Update);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                ResourceManager.LoadPrefab("Prefab/Cube.prefab",null, OnLoad);
            }
        }

        private void OnLoad(GameObject go) 
        {
            go.name = "33333";
        }
        
        private void OnHotFixFinish()
        {
            Debug.Log("完全做完初始化.....");
            UIObject.Open(UIConfig.UIMain, UITweenType.Fade);
            GMHelper.Init();
        }
    }

}

