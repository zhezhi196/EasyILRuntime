//故障效果shader
Shader "ScreenEffect/HologramBlockPE"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		//_BlockSize("BlockSize", float) = 1
		//_Speed("Speed", float) = 10
	}
		SubShader
		{
			ZTest Always ZWrite Off Cull Off
			Tags {"RenderPipeline" = "UniversalPipeline"}

			Pass
			{
				HLSLPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
					//float4 worldVertex : TEXCOORD1;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				//half _BlockSize;//删除Block
				//half _Speed;//删除Speed
				float4 _Params;
				//float randomNoise(float2 n)
				//{
				//    return sin(dot(n* floor(_Time.y * _Speed), half2(1233.224, 1743.335)));
				//}

				float randomNoise(float2 seed)
				{
					return frac(sin(dot(seed * floor(_Time.y*0 ), float2(17.13, 3.71))) * 43758.5453123);//删除Speed
					//return frac(sin(dot(seed * floor(_Time.y * _Speed), float2(17.13, 3.71))) * 43758.5453123);
				}
				float nrand(float x, float y)
				{
					return frac(sin(dot(float2(x, y), float2(12.9898, 78.233))) * 43758.5453);
				}

				v2f vert(appdata v)
				{
					v2f o;
					VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
					o.vertex = vertexInput.positionCS;
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}

				float4 frag(v2f i) : SV_Target
				{
					float2 _ScanLineJitter = float2(_Params.x, _Params.y);
					float2 _ColorDrift = float2(_Params.z,_Params.w);
					//half block = randomNoise(floor((i.uv) * _BlockSize));//删除Block
					float4 col = tex2D(_MainTex, i.uv);
					//half displaceNoise = pow(block.x, 8.0) * pow(block.x, 3.0);//删除Block

					float u = i.uv.x;
					float v = i.uv.y;
					float jitter = nrand(v, _Time.x) * 2 - 1;
					jitter *= step(_ScanLineJitter.y, abs(jitter)) * _ScanLineJitter.x;
					// Color drift
					float drift = sin(_ColorDrift.y) * _ColorDrift.x;

					//half ColorR = tex2D(_MainTex, frac(i.uv + float2(jitter,0))).r;
					//half ColorG = tex2D(_MainTex, frac(i.uv + float2(displaceNoise * 0.1 * randomNoise(7.0) + jitter + drift, 0.0))).g;
					//half ColorB = tex2D(_MainTex, frac(i.uv - float2(displaceNoise * 0.1 * randomNoise(13.0) - jitter, 0.0))).b;

					half ColorR = tex2D(_MainTex, frac(i.uv + float2(jitter,0))).r;
					half ColorG = tex2D(_MainTex, frac(i.uv + float2(0.1 * randomNoise(7.0) + jitter + drift, 0.0))).g;//删除Block
					half ColorB = tex2D(_MainTex, frac(i.uv - float2(0.1 * randomNoise(13.0) - jitter, 0.0))).b;//删除Block
					float4 finCol = float4(ColorR, ColorG, ColorB, 1);

					return finCol;
				}
			ENDHLSL
		}
	}
}
