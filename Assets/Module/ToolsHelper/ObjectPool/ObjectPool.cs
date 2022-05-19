/*
 * 脚本名称：ObjectItem
 * 脚本作者：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2018-03-20 13:52:47
 * 脚本作用：
*/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Module
{
    [Flags]
    public enum PoolFlag
    {
        NotFromPool = 1 << 0,
        NotAutoActive = 1 << 1,
        NotSetParent=1<<2
    }

    public class ObjectPool
    {
        private static readonly Dictionary<string, ObjectPool> poolDic = new Dictionary<string, ObjectPool>();

        public static void OnSceneChanged()
        {
            var newOne = new Dictionary<string, ObjectPool>(poolDic);
            foreach (KeyValuePair<string,ObjectPool> keyValuePair in newOne)
            {
                keyValuePair.Value.ClearNull();
            }
        }

        #region Get & Return

        public static ObjectPool GetPool(string path, Transform root)
        {
            ObjectPool pool = null;
            if (!poolDic.TryGetValue(path, out pool))
            {
                pool = new ObjectPool(path, root);
                poolDic.Add(path, pool);
            }

            return pool;
        }

        public static void ReturnToPool(GameObject go)
        {
            foreach (KeyValuePair<string, ObjectPool> keyValuePair in poolDic)
            {
                for (int i = 0; i < keyValuePair.Value.poolObject.Count; i++)
                {
                    if (keyValuePair.Value.poolObject[i].gameObject == go)
                    {
                        keyValuePair.Value.ReturnObject(keyValuePair.Value.poolObject[i]);
                    }
                }
            }
        }

        public static void ReturnToPool(IPoolObject go)
        {
            if (go.pool != null)
            {
                go.pool.ReturnObject(go);
            }
            else
            {
                ReturnToPool(go.gameObject);
            }
        }
        
        public static void RemoveObjectFromPool(GameObject go)
        {
            ObjectPool tar = null;
            foreach (KeyValuePair<string,ObjectPool> keyValuePair in poolDic)
            {
                if (keyValuePair.Value.RemoveFromPool(go))
                {
                    if (keyValuePair.Value.poolObject.Count == 0)
                    {
                        tar = keyValuePair.Value;
                    }
                    break;
                }
            }

            if (tar != null)
            {
                tar.Destroy();
            }
        }

        public static ObjectPool Cache(string path, int count)
        {
            ObjectPool pool = GetPool(path, poolRoot);
            pool.SetDefaultCount(count);
            return pool;
        }

        #endregion
        
        #region PoolData
        private enum ObjectType
        {
            UnKnow,
            GameObject,
            IPoolObject
        }
        private class PoolData
        {
            public GameObject gameObject;
            public bool active;
            public string id;

            public PoolData(GameObject go, string id)
            {
                this.gameObject = go;
                this.id = id;
            }

            public PoolData SetActive(bool active)
            {
                this.active = active;
                return this;
            }
        }

        #endregion

        #region poolRoot

        public const string poolPoint = "GamePlay/PoolRoot";
        private static Transform m_root;

        public static Transform poolRoot
        {
            get
            {
                if (m_root == null)
                {
                    GameObject o = GameObject.Find(poolPoint);
                    if (o != null)
                    {
                        m_root = o.transform;
                    }
                }

                return m_root;
            }
        }

        #endregion

        #region Private

        private Queue<(Action<GameObject>, Transform, PoolFlag)> cacheAction = new Queue<(Action<GameObject>, Transform, PoolFlag)>();

        private Transform _parentRoot;
        private ObjectType objectType;
        public bool isLoading;
        private List<PoolData> poolObject = new List<PoolData>();
        private AsyncOperationHandle<GameObject> handles;
        public event Action onComplete;

        #endregion

        #region Property

        public string path { get; }

        public Transform parentRoot
        {
            get
            {
                if (_parentRoot == null || _parentRoot.gameObject.IsNullOrDestroyed()) return poolRoot;
                return _parentRoot;
            }
        }

        private ObjectPool(string path, Transform root)
        {
            this.path = path;
            _parentRoot = root;
        }

        #endregion
        
        private void ClearNull()
        {
            for (int i = poolObject.Count - 1; i >= 0; i--)
            {
                CheckNull(poolObject[i], i);
            }

            if (poolObject.Count == 0)
            {
                Destroy();
            }
        }

        public void SetDefaultCount(int count, Transform parent)
        {
            for (int i = 0; i < count; i++)
            {
                if (path != null)
                {
                    AddNewToPool(path, true, parent, go =>
                    {
                        go.OnActive(false);
                    });
                }
            }
        }

        public void SetDefaultCount(int count)
        {
            SetDefaultCount(count, parentRoot);
        }

        public bool RemoveFromPool(GameObject go)
        {
            for (int i = poolObject.Count - 1; i >= 0; i--)
            {
                if (poolObject[i].gameObject == go)
                {
                    poolObject.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public void Destroy()
        {
            poolDic.Remove(path);
            AssetLoad.Release(handles);
        }

        #region GetObject

        public void GetObject<T>(Transform parent, Action<T> callBack, PoolFlag flag = 0)
        {
            GetObjectInternal(go => callBack?.Invoke(go.GetComponent<T>()), parent, flag);
        }

        public void GetObject(Transform parent, Action<GameObject> callBack, PoolFlag flag = 0)
        {
            GetObjectInternal(go => callBack?.Invoke(go), parent, flag);
        }

        public void GetObject(Action<GameObject> callBack, PoolFlag flag = 0)
        {
            GetObjectInternal(go => callBack?.Invoke(go), parentRoot, flag);
        }

        public void GetObject<T>(Action<T> callBack, PoolFlag flag = 0)
        {
            GetObjectInternal(go => callBack?.Invoke(go.GetComponent<T>()), parentRoot, flag);
        }

        private void GetObjectInternal(Action<GameObject> callBack, Transform parent, PoolFlag flag)
        {
            if (isLoading)
            {
                cacheAction.Enqueue((callBack, parent, flag));
                return;
            }

            PoolData data = FindActive();

            if (data != null && !flag.HasFlag(PoolFlag.NotFromPool))
            {
                GameObject returnValue = data.SetActive(false).gameObject;
                OnGetGo(callBack, parent, flag, returnValue);
            }
            else
            {
                isLoading = true;
                AddNewToPool(path, false, parent, go =>
                {
                    isLoading = false;
                    int count = cacheAction.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var itm = cacheAction.Dequeue();
                        GetObject(itm.Item2, itm.Item1, itm.Item3);
                    }
                    
                    OnGetGo(callBack, parent, flag, go);
                });
            }
        }

        private void OnGetGo(Action<GameObject> callBack, Transform parent, PoolFlag flag,GameObject returnValue)
        {
            if (!flag.HasFlag(PoolFlag.NotAutoActive) && !returnValue.activeInHierarchy)
            {
                returnValue.OnActive(true);
            }

            if ((flag & PoolFlag.NotSetParent) == 0)
            {
                returnValue.transform.SetParentZero(parent);
            }

            callBack?.Invoke(returnValue);
            if (objectType == ObjectType.UnKnow)
            {
                IPoolObject target = returnValue.GetComponent<IPoolObject>();
                if (target != null)
                {
                    objectType = ObjectType.IPoolObject;
                    target.pool = this;
                    target.OnGetObjectFromPool();
                }
                else
                {
                    objectType = ObjectType.GameObject;
                }
            }
            else if (objectType == ObjectType.IPoolObject)
            {
                IPoolObject target = returnValue.GetComponent<IPoolObject>();
                target.pool = this;
                target.OnGetObjectFromPool();
            }
        }
        
        #endregion

        #region ReturnObject

        private void ReturnObject(PoolData data)
        {
            data.SetActive(true);
            data.gameObject.OnActive(false);
            if (data.gameObject != null)
            {
                data.gameObject.transform.SetParentZero(parentRoot);
            }
        }

        /// <summary>
        /// 归还对象
        /// </summary>
        /// <param id="obj"></param>
        public void ReturnObject(GameObject obj, bool autoActive = true)
        {
            if (obj != null)
            {
                FindData(obj).SetActive(true);
                if (autoActive) obj.OnActive(false);
                if (obj.gameObject != null)
                {
                    obj.gameObject.transform.SetParentZero(parentRoot);
                }
            }
        }

        public void ReturnObject(IPoolObject obj, bool autoActive = true)
        {
            if (obj != null)
            {
                ReturnObject(obj.gameObject, autoActive);
                obj.ReturnToPool();
            }
        }

        public void DisposeFromMemory()
        {
            for (int i = 0; i < poolObject.Count; i++)
            {
                if (poolObject[i].gameObject != null)
                {
                    AssetLoad.Destroy(poolObject[i].gameObject);
                }
            }
        }

        #region Find

        private PoolData FindData(GameObject go)
        {
            for (int i = poolObject.Count - 1; i >= 0; i--)
            {
                PoolData temp = poolObject[i];
                if (CheckNull(temp, i)) continue;
                if (temp.gameObject == go) return temp;
            }

            return null;
        }

        private PoolData FindActive()
        {
            for (int i = poolObject.Count - 1; i >= 0; i--)
            {
                PoolData temp = poolObject[i];
                if (CheckNull(temp, i)) continue;
                if (temp.active) return temp;
            }

            return null;
        }

        private bool CheckNull(PoolData temp, int index)
        {
            if (temp.gameObject.IsNullOrDestroyed())
            {
                poolObject.RemoveAt(index);
                return true;
            }

            return false;
        }
        
        private void AddNewToPool(string path, bool active, Transform parent, Action<GameObject> callback)
        {
            AssetLoad.Instantiate(path, parent, handle =>
            {
                this.handles = handle;
                poolObject.Add(new PoolData(handle.Result, path).SetActive(active));
                callback?.Invoke(handle.Result);
            });
        }

        /// <summary>
        /// 从外界添加物体到对象池
        /// </summary>
        /// <param name="go"></param>
        public void AddNewToPoolFromOutside(GameObject go)
        {
            PoolData d = new PoolData(go,null).SetActive(true);
            go.transform.SetParentZero(parentRoot);
            go.OnActive(false);
            poolObject.Add(d);
        }

        public int GetActivePoolCount()
        {
            int ret = 0;
            for (int i = poolObject.Count - 1; i >= 0; i--)
            {
                PoolData temp = poolObject[i];
                if (CheckNull(temp, i)) continue;
                if (temp.active)
                    ret++;
            }
            return ret;
        }
        
        #endregion

        #endregion

    }
}