Shader "Custom/FakePointLight"
{
	Properties
	{
		_Color("Color", COLOR) = (1,1,1,1)
		_Intensity("Intensity", Float) = 1
		_Range("Range", Float) = 1
	}
		SubShader
		{
			Tags { "RenderType" = "TransparentCutout" "Queue" = "AlphaTest" }
			Pass
			{
				Tags
				{
					"LightMode" = "UniversalForward"
				}
				Cull Off
				Blend SrcAlpha OneMinusSrcAlpha
				HLSLPROGRAM
				#pragma vertex vert
				#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;

			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 scrPos : TEXCOORD3;
			};


			CBUFFER_START(UnityPerMaterial)
			float4 _Color;
			float _Intensity;
			float _Range;
			CBUFFER_END
			TEXTURE2D_X_FLOAT(_CameraDepthTexture);
			SAMPLER(sampler_CameraDepthTexture);
			TEXTURE2D(_GrabPassTexture);
			SAMPLER(sampler_GrabPassTexture);

			v2f vert(appdata v)
			{
				v2f o;
				VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
				o.pos = vertexInput.positionCS;
				o.scrPos = ComputeScreenPos(vertexInput.positionCS);
				return o;
			}
			
			float4 GetWorldPositionFromDepthValue(float2 uv, float linearDepth) //通过深度图倒退世界坐标值
			{
				float camPosZ = _ProjectionParams.y + (_ProjectionParams.z - _ProjectionParams.y) * linearDepth;

				float height = 2 * camPosZ / unity_CameraProjection._m11;
				float width = _ScreenParams.x / _ScreenParams.y * height;

				float camPosX = width * uv.x - width / 2;
				float camPosY = height * uv.y - height / 2;
				float4 camPos = float4(camPosX, camPosY, camPosZ, 1.0);
				return mul(unity_CameraToWorld, camPos);
			}

			float NormalDistribution(float x, float intensity, float range) //正态分布
			{
				return ((1 / sqrt(2)*PI) * exp(-pow(x*(1 / range), 2) / 2)) * intensity;
			}

			float4 frag(v2f i) : SV_Target
			{
				half2 screenPos = i.scrPos.xy / i.scrPos.w;
				float4 sceneCol = SAMPLE_TEXTURE2D_X(_GrabPassTexture, sampler_GrabPassTexture, screenPos);
				//深度
				float depth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, screenPos).r;
				float depthValue = Linear01Depth(depth, _ZBufferParams);
				float3 screenWorldPos = GetWorldPositionFromDepthValue(screenPos, depthValue).rgb; //获取本体外世界坐标

				float4x4 m = UNITY_MATRIX_M;
				float3 worldPos = float3(m[0].w, m[1].w, m[2].w); //获取本体的世界位置

				float distance = length(screenWorldPos - worldPos); //算出世界坐标与本体的距离

				float4 col = max(NormalDistribution(distance, _Intensity, _Range) * _Color * sceneCol, 0); //与场景颜色进行相乘得到加亮的颜色
				//float fff = NormalDistribution(distance, _Intensity, _Range);
				//return float4(col.rgb,fff);
                return col;
			}
			ENDHLSL
		}
		}
}