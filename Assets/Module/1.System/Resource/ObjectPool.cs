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
        private Queue<(Action<GameObject>, bool)> cacheAction = new Queue<(Action<GameObject>, bool)>();

        #region poolRoot

        private static Transform m_root;

        public static Transform poolRoot
        {
            get
            {
                if (m_root == null)
                {
                    m_root = new GameObject("PoolRoot").transform;
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
        private Queue<GameObject> activeObject { set; get; }

        public string path { get; set; }

        public int count
        {
            get { return activeObject.Count; }
        }

        public ObjectPool(string path, GameObject prefab)
        {
            InitPrefab(path, prefab);
        }

        public ObjectPool(string path)
        {
            this.path = path;
        }

        public void InitPrefab(string path, GameObject prefab)
        {
            this.path = path;
            this.prefab = prefab;
            activeObject = new Queue<GameObject>();
        }

        public void InvokeAllCacheAction()
        {
            int index = cacheAction.Count;
            for (int i = 0; i < index; i++)
            {
                var temp = (cacheAction.Dequeue());
                GetObject(temp.Item1, temp.Item2);
            }
        }

        public void GetObject<T>(Action<T> callBack, bool fromPool = true)
        {
            GetObject(go => { callBack?.Invoke(go.GetComponent<T>()); }, fromPool);
        }

        public void GetObject(Action<GameObject> callBack, bool fromPool = true)
        {
            if (!isActive)
            {
                cacheAction.Enqueue((callBack, fromPool));
                return;
            }

            GameObject returnValue = activeObject.Count > 0 && fromPool
                ? activeObject.Dequeue()
                : GameObject.Instantiate(prefab);
            IPoolObject target = returnValue.GetComponent<IPoolObject>();
            returnValue.SetActive(true);
            callBack?.Invoke(returnValue);
            if (target != null)
            {
                target.pool = this;
                target.OnGetObjectFromPool();
            }
        }

        public GameObject GetObject(bool fromPool = true)
        {
            if (!isActive)
            {
                Debug.LogError("GameObject prefab is null");
                return null;
            }

            GameObject returnValue = activeObject.Count > 0 && fromPool
                ? activeObject.Dequeue()
                : GameObject.Instantiate(prefab);
            IPoolObject target = returnValue.GetComponent<IPoolObject>();
            returnValue.SetActive(true);
            if (target != null)
            {
                target.pool = this;
                target.OnGetObjectFromPool();
            }

            return returnValue;
        }

        public T GetObject<T>(bool fromPool = true) where T : IPoolObject
        {
            if (!isActive)
            {
                Debug.LogError("GameObject prefab is null");
                return default;
            }

            GameObject returnValue = activeObject.Count > 0 && fromPool
                ? activeObject.Dequeue()
                : GameObject.Instantiate(prefab);
            IPoolObject target = returnValue.GetComponent<IPoolObject>();
            returnValue.SetActive(true);
            if (target != null)
            {
                target.pool = this;
                target.OnGetObjectFromPool();
            }

            return (T) target;
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
                obj.gameObject.transform.SetParent(poolRoot);
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
                obj.gameObject.transform.SetParent(poolRoot);
                obj.OnReturnToPool();
            }
        }
    }
}