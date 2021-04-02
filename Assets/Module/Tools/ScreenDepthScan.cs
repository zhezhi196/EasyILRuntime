using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    [ExecuteInEditMode]
    public class ScreenDepthScan : MonoBehaviour
    {
        private Material postEffectMat = null;
        private Camera currentCamera = null;

        [Range(0.8f, 1.0f)] public float scanValue = 0.85f;
        [Range(0.0f, 0.1f)] public float scanLineWidth = 0.02f;
        [Range(0.0f, 10.0f)] public float scanLightStrength = 10.0f;
        public Color scanLineColor = Color.white;

        void Awake()
        {
            currentCamera = GetComponent<Camera>();
        }

        void OnEnable()
        {
            if (postEffectMat == null)
                postEffectMat = new Material(Shader.Find("DepthTexture/ScreenDepthScan"));
            currentCamera.depthTextureMode |= DepthTextureMode.Depth;
        }

        void OnDisable()
        {
            currentCamera.depthTextureMode &= ~DepthTextureMode.Depth;
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (postEffectMat == null)
            {
                Graphics.Blit(source, destination);
            }
            else
            {
                //限制一下最大值，最小值
                float lerpValue = Mathf.Min(0.95f, 1 - scanValue);
                if (lerpValue < 0.0005f)
                    lerpValue = 1;

                //此处可以一个vec4传进去优化
                postEffectMat.SetFloat("_ScanValue", lerpValue);
                postEffectMat.SetFloat("_ScanLineWidth", scanLineWidth);
                postEffectMat.SetFloat("_ScanLightStrength", scanLightStrength);
                postEffectMat.SetColor("_ScanLineColor", scanLineColor);
                Graphics.Blit(source, destination, postEffectMat);
            }
        }
    }
}