using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Video;
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
            Effect,
            DB,
            Bundle,
            Altas,
            WordsCheck,
            Analytics,
            Material,
            Video
        }

        #region LoadAsset ReleaseAsset

        private static Dictionary<string, AsyncOperationHandle> assetCache = new Dictionary<string, AsyncOperationHandle>();
        
        public static AsyncOperationHandle<T> PreloadAsset<T>(string path, Action<AsyncOperationHandle<T>> callback)
        {
            var load = Addressables.LoadAssetAsync<T>(GetAssetsPath(path));
            assetCache.SetOrAdd(path, load);
            load.Completed += callback;
            return load;
        }

        public static void Release<T>(AsyncOperationHandle<T> handle)
        {
            if (handle.IsValid())
                Addressables.Release(handle);
        }

        public static void Release(string path)
        {
            AsyncOperationHandle handle;
            if (assetCache.TryGetValue(path, out handle))
            {
                if (handle.IsValid())
                    Addressables.Release(handle);
            }
        }

        #endregion
        
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

        public static ObjectPool GetPool(string path, Transform parent)
        {
            ObjectPool pool = ObjectPool.GetPool(path, parent);
            return pool;
        }

        #endregion

        #region Destroy

        /// <summary>
        /// 销毁资源
        /// </summary>
        /// <param name="obj"></param>
        public static void Destroy(Object obj)
        {
            if (obj is GameObject go)
            {
                ObjectPool.RemoveObjectFromPool(go);
                Addressables.ReleaseInstance(go);
            }
            else if (!(obj is Component))
            {
                Addressables.Release(obj);
            }
            Object.Destroy(obj);
        }
        
        public static void DestroyImmediate(GameObject obj)
        {
            if (obj is GameObject go)
            {
                ObjectPool.RemoveObjectFromPool(go);
                Addressables.ReleaseInstance(go);
            }
            else if (!(obj is Component))
            {
                Addressables.Release(obj);
            }
            Object.DestroyImmediate(obj);
        }

        #endregion

        #region Instantiate

        public static void Instantiate(string path, Transform parent, Action<AsyncOperationHandle<GameObject>> callback)
        {
            var temp = Addressables.InstantiateAsync(GetAssetsPath(path), parent);
            assetCache.SetOrAdd(path, temp);
            temp.Completed += callback;
        }

        #endregion

        #region Path

        public static string GetAssetsPath(string path)
        {
            return $"Assets/{ConstKey.GetFolder(AssetFolderType.Bundle)}/{path}";
        }

        public static string GetFolderFilePath(string name, AssetFolderType type)
        {
            return string.Join("/", ConstKey.GetFolder(type), name);
        }

        #endregion
    }
}
