using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
/// <summary>
/// 故障效果后处理RenderPass
/// </summary>
public class GlitchEffectPass : ScriptableRenderPass
{
    static readonly string k_RenderTag = "Glitch Effects";
    //static readonly int _MainTexId = Shader.PropertyToID("_MainTex");
    //static readonly int _BlockSizeId = Shader.PropertyToID("_BlockSize");
    //static readonly int _SpeedId = Shader.PropertyToID("_Speed");
    static readonly int _Params = Shader.PropertyToID("_Params");
    private RenderTargetHandle destination { get; set; }
    public int blitShaderPassIndex = 0;
    [Sirenix.OdinInspector.ReadOnly]
    public Material effectMat;
    GlitchEffect glitchEffect;
    RenderTargetIdentifier currentTarget;
    RenderTargetHandle m_temporaryColorTexture;
    public FilterMode filterMode { get; set; }

    public GlitchEffectPass()
    {
        var shader = Shader.Find("ScreenEffect/HologramBlockPE");
        effectMat = CoreUtils.CreateEngineMaterial(shader);
        //m_temporaryColorTexture.Init("temporaryColorTexture");
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (!renderingData.cameraData.postProcessEnabled) return;
        //通过队列来找到HologramBlock组件，然后
        var stack = VolumeManager.instance.stack;
        glitchEffect = stack.GetComponent<GlitchEffect>();
        if (glitchEffect == null) { return; }
        if (!glitchEffect.IsActive()) return;
        var cmd = CommandBufferPool.Get(k_RenderTag);
        //UnityEngine.Debug.LogError("Execute渲染中！");
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.isSceneViewCamera) return;
        var source = currentTarget;

        //effectMat.SetFloat(_BlockSizeId, glitchEffect.blockSize.value);
        //effectMat.SetFloat(_SpeedId, glitchEffect.speed.value);
        var sl_thresh = Mathf.Clamp01(1.0f - glitchEffect.scanLineJitter.value * 1.2f);
        var sl_disp = 0.002f + Mathf.Pow(glitchEffect.scanLineJitter.value, 3) * 0.05f;
        //var cd = new Vector2(glitchEffect.colorDrift.value * 0.04f, Time.time * 606.11f);
        var cd = new Vector2(glitchEffect.colorDrift.value * 0.04f,606.11f);
        effectMat.SetVector(_Params, new Vector4(sl_disp, sl_thresh, cd.x, cd.y));

        RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
        opaqueDesc.depthBufferBits = 0;
        //不能读写同一个颜色target，创建一个临时的render Target去blit
        if (destination == RenderTargetHandle.CameraTarget)
        {
            cmd.GetTemporaryRT(m_temporaryColorTexture.id, opaqueDesc, filterMode);
            Blit(cmd, source, m_temporaryColorTexture.Identifier(), effectMat, blitShaderPassIndex);
            Blit(cmd, m_temporaryColorTexture.Identifier(), source);
        }
        else
        {
            Blit(cmd, source, destination.Identifier(), effectMat, blitShaderPassIndex);
        }
    }

    public override void FrameCleanup(CommandBuffer cmd)
    {
        if (destination == RenderTargetHandle.CameraTarget)
            cmd.ReleaseTemporaryRT(m_temporaryColorTexture.id);
    }

    public void Setup(in RenderTargetIdentifier currentTarget, RenderTargetHandle dest)
    {
        this.destination = dest;
        this.currentTarget = currentTarget;
    }
}
