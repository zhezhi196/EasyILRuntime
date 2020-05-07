using System;
using System.Collections;
using System.Collections.Generic;
using Module;
using UnityEngine;
using xasset;

namespace Module
{
    public class SceneManager : Manager
    {
        protected override int runOrder { get; }

        protected override string processDiscription
        {
            get { return "场景初始化完成"; }
        }

        private string currentScene;
        protected override void BeforeInit()
        {
        }

        private void OnResourceInit()
        {
        }

        protected override void Init(RunTimeSequence runtime)
        {
            runtime.NextAction();
        }

        public SceneAsset LoadScene(string sceneName, Action<SceneAsset> callBack)
        {
            if (currentScene == sceneName) return null;
            currentScene = sceneName;
            return Load(sceneName, callBack);
        }

        public SceneAsset ReloadScene(Action<SceneAsset> callBack)
        {
            return Load(currentScene,callBack);
        }

        private SceneAsset Load(string sceneName, Action<SceneAsset> callBack)
        {
            Assets.UnloadScene(GetPath(currentScene));
            SceneAsset scene = Assets.LoadScene(GetPath(sceneName), true, false);
            scene.onComplete += asset => callBack?.Invoke(scene);
            return scene;
        }

        private string GetPath(string sceneName)
        {
            return $"Assets/Bundles/Scene/{sceneName}.unity";
        }
    }

}

