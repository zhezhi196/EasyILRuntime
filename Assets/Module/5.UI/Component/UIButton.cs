using System;
using UnityEngine;
using UnityEngine.UI;

namespace Module
{
    [RequireComponent(typeof(Button))]
    public class UIButton : MonoBehaviour
    {
        private Button button;
        private float m_pointTime;
        public KeyCode substituteKey = KeyCode.None;
        
        public UIButton[] rejectBtn;

        public bool isPointing;
        
        private Action onClickAction;
        
        private Action<float> onPointing;
        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                if (!rejectBtn.IsNullOrEmpty())
                {
                    for (int i = 0; i < rejectBtn.Length; i++)
                    {
                        if (rejectBtn[i].isPointing)
                        {
                            return;
                        }
                    }

                }
    
                onClickAction?.Invoke();
            });
        }
#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKey(substituteKey))
            {
                m_pointTime += Time.unscaledDeltaTime;
                onPointing?.Invoke(m_pointTime);
            }
            else if (Input.GetKeyDown(substituteKey))
            {
                m_pointTime = 0;
                onClickAction?.Invoke();
            }
            
        }
#endif
        private void OnDestroy()
        {
            onClickAction = null;
            onPointing = null;
        }

        #region AddListener

        public void AddListener(Action callBack)
        {
            onClickAction += () => { callBack(); };
        }

        public void AddListener<T>(Action<T> callBack, T arg)
        {
            onClickAction += () => { callBack(arg); };
        }

        public void AddListener<T, K>(Action<T, K> callBack, T arg1, K arg2)
        {
            onClickAction += () => { callBack(arg1, arg2); };
        }

        public void AddListener<T, K, L>(Action<T, K, L> callBack, T arg1, K arg2, L arg3)
        {
            onClickAction += () => { callBack(arg1, arg2, arg3); };
        }

        public void AddListener<T, K, L, M>(Action<T, K, L, M> callBack, T arg1, K arg2, L arg3, M arg4)
        {
            onClickAction += () => { callBack(arg1, arg2, arg3, arg4); };
        }

        #endregion

        #region AddPointing

        public void AddPointing(Action<float> callBack)
        {
            onPointing += (pointTime) => { callBack(this.m_pointTime); };
        }

        public void AddPointing<T>(Action<float, T> callBack, T arg)
        {
            onPointing += (pointTime) => { callBack(this.m_pointTime, arg); };
        }

        public void AddPointing<T, K>(Action<float, T, K> callBack, T arg1, K arg2)
        {
            onPointing += (pointTime) => { callBack(this.m_pointTime, arg1, arg2); };
        }

        public void AddPointing<T, K, L>(Action<float, T, K, L> callBack, T arg1, K arg2, L arg3)
        {
            onPointing += (pointTime) => { callBack(this.m_pointTime, arg1, arg2, arg3); };
        }

        public void AddPointing<T, K, L, M>(Action<float, T, K, L, M> callBack, T arg1, K arg2, L arg3, M arg4)
        {
            onPointing += (pointTime) => { callBack(this.m_pointTime, arg1, arg2, arg3, arg4); };
        }

        #endregion
    }
}