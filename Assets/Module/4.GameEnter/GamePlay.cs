using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using MongoDB.Bson;
using UnityEngine;
using xasset;
using Object = UnityEngine.Object;

namespace Module
{
    public class GamePlay : MonoBehaviour
    {
        #region gamePointer

        private static GamePlay instance;

        public static GamePlay Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GamePlay>();
                }

                return instance;
            }
        }
        
        #endregion
        
        private BridgeBase bridge;
        
        protected virtual void OnBeforeInit()
        {
            FPS.SetFps();
        }
        
        #region 原生事件
        
        void Awake()
        {
            OnBeforeInit();
            DontDestroyOnLoad(gameObject);
            RunTimeSequence sequence = new RunTimeSequence();
            Manager.Initialize(sequence).OnComplete(() => bridge = HotFixManager.GetBridge());
            sequence.BeginAction();
        }
        
        private void Update()
        {
            Clock.Update();
            FPS.Update();
            Async.Update();
            UIComponent.Update();
            EventCenter.Dispatch(EventKey.Update);
        }

        private void FixedUpdate()
        {
            EventCenter.Dispatch(EventKey.FixedUpdate);
        }

        private void LateUpdate()
        {
            EventCenter.Dispatch(EventKey.LateUpdate);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            EventCenter.Dispatch(EventKey.OnApplicationPause,pauseStatus);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            EventCenter.Dispatch(EventKey.OnApplicationFocus, hasFocus);
        }

        private void OnApplicationQuit()
        {
            EventCenter.Dispatch(EventKey.OnApplicationQuit);
        }

        #endregion
    }
}