using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;

[Serializable, VolumeComponentMenu("ScreenEffect/GlitchEffect")]
public class PointLightEffect : VolumeComponent, IPostProcessComponent
{
    public BoolParameter enableEffect = new BoolParameter(true);

    public bool IsActive()
    {
        return enableEffect.value;
    }

    public bool IsTileCompatible()
    {
        return false;
    }

}
