// Decompiled with JetBrains decompiler
// Type: xasset.SceneAsset
// Assembly: xasset, Version=1.0.7365.4552, Culture=neutral, PublicKeyToken=null
// MVID: 4D869495-BD31-47B9-91C2-89825580533B
// Assembly location: E:\WorkSpace\UnityProject\FrameWork\FrameWork\Assets\Editor\XAsset\xasset.dll

using System.IO;
using UnityEngine.SceneManagement;

namespace xasset
{
    public class SceneAsset : Asset
    {
        protected readonly LoadSceneMode loadSceneMode;
        protected readonly string sceneName;
        public string assetBundleName;
        protected Bundle bundle;

        public SceneAsset(string path, bool addictive)
        {
            name = path;
            sceneName = Path.GetFileNameWithoutExtension(name);
            loadSceneMode = addictive ? LoadSceneMode.Additive : LoadSceneMode.Single;
        }

        public override float progress
        {
            get
            {
                return 1f;
            }
        }

        internal override void Load()
        {
            if (!string.IsNullOrEmpty(assetBundleName))
            {
                bundle = Bundles.Load(assetBundleName);
                if (bundle == null)
                    return;
                SceneManager.LoadScene(sceneName, loadSceneMode);
            }
            else
                SceneManager.LoadScene(sceneName, loadSceneMode);
        }

        internal override void Unload()
        {
            if (bundle != null)
                bundle.Release();
            if (SceneManager.GetSceneByName(sceneName).isLoaded)
                SceneManager.UnloadSceneAsync(sceneName);
            bundle = null;
        }
    }
}