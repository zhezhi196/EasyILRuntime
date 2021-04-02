using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;


namespace Module
{
    /// <summary>
    /// 资源管理器
    /// </summary>
    public static class AssetLoad
    {
        public enum AssetFolderType
        {
            Scenes,
            Config,
            UI,
        }
        public static void PreloadAsset<T>(string path, Action<AsyncOperationHandle<T>> callback)
        {
            Addressables.LoadAssetAsync<T>(GetAssetsPath(path)).Completed += callback;
        }

        public static void PreloadGameobject(string path, Action<ObjectPool, object[]> callback, params object[] args)
        {
            ObjectPool pool = ObjectPool.GetPool(path, null);
            pool.onComplete += () => callback?.Invoke(pool, args);
        }

        #region LoadGameObject

        public static ObjectPool LoadGameObject<T>(string path, Transform parent, Action<T, object[]> callback, params object[] args) where T : Object
        {
            return LoadGameObject(path, parent, (a, b) => callback?.Invoke(a.GetComponent<T>(), b), 0, args);
        }

        public static ObjectPool LoadGameObject(string path, Transform parent, Action<GameObject, object[]> callback, params object[] args)
        {
            return LoadGameObject(path, parent, callback, 0, args);
        }

        public static ObjectPool LoadGameObject<T>(string path, Transform parent, Action<T, object[]> callback, PoolFlag flag, params object[] args) where T: Object
        {
            return LoadGameObject(path, parent, (a, b) => callback?.Invoke(a.GetComponent<T>(), b), flag, args);
        }

        public static ObjectPool LoadGameObject(string path, Transform parent, Action<GameObject, object[]> callback, PoolFlag flag, params object[] args)
        {
            ObjectPool pool = ObjectPool.GetPool(path, parent);
            pool.GetObject(parent, result => callback?.Invoke(result, args), flag);
            return pool;
        }

        #endregion

        public static void Release<T>(AsyncOperationHandle<T> handle)
        {
            Addressables.Release(handle);
        }
        public static void Release<T>(T handle)
        {
            Addressables.Release(handle);
        }

        public static void Destroy(GameObject go)
        {
            if(go==null) return;
            ObjectPool.TryRemove(go);
            Object.Destroy(go);
        }

        public static T Instantiate<T>(T prefab) where T : Object
        {
            return GameObject.Instantiate<T>(prefab);
        }
        
        public static T Instantiate<T>(T prefab,Transform parent) where T : Object
        {
            return GameObject.Instantiate<T>(prefab,parent);
        }

        public static GameObject Instantiate(GameObject prefab)
        {
            return GameObject.Instantiate(prefab);
        }

        public static GameObject Instantiate(GameObject prefab, Transform parent)
        {
            return GameObject.Instantiate(prefab, parent);
        }

        public static void Destroy(Object obj, bool release = true)
        {
            if (release)
            {
                Addressables.Release(obj);
            }
            Object.Destroy(obj);
        }
        
        public static string GetAssetsPath(string path)
        {
            return "Assets/Bundles/" + path;
        }

        public static string GetFolderFilePath(string name, AssetFolderType type)
        {
           return string.Join("/", GetFolder(type), name);
        }

        public static string GetFolder(AssetFolderType type)
        {
            switch (type)
            {
                case AssetFolderType.Config:
                    return "Config";
                case AssetFolderType.Scenes:
                    return "Scenes";
                case AssetFolderType.UI:
                    return "UI";
            }

            return string.Empty;
        }
    }
}
