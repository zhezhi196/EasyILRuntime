using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Module;
using UnityEngine;
using xasset;
using Object = UnityEngine.Object;

namespace Module
{
    public class BundleManager : Manager
    {
        private RunTimeSequence runtime;
        private Dictionary<string, string> _versions = new Dictionary<string, string>();
        private Dictionary<string, string> _serverVersions = new Dictionary<string, string>();
        private readonly Queue<Download> downLoadQueue = new Queue<Download>();
        private Download currentDownload;
        private static Dictionary<string, ObjectPool> objectPool = new Dictionary<string, ObjectPool>();
        private int allItem;
        protected override int runOrder { get; } = -999;

        protected override string processDiscription
        {
            get { return "资源下载完毕"; }
        }

        protected override void BeforeInit()
        {
        }

        protected override void Init(RunTimeSequence runtime)
        {
            EventCenter.Dispatch(EventKey.BundleInitStart);
            this.runtime = runtime;
            Versions.Load();
            CheckVersion();
        }

        private void CheckVersion()
        {
            Assets.Initialize(() =>
            {
                if (Assets.source == ResourceSource.InnerBundle)
                {
                    //从工程进,直接进
                    runtime.NextAction();
                    return;
                }

                //拿到persistentDataPath的MD5码
                string path = Assets.GetRelativeUpdatePath(Versions.versionText);
                if (!File.Exists(path))
                {
                    //从streamAsset拿MD5码
                    Assets.LoadAsync(Assets.GetAssetBundleDataPathURL(Versions.versionText), typeof(TextAsset))
                        .onComplete += asset =>
                    {
                        if (asset.error != null)
                        {
                            LoadVersions(string.Empty);
                            return;
                        }

                        //把streamAsset的version文件拷贝到persistentDataPath
                        string dir = Path.GetDirectoryName(path);
                        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                        File.WriteAllText(path, asset.text);
                        LoadVersions(asset.text);
                        asset.Release();
                    };
                }
                else
                {
                    LoadVersions(File.ReadAllText(path));
                }
            }, error => runtime.NextAction());
        }

        private void Download()
        {
            currentDownload = downLoadQueue.Dequeue();
            currentDownload.Start();
        }

        private void Update()
        {
            if (currentDownload == null)
            {
                if (downLoadQueue.Count == 0)
                {
                    Versions.Save();
                    Complete();

                    return;
                }
                else
                {
                    Download();
                }
            }

            currentDownload.Update();

            EventCenter.Dispatch(EventKey.BundleProcess, (float) (allItem - downLoadQueue.Count) / allItem);
            float loading = UIComponent.SetLoading(this.GetType().FullName,
                $"正在下载资源: {(allItem - downLoadQueue.Count)}/{allItem}",
                (float) (allItem - downLoadQueue.Count) / allItem);
            if (currentDownload.isDone)
            {
                SaveDownload(currentDownload);
                currentDownload = null;
                GameDebug.Log($"loading process: {allItem - downLoadQueue.Count}/{allItem}");
            }
        }

        private void Complete()
        {
            EventCenter.UnRegister(EventKey.Update, Update);
            runtime.NextAction();
            EventCenter.Dispatch(EventKey.BundleInitComplete);
        }

        private void SaveDownload(Download downLoad)
        {
            if (!downLoad.isDone) return;
            if (_serverVersions.ContainsKey(downLoad.path))
            {
                _versions[downLoad.path] = _serverVersions[downLoad.path];
            }

            StringBuilder sb = new StringBuilder();
            foreach (var item in _versions)
            {
                sb.AppendLine(string.Format("{0}:{1}", item.Key, item.Value));
            }

            string path = Assets.GetRelativeUpdatePath(Versions.versionText);

            if (File.Exists(path))
            {
                StreamWriter writer = File.CreateText(path);
                writer.Write(sb.ToString());
                writer.Close();
            }
            else
            {
                File.WriteAllText(path, sb.ToString());
            }
        }

