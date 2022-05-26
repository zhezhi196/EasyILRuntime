using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PointLightPass : ScriptableRenderPass
{
    static readonly string k_RenderTag = "PointLightEffects";
    private RenderTargetHandle destination { get; set; }
    public int blitShaderPassIndex = 0;
    public Material effectMat;
    PointLightEffect lightEffect;
    RenderTargetIdentifier currentTarget;
    RenderTargetHandle m_temporaryColorTexture;
    public FilterMode filterMode { get; set; }

    public PointLightPass()
    {
        var shader = Shader.Find("Custom/FakePointLight");
        effectMat = CoreUtils.CreateEngineMaterial(shader);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (!renderingData.cameraData.postProcessEnabled) return;
        var stack = VolumeManager.instance.stack;
        lightEffect = stack.GetComponent<PointLightEffect>();
        if (lightEffect == null) { return; }
        if (!lightEffect.IsActive()) return;
        var cmd = CommandBufferPool.Get(k_RenderTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.isSceneViewCamera) return;
        var source = currentTarget;
        //设置参数
        RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
        opaqueDesc.depthBufferBits = 0;
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

    public void Setup(in RenderTargetIdentifier currentTarget, RenderTargetHandle dest)
    {
        this.destination = dest;
        this.currentTarget = currentTarget;
    }
}
