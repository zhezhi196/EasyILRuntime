using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 故障效果后处理
/// </summary>
public class GlitchEffectFeature : ScriptableRendererFeature
{
    public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    RenderTargetHandle m_renderTargetHandle;
    GlitchEffectPass effectPass;
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // var cameraColorTarget = renderer.cameraColorTarget;
        // var cameraDepth = renderer.cameraDepth;
        var des = RenderTargetHandle.CameraTarget;
        //把camera渲染的画面传入到GlitchEffectPass
        effectPass.Setup(renderer.cameraColorTarget, des);
        //渲染队列中加入GlitchEffectPass
        renderer.EnqueuePass(effectPass);
    }

    public override void Create()
    {
        effectPass = new GlitchEffectPass();
        effectPass.renderPassEvent = renderPassEvent;
        //m_renderTargetHandle.Init("_ScreenTexture2");
    }
}
