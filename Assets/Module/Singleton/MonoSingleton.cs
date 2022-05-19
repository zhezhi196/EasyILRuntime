/*
 * 脚本名称：MonoSingleton
 * 项目名称：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2017-12-21 22:26:01
 * 脚本作用：
*/

using System;

namespace Module
{
    using UnityEngine;

    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
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

        protected virtual void OnDestroy()
        {
            
        }
        
        private void OnApplicationQuit()
        {
            instance = null;
        }
    }
}
