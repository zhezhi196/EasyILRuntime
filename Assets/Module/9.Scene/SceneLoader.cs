using System;
using System.Collections;
using System.Collections.Generic;
using Module;
using UnityEngine;
using xasset;

namespace Module
{
    public static class SceneLoader
    {
        private static string currentScene;
        public static SceneAsset LoadScene(string sceneName, Action<SceneAsset> callBack)
        {
            if (currentScene == sceneName) return null;
            currentScene = sceneName;
            return Load(sceneName, callBack);
        }

        public static SceneAsset ReloadScene(Action<SceneAsset> callBack)
        {
            return Load(currentScene,callBack);
        }

        private static SceneAsset Load(string sceneName, Action<SceneAsset> callBack)
        {
            Assets.UnloadScene(GetPath(currentScene));
            SceneAsset scene = Assets.LoadScene(GetPath(sceneName), true, false);
            scene.onComplete += asset => callBack?.Invoke(scene);
            return scene;
        }

        private static string GetPath(string sceneName)
        {
            return $"Assets/Bundles/Scene/{sceneName}.unity";
        }
    }

}

