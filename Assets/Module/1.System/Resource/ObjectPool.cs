/*
 * 脚本名称：ObjectItem
 * 脚本作者：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2018-03-20 13:52:47
 * 脚本作用：
*/

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Module
{
    public class ObjectPool
    {
        private Queue<Action<GameObject>> cacheAction = new Queue<Action<GameObject>>();
        
        #region poolRoot

        private static Transform m_root;
        public static Transform poolRoot
        {
            get
            {
                if (m_root == null)
                {
                    m_root=new GameObject("PoolRoot").transform;
                }

                return m_root;
            }
        }

        #endregion

        /// <summary>
        /// 对象的prefab
        /// </summary>
        public GameObject prefab { private set; get; }
        
        public bool isActive
        {
            get { return prefab != null; }
        }
        
        /// <summary>
        /// 所有生成的对象
        /// </summary>
        private  Queue<GameObject> activeObject { set; get; }

        public ObjectPool(GameObject prefab)
        {
            InitPrefab(prefab);
        }

        public ObjectPool()
        {
        }

        public void InitPrefab(GameObject prefab)
        {
            this.prefab = prefab;
            activeObject = new Queue<GameObject>();
        }

        public void InvokeAllCacheAction()
        {
            for (int i = 0; i < cacheAction.Count; i++)
            {
                GetObject(cacheAction.Dequeue());
            }
        }
        
        public void GetObject<T>(Action<T> callBack)
        {
            GetObject(go =>
            {
                callBack?.Invoke(go.GetComponent<T>());
            });
        }
        
        public void GetObject(Action<GameObject> callBack)
        {
            if (!isActive)
            {
                cacheAction.Enqueue(callBack);
                return;
            }

             GameObject returnValue = activeObject.Count > 0 ? activeObject.Dequeue() : GameObject.Instantiate(prefab);
             IPoolObject target = returnValue.GetComponent<IPoolObject>();
             if (target != null)
             {
                 target.pool = this;
                 target.OnGetObjectFromPool();
             }

            returnValue.SetActive(true);
            callBack?.Invoke(returnValue);
        }

        /// <summary>
        /// 归还对象
        /// </summary>
        /// <param id="obj"></param>
        public void ReturnObject(GameObject obj)
        {
            if (obj != null)
            {
                activeObject.Enqueue(obj);
                obj.SetActive(false);
                IPoolObject target = obj.GetComponent<IPoolObject>();
                if (target != null)
                {
                    target.OnReturnToPool();
                }
            }
        }
        public void ReturnObject(IPoolObject obj)
        {
            if (obj != null)
            {
                activeObject.Enqueue(obj.gameObject);
                obj.gameObject.SetActive(false);
                obj.OnReturnToPool();
            }
        }
    }
}