/*
 * 脚本名称：CameraController
 * 项目名称：Bow
 * 脚本作者：黄哲智
 * 创建时间：2018-09-05 17:29:41
 * 脚本作用：
*/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public static class CameraProcessing
    {
        #region 私有字段

        private static Dictionary<PostProcessID, PostProcessingBase>
            m_effDic = new Dictionary<PostProcessID, PostProcessingBase>();

        private static Dictionary<string, GameObject> m_prefab = new Dictionary<string, GameObject>();

        #endregion

        #region 公有字段

        public  static Dictionary<string, GameCamera> cameraList = new Dictionary<string, GameCamera>();

        public static PostProcessingBase currentEffect { get; private set; }

        public static bool isPlaying { get; private set; }

        #endregion
        
        #region 特效的play方法

        public static void Play(GameCamera[] effect, PostProcessID name, params object[] args)
        {
            if (currentEffect == m_effDic[name]) return;
            if (isPlaying) return;
            isPlaying = true;

            currentEffect = m_effDic[name];
            for (int i = 0; i < effect.Length; i++)
            {
                effect[i].enabled = true;
                currentEffect.cameraList.Add(effect[i]);
            }

            currentEffect.Play(args);
        }

        public static void Play(GameCamera effect, PostProcessID name, params object[] args)
        {
            if (currentEffect == m_effDic[name]) return;
            if (isPlaying) return;
            isPlaying = true;

            currentEffect = m_effDic[name];
            effect.enabled = true;
            currentEffect.cameraList.Add(effect);
            currentEffect.Play(args);
        }

        public static void Play(string[] effect, PostProcessID name, params object[] args)
        {
            if (currentEffect == m_effDic[name]) return;
            if (isPlaying) return;
            isPlaying = true;

            currentEffect = m_effDic[name];
            for (int i = 0; i < effect.Length; i++)
            {
                GameCamera camera = cameraList[effect[i]];
                camera.enabled = true;
                currentEffect.cameraList.Add(camera);
            }

            currentEffect.Play(args);
        }

        public static void Play(string effect, PostProcessID name, params object[] args)
        {
            if (currentEffect == m_effDic[name]) return;
            if (isPlaying) return;
            isPlaying = true;

            currentEffect = m_effDic[name];
            GameCamera camera = cameraList[effect];
            camera.enabled = true;
            currentEffect.cameraList.Add(camera);
            currentEffect.Play(args);
        }

        public static void Stop()
        {
            currentEffect.Stop();
            ClearEffect();
            isPlaying = false;
        }

        #endregion

        #region  关于摄像机特效的一些方法

        public static void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (currentEffect == null) return;

            currentEffect.OnRenderImage(source, destination);
            if (currentEffect.isComplete) Stop();
        }

        private static void ClearEffect()
        {
            for (int i = 0; i < currentEffect.cameraList.Count; i++)
            {
                currentEffect.cameraList[i].enabled = false;
            }

            currentEffect.cameraList.Clear();
            currentEffect = null;
        }

        #endregion

        #region Get方法

        public static T GetPostProcessing<T>(PostProcessID name) where T : PostProcessingBase
        {
            return m_effDic[name] as T;
        }

        public static T GetCameraPrefab<T>(string name, Transform parent) where T : Component
        {
            GameObject ob = m_prefab[name];

            T obj = GameObject.Instantiate(ob, parent, false).GetComponent<T>();
            return obj;
        }

        public static T GetCameraPrefab<T>(string name, Vector3 pos) where T : Component
        {
            GameObject ob = m_prefab[name];
            T obj = GameObject.Instantiate(ob, pos, Quaternion.identity).GetComponent<T>();
            return obj;
        }

        #endregion
    }
}