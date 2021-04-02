/*
 * 脚本名称：CameraEffectBase
 * 项目名称：Bow
 * 脚本作者：黄哲智
 * 创建时间：2018-09-05 16:57:00
 * 脚本作用：
*/

using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public enum PostProcessID
    {
        RadiaBlur,
        GoisBulr,
        Bloom,
        WaterWave
    }

    public abstract class PostProcessingBase
    {
        public PostProcessID name { get; protected set; }
        public Material material { get; protected set; }
        public List<GameCamera> cameraList { get; }=new List<GameCamera>();

        public abstract bool isComplete { get;}
        public abstract void OnRenderImage(RenderTexture src, RenderTexture dest);
        public abstract void Play(params object[] args);
        public abstract void Stop();

        public PostProcessingBase(PostProcessID name)
        {
            this.name = name;
            //AssetLoad.PreloadAsset(string.Format(ConstKey.ShaderPath, name), OnLoadShader);
        }

        // private void OnLoadShader(AssetRequest shader, object[] arg2)
        // {
        //     material = GenerateMaterial(((Shader)shader.asset));
        // }

        public Material GenerateMaterial(Shader shader)
        {
            if (shader == null)
                return null;
            //需要判断shader是否支持
            if (shader.isSupported == false)
                return null;
            Material material = new Material(shader);
            material.hideFlags = HideFlags.DontSave;
            if (material)
                return material;
            return null;
        }

    }

}


