using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Module;
using UnityEngine;
using xasset;
using Object = UnityEngine.Object;

namespace HotFix
{
    public class ResourceManager : Manager
    {
        protected override int runOrder { get; } = -2;

        protected override string processDiscription
        {
            get { return "资源初始化完成"; }
        }

        private BundleManager bundleManager;


        protected override void BeforeInit()
        {
        }

        protected override void Init(RunTimeSequence runtime)
        {
            ObjectPool.poolRoot.transform.SetParent(GamePlay.Instance.transform);
            bundleManager = BundleManager.Get<BundleManager>();
            runtime.NextAction();
        }

        #region 加载 预加载

        public static ObjectPool PreLoad(string path, Action<Object> onLoad)
        {
            return BundleManager.PreLoad(path, onLoad);
        }

        public static GameObject Instantiate(GameObject prefab, Transform parent)
        {
            if (prefab == null)
            {
                Debug.LogError("prefab 不能为 null");
                return default;
            }
            
            GameObject go = GameObject.Instantiate(prefab, parent);
            ViewReference[] views = go.transform.GetComponentsInChildren<ViewReference>();
            for (int i = 0; i < views.Length; i++)
            {
                HotFixMonoBehaviour.CreateMonoBehaviour(views[i]);
            }

            return go;
        }
        
        public static ObjectPool LoadPrefab(string path,Transform parent, Action<GameObject> onLoad)
        {
            return BundleManager.LoadPrefab(path, go =>
            {
                ViewReference[] views = go.transform.GetComponentsInChildren<ViewReference>();
                for (int i = 0; i < views.Length; i++)
                {
                    HotFixMonoBehaviour.CreateMonoBehaviour(views[i]);
                }
                onLoad?.Invoke(go);
            });
        }

        public static void LoadObject<T>(string path, Action<T> callback) where T : Object
        {
            BundleManager.LoadObject(path,callback);
        }

        #endregion
    }
}