        private void LoadVersions(string text)
        {
            //把本地的version写入字典
            ReadVersionToMap(text, ref _versions);
            //获取服务器的version
            Assets.LoadAsync(Assets.GetDownloadURL(Versions.versionText), typeof(TextAsset)).onComplete += asset =>
            {
                if (asset.error != null)
                {
                    GameDebug.Log("热更服务器出现错误");
                    return;
                }

                //把服务器的version写入字典
                ReadVersionToMap(asset.text, ref _serverVersions);
                foreach (var item in _serverVersions)
                {
                    string ver;
                    //如果本地和服务器不一样或者本地不存在
                    if (!_versions.TryGetValue(item.Key, out ver) || !ver.Equals(item.Value))
                    {
                        Download downloader = new Download();
                        downloader.url = Assets.GetDownloadURL(item.Key);
                        downloader.path = item.Key;
                        downloader.version = item.Value;
                        downloader.savePath = Assets.GetRelativeUpdatePath(item.Key);
                        downLoadQueue.Enqueue(downloader);
                    }
                }

                if (downLoadQueue.Count != 0)
                {
                    Download downloader = new Download();
                    downloader.url = Assets.GetDownloadURL(Assets.platform);
                    downloader.path = Assets.platform;
                    downloader.savePath = Assets.GetRelativeUpdatePath(Assets.platform);
                    downLoadQueue.Enqueue(downloader);
                    EventCenter.Register(EventKey.Update, Update);
                    allItem = downLoadQueue.Count;
                    Download();
                }
                else
                {
                    Complete();
                }
            };
        }

        private void ReadVersionToMap(string text, ref Dictionary<string, string> map)
        {
            map.Clear();
            if (text == null) text = String.Empty;
            using (var reader = new StringReader(text))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var fields = line.Split(':');
                    if (fields.Length > 1)
                    {
                        map.Add(fields[0], fields[1]);
                    }
                }
            }
        }

        #region 加载 预加载

        public static ObjectPool PreLoad(string path, Action<Object> onLoad)
        {
            ObjectPool pool = null;
            if (!objectPool.TryGetValue(path, out pool))
            {
                pool = new ObjectPool(path);
                objectPool.Add(path, pool);
            }

            if (!pool.isActive)
            {
                Assets.LoadAsync(GetAssetPath(path), typeof(GameObject)).onComplete += asset =>
                {
                    GameObject ass = (GameObject) asset.asset;
                    pool.InitPrefab(path, ass);
                    onLoad?.Invoke(ass);
                };
            }
            else
            {
                onLoad?.Invoke(pool.prefab);
            }

            return pool;
        }

        public static ObjectPool LoadGameoObject<T>(string path, Action<T> onLoad) where T : IPoolObject
        {
            return LoadGameoObject(path, (go) => { onLoad?.Invoke(go.GetComponent<T>()); });
        }

        public static ObjectPool LoadGameoObject(string path, Action<GameObject> onLoad)
        {
            ObjectPool pool = null;
            if (!objectPool.TryGetValue(path, out pool))
            {
                pool = new ObjectPool(path);
                objectPool.Add(path, pool);
            }

            if (!pool.isActive)
            {
                Assets.LoadAsync(GetAssetPath(path), typeof(GameObject)).onComplete += asset =>
                {
                    if (asset.asset == null)
                    {
                        GameDebug.LogError("Connot find prefab :" + path);
                        return;
                    }

                    pool.InitPrefab(path, (GameObject) (asset.asset));
                    pool.InvokeAllCacheAction();
                    pool.GetObject(onLoad);
                };
            }
            else
            {
                pool.GetObject(onLoad);
            }

            return pool;
        }

        public static void LoadObject<T>(string path, Action<T> callback) where T : Object
        {
            Assets.LoadAsync(path, typeof(T)).onComplete += asset => { callback?.Invoke((T) asset.asset); };
        }

        private static string GetAssetPath(string path)
        {
            return "Assets/Bundles/" + path;
        }

        #endregion
    }
}