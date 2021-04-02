/*
 * 脚本名称：MonoSingleton
 * 项目名称：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2017-12-21 22:26:01
 * 脚本作用：
*/

namespace Module
{
    using UnityEngine;

    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T instance = null;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                }

                return instance;
            }
        }

        public virtual void Init()
        {
        }

        protected virtual void Awake()
        {
            if (instance != null)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        protected virtual void OnDestory()
        {
            DeInitialize();
        }

        public virtual void Initialize()
        {
        }

        public virtual void DeInitialize()
        {
        }

        private void OnApplicationQuit()
        {
            instance = null;
        }
    }
}
