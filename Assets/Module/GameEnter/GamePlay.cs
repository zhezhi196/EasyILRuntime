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
            Clock.Update();
            FPS.Update();
            Async.Update();
            UIComponent.Update();
            
            EventCenter.Dispatch(EventKey.Update);
            
            if(Input.GetKeyDown(KeyCode.T))
            {
                BundleManager.LoadGameoObject("Prefab/Cube.prefab", OnLoadTest);
                //Debug.Log(ServerSimulator.GetSqlService<PlayerData>().Where((data => data.ID == 1)).name);
            }
            else if (Input.GetKeyDown(KeyCode.Y))
            {
                Color c=new Color(0.5f,0.5f,0.5f,0.5f);
                Debug.Log(c.ToString());
            }
            else if (Input.GetKeyDown(KeyCode.U))
            {
                JsonData data = new JsonData();
                object ddd = 1;
                data["aaa"] = "1";
                Debug.Log(data.ToJson());
            }


            //Debug.Log("loadIndex: " + loadIndex++);
        }

        private int loadIndex;
        private int index;

        private void OnLoadTest(GameObject obj)
        {
            Debug.Log("OnLoadTest: " + index++);
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