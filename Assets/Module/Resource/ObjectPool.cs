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
        public class ObjectItem
        {
            public bool isActive { set; get; }
            public Object Obj;

            public ObjectItem(Object obj, bool isActive)
            {
                this.isActive = isActive;
                Obj = obj;
            }
        }

        private Queue<Action<Object>> cache = new Queue<Action<Object>>();
        
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
        public Object prefab { private set; get; }

        private Dictionary<string, object> allPara = new Dictionary<string, object>();
        
        public bool isActive
        {
            get { return prefab != null; }
        }
        
        /// <summary>
        /// 所有生成的对象
        /// </summary>
        public List<ObjectItem> ObjectList { private set; get; }

        public ObjectPool(Object prefab)
        {
            InitPrefab(prefab);
        }

        public ObjectPool()
        {
        }

        public void InitPrefab(Object prefab)
        {
            this.prefab = prefab;

            ObjectList = new List<ObjectItem>();
        }

        public void GetCacheObject()
        {
            for (int i = 0; i < cache.Count; i++)
            {
                GetObject(cache.Dequeue());
            }
        }

        public void GetObject(Action<Object> callBack)
        {
            if (!isActive)
            {
                cache.Enqueue(callBack);
                return;
            }
            for (int i = 0; i < ObjectList.Count; i++)
            {
                if (!ObjectList[i].isActive)
                {
                    ObjectList[i].isActive = true;
                    if (ObjectList[i].Obj == null)
                    {
                        ObjectList.RemoveAt(i);
                        continue;
                    }
                    callBack?.Invoke(ObjectList[i].Obj);
                    return ;
                }
            }

            Object obj = GameObject.Instantiate(prefab);
            ObjectItem newItem = new ObjectItem(obj, true);
            ObjectList.Add(newItem);
            callBack?.Invoke(obj);
        }

        /// <summary>
        /// 归还对象
        /// </summary>
        /// <param id="obj"></param>
        public void ReturnObject(Object obj)
        {
            for (int i = 0; i < ObjectList.Count; i++)
            {
                if (ObjectList[i].Obj.Equals(obj) && ObjectList[i].isActive)
                {
                    ObjectList[i].isActive = false;
                    ((GameObject)ObjectList[i].Obj).transform.SetParent(poolRoot);
                    break;
                }
            }
        }

        public void RemoveObject(Object obj)
        {
            for (int i = 0; i < ObjectList.Count; i++)
            {
                if (ObjectList[i].Obj.Equals(obj))
                {
                    ObjectList.RemoveAt(i);
                    break;
                }
            }
        }
    }
}