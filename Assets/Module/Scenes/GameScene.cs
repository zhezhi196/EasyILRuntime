using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Module
{
    public class GameScene : IEnumerator
    {
        #region Static

        private static Dictionary<string, GameScene> m_scene = new Dictionary<string, GameScene>();
        private static GameScene _currActiveScene;
        public static int refCount;

        public static GameScene activeScene
        {
            get { return _currActiveScene; }
            set
            {
                _currActiveScene = value;
                SceneManager.SetActiveScene(_currActiveScene.m_request.Result.Scene);
            }
        }

        public static void UnLoad(string name,Action callback)
        {
            GameScene scene = null;
            if (m_scene.TryGetValue(name, out scene))
            {
                scene.UnLoad(callback);
            }
        }

        public static void UnLoadCurrent(Action callback = null)
        {
            if (activeScene != null)
            {
                activeScene.UnLoad(callback);
            }
        }
        
        #endregion

        #region Load
        
        public static GameScene LoadWithLoading<T>(string name, Action callback) where T : Loading, new()
        {
            Loading.Load<T>();
            GameScene target = Load(name, () =>
            {
                callback?.Invoke();
                Loading.Close<T>();
            });
            return target;
        }

        public static GameScene LoadAdditiveWithLoading<T>(string name, bool newScene, Action callback) where T : Loading, new()
        {
            Loading.Load<T>();
            GameScene target = LoadAdditive(name, () =>
            {
                callback?.Invoke();
                Loading.Close<T>();
            });
            return target;
        }

        public static GameScene Load(string name, Action callback)
        {
            GameScene target = null;
            target = LoadPrivate(name, () =>
            {
                foreach (KeyValuePair<string,GameScene> keyValuePair in m_scene)
                {
                    if (keyValuePair.Value.isLoaded && keyValuePair.Value != target)
                    {
                        keyValuePair.Value.isLoaded = false;
                    }
                }
                
                callback?.Invoke();
            }, LoadSceneMode.Single);
            refCount = 1;
            return target;
        }

        public static GameScene LoadAdditive(string name, Action callback)
        {
            return LoadPrivate(name, callback,LoadSceneMode.Additive);
        }

        private static GameScene LoadPrivate(string name, Action callback,LoadSceneMode mode)
        {
            if (activeScene != null && activeScene.name == name) return null;
            GameDebug.LogFormat("开始加载场景{0}" , name);
            GameScene scene = null;
            if (!m_scene.TryGetValue(name, out scene))
            {
                scene = new GameScene(name);
                m_scene.Add(name, scene);
            }

            if (scene.isLoaded)
            {
                OnLoadComplete(scene, callback);
                return scene;
            }

            UICommpont.FreezeUI("LoadScene");
            scene.Load(() => OnLoadComplete(scene, callback), mode);

            return scene;
        }

        private static void OnLoadComplete(GameScene scene,Action callback)
        {
            activeScene = scene;
            callback?.Invoke();
            UICommpont.UnFreezeUI("LoadScene");
        }

        #endregion

        #region 属性,字段,构造

        private AsyncOperationHandle<SceneInstance> m_request;

        /// <summary>
        /// 场景名字
        /// </summary>
        public string name { get; }
        
        public object Current { get; }
        public GameScene(string name)
        {
            this.name = name;
        }

        public bool isLoaded;

        #endregion

        #region Load

        private void Load(Action callback,LoadSceneMode mode)
        {
            refCount++;
            string path = AssetLoad.GetFolderFilePath(name + ".unity", AssetLoad.AssetFolderType.Scenes);
            m_request = Addressables.LoadSceneAsync(AssetLoad.GetAssetsPath(path), mode);
            m_request.Completed += ass =>
            {
                if (mode == LoadSceneMode.Single)
                {
                    ObjectPool.ClearNullDic();
                }
                isLoaded = true;
                callback?.Invoke();
            };
        }

        #endregion

        #region Unload

        public void UnLoad(Action callback)
        {
            refCount--;
            Addressables.UnloadSceneAsync(m_request).Completed += a =>
            {
                ObjectPool.ClearNullDic();
                isLoaded = false;
                callback?.Invoke();
            };
        }

        #endregion

        #region 其他

        public bool MoveNext()
        {
            return !m_request.IsDone;
        }

        public void Reset()
        {
        }

        #endregion
    }
}