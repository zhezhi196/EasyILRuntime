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

namespace Module
{
    [Flags]
    public enum PoolFlag
    {
        NotFromPool = 1 << 0,
        NotAutoActive = 1 << 1,
    }

    public class ObjectPool : IProcess
    {
        public static readonly Dictionary<string, ObjectPool> poolDic = new Dictionary<string, ObjectPool>();

        /// <summary>
        /// 所有动态生成的物品都在这里
        /// </summary>
        private static Dictionary<GameObject, string> instateObject = new Dictionary<GameObject, string>();

        public static void ClearNullDic()
        {
            Dictionary<GameObject, string> mirror = new Dictionary<GameObject, string>(instateObject);
            foreach (KeyValuePair<GameObject,string> keyValuePair in instateObject)
            {
                if (keyValuePair.Key == null)
                {
                    mirror.Remove(keyValuePair.Key);
                }
            }

            instateObject = mirror;
        }
        
        #region PoolData

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

        private Queue<(Action<GameObject>, Transform, PoolFlag)> cacheAction =
            new Queue<(Action<GameObject>, Transform, PoolFlag)>();

        public GameObject prefab;
        private Transform _parentRoot;
        private bool isIPoolObject;
        private List<PoolData> poolObject = new List<PoolData>();
        public event Action onComplete;

        #endregion

        #region public

        public Func<bool> listener { get; set; }

        public bool isComplete
        {
            get { return prefab != null; }
        }

        public void SetListener(Func<bool> listen)
        {
            this.listener = listen;
        }

        public bool MoveNext()
        {
            return !isComplete;
        }

        public object Current
        {
            get { return prefab; }
        }

        public string path { get; }

        public Transform parentRoot
        {
            get
            {
                if (_parentRoot == null || _parentRoot.gameObject == null) return poolRoot;
                return _parentRoot;
            }
        }

        #endregion

        #region 构造函数

        public static ObjectPool GetPool(string path, Transform root)
        {
            ObjectPool pool = null;
            if (!poolDic.TryGetValue(path, out pool))
            {
                pool = new ObjectPool(path, root);
                pool.InitPrefab(null);
                poolDic.Add(path, pool);
            }

            return pool;
        }

        public static ObjectPool GetPool(string path, Transform root, GameObject prefab)
        {
            ObjectPool pool = null;
            if (!poolDic.TryGetValue(prefab.name, out pool))
            {
                pool = new ObjectPool(path, root);
                pool.InitPrefab(prefab);
                poolDic.Add(path, pool);
            }

            return pool;
        }

        public static void ReturnToPool(GameObject go)
        {
            string path = null;
            if (instateObject.TryGetValue(go, out path))
            {
                ObjectPool pool = null;
                if (poolDic.TryGetValue(path, out pool))
                {
                    pool.ReturnObject(go);
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

        private ObjectPool(string path, Transform root)
        {
            this.path = path;
            _parentRoot = root;
        }

        private void InitPrefab(GameObject p)
        {
            if (p == null)
            {
                AssetLoad.PreloadAsset<GameObject>(path, pre =>
                {
                    if (pre.Result == null)
                    {
                        GameDebug.LogError(path);
                        return;
                    }

                    prefab = pre.Result;
                    isIPoolObject = prefab.GetComponent<IPoolObject>() != null;
                    int index = cacheAction.Count;
                    for (int i = 0; i < index; i++)
                    {
                        var temp = (cacheAction.Dequeue());
                        GetObjectInternal(temp.Item1, temp.Item2, temp.Item3);
                    }

                    onComplete?.Invoke();
                    onComplete = null;
                });
            }
            else
            {
                prefab = p;
                isIPoolObject = prefab.GetComponent<IPoolObject>() != null;
                onComplete?.Invoke();
                onComplete = null;
            }
        }

        #endregion

        #region Set

        public static void TryRemove(GameObject go)
        {
            ObjectPool pool = null;
            string path = TryGetPathRemove(go);
            if (path != null && poolDic.TryGetValue(path, out pool))
            {
                pool.RemoveFromPool(go);
            }
        }
        public static string TryGetPathRemove(GameObject go)
        {
            string path = null;
            if (instateObject.TryGetValue(go, out path))
            {
                instateObject.Remove(go);
                return path;
            }

            return path;
        }
        public void Reset()
        {
            prefab = null;
        }

        private GameObject AddNewToPool(string path, bool active, Transform parent)
        {
            GameObject go = AssetLoad.Instantiate(prefab, parent);
            instateObject.Add(go, path);
            poolObject.Add(new PoolData(go, path).SetActive(active));
            return go;
        }

        public void RemoveFromPool(GameObject go)
        {
            for (int i = 0; i < poolObject.Count; i++)
            {
                PoolData temp = poolObject[i];
                if (temp.gameObject == go)
                {
                    poolObject.RemoveAt(i);
                    break;
                }
            }

            if (poolObject.Count == 0)
            {
                DestroyPool();
            }
        }

        public void DestroyPool()
        {
            poolDic.Remove(path);
            AssetLoad.Release(prefab);
        }

        public void SetDefaultCount(int count, Transform parent, bool autoActive = true)
        {
            if (!isComplete) return;
            for (int i = 0; i < count; i++)
            {
                if (path != null)
                {
                    GameObject go = AddNewToPool(path, true, parent);
                    if (autoActive && go.activeInHierarchy) go.OnActive(false);
                }
            }
        }

        public void SetDefaultCount(int count, bool autoActive = true)
        {
            SetDefaultCount(count, parentRoot, autoActive);
        }

        #endregion

        #region GetObject

        public T GetObject<T>(PoolFlag flag = 0)
        {
            return GetObject(parentRoot, flag).GetComponent<T>();
        }

        public GameObject GetObject(PoolFlag flag = 0)
        {
            return GetObject(parentRoot, flag);
        }

        public T GetObject<T>(Transform parent, PoolFlag flag = 0)
        {
            return GetObject(parent, flag).GetComponent<T>();
        }

        public GameObject GetObject(Transform parent, PoolFlag flag = 0)
        {
            if (!isComplete)
            {
                GameDebug.LogError("ObjectPool is invaid");
                return default;
            }

            GameObject result = default;
            GetObjectInternal(go => result = go, parent, flag);
            return result;
        }
        
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

        // ReSharper disable Unity.PerformanceAnalysis
        private void GetObjectInternal(Action<GameObject> callBack, Transform parent, PoolFlag flag)
        {
            if (!isComplete)
            {
                cacheAction.Enqueue((callBack, parent, flag));
                return;
            }

            PoolData data = FindActive();
            GameObject returnValue = null;

            if (data != null && !flag.HasFlag(PoolFlag.NotFromPool))
            {
                returnValue = data.SetActive(false).gameObject;
            }
            else
            {
                returnValue = AddNewToPool(path, false, parent);
            }

            if (!flag.HasFlag(PoolFlag.NotAutoActive) && !returnValue.activeInHierarchy)
            {
                returnValue.OnActive(true);
            }

            returnValue.transform.SetParentZero(parent);

            callBack?.Invoke(returnValue);
            if (isIPoolObject)
            {
                IPoolObject target = returnValue.GetComponent<IPoolObject>();
                if (target != null)
                {
                    target.pool = this;
                    target.OnGetObjectFromPool();
                }
            }
        }

        #endregion

        #region ReturnObject

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

            prefab = null;
            poolDic.Remove(path);
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
            if (temp.gameObject == null)
            {
                poolObject.RemoveAt(index);
                return true;
            }

            return false;
        }

        #endregion

        #endregion

    }
}