using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Module
{
    public abstract class GameEntry<T> : MonoSingleton<T> where T : MonoBehaviour, IConfig
    {
        public GameObject[] dontDestroy;
        [SerializeField] private Config _config;

        public Config config => _config;

        #region Private

        protected AsyncLoadProcess process;
        protected DateTime m_record;

        #endregion

        #region Event

        public Action OnGameUpdate;
        public Action OnGameFixUpdate;
        public Action OnBack;
        public Action OnGameInit;

        #endregion

        #region Awake

        protected void Awake()
        {
            for (int i = 0; i < dontDestroy.Length; i++)
            {
                if (dontDestroy[i] != null)
                    DontDestroyOnLoad(dontDestroy[i]);
            }

            m_record = TimeHelper.now;
            process = new AsyncLoadProcess();
        }

            #endregion

        #region Update

        protected virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Back();
            }

            OnGameUpdate?.Invoke();
        }
        
        protected virtual void FixedUpdate()
        {
            OnGameFixUpdate?.Invoke();
        }

        public abstract void Back();
        public abstract void OnPause();
        public abstract void OnContinue();

        #endregion

        #region Log

        [Conditional("LOG_ENABLE")]
        protected void Log(string content)
        {
            GameDebug.Log(content + "<==> 用时: " + (TimeHelper.now - m_record).TotalSeconds);
            m_record = TimeHelper.now;
        }

        #endregion

        #region Pause

        public void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                OnPause();
            }
            else
            {
                OnContinue();
            }
        }
        public void OnApplicationFocus(bool focus)
        {
            if (!focus)
            {
                OnPause();
            }
            else
            {
                OnContinue();
            }
        }

        #endregion

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UIController.Instance.Dispose();
            UICommpont.ClearList();
        }
    }
}