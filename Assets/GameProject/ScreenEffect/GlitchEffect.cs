using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Module;
using DG.Tweening;
/// <summary>
/// 故障效果后处理Volume
/// </summary>
[Serializable, VolumeComponentMenu("ScreenEffect/GlitchEffect")]
public class GlitchEffect : VolumeComponent, IPostProcessComponent
{
    public BoolParameter enableEffect = new BoolParameter(true);
    //public FloatParameter value = new FloatParameter(1);
    [Range(0f, 1f), Tooltip("全息投影的扫描线抖动")]
    public FloatParameter scanLineJitter = new FloatParameter(0.17f);

    [Range(0f, 1f), Tooltip("全息投影的扫描线颜色抖动")]
    public FloatParameter colorDrift = new FloatParameter(0f);

    //[Range(0f, 100f), Tooltip("全息投影的错误视频速度")]
    //public FloatParameter speed = new FloatParameter(0f);

    //[Range(0f, 100f), Tooltip("全息投影的错误视频方块大小")]
    //public FloatParameter blockSize = new FloatParameter(5f);

    public bool IsActive()
    {
        return enableEffect.value;
    }

    public bool IsTileCompatible()
    {
        return false;
    }
}


