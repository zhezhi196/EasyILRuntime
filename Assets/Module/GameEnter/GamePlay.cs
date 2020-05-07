using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xasset;
using Object = UnityEngine.Object;

namespace Module
{
    public class GamePlay : MonoBehaviour
    {
        private BridgeBase bridge;

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
        
        protected virtual void OnBeforeInit()
        {
            FPS.SetFps();
        }

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
            Clock.UpdateLilst();
            FPS.Update();
            Async.Update();
            UIComponent.Update();
            EventCenter.Dispatch(EventKey.Update);

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                EventCenter.Dispatch(EventKey.Escape);
            }
            
            if(Input.GetKeyDown(KeyCode.T))
            {
                //Debug.Log(ServerSimulator.GetSqlService<PlayerData>().Where((data => data.ID == 1)).name);
            }
            else if (Input.GetKeyDown(KeyCode.Y))
            {
            }
            else if (Input.GetKeyDown(KeyCode.U))
            {
            }
        }

        #region 原生事件

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