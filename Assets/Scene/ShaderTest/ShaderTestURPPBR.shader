// Upgrade NOTE: replaced tex2D unity_LightmapInd with UNITY_SAMPLE_TEX2D_SAMPLER

Shader "Test/ShaderTestURPPBR"//
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Tint("Tint", Color) = (1 ,1 ,1 ,1)
		_NormalMap("Normal Map", 2D) = "bump"{} // 表示当该位置没有指定任何法线贴图时，就使用模型顶点自带的法线
        _BumpScale("Bump Scale", Float) = 1  
		[Gamma] _Metallic("Metallic", Range(0, 1)) = 0 //金属度要经过伽马校正
		_Smoothness("Smoothness", Range(0, 1)) = 0.5
		//[Toggle(LIGHTMAP_ON)] _LIGHTMAP_ON("USE Lightmap Map", Int)=1
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		float4 _Tint;
		float _Metallic;
		float _Smoothness;
		sampler2D _MainTex;
		float4 _MainTex_ST;
		sampler2D _NormalMap;
        float4 _NormalMap_ST;
        float _BumpScale;    
		float4 _LightAngle;
		SAMPLER(sampler_LinearClamp);
		ENDHLSL

		Pass
		{
			Tags {
				"LightMode" = "UniversalForward"
			}
			HLSLPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			//#include "UnityStandardBRDF.cginc" 
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Assets/Bundles/Shader/SpaceTransforms.hlsl"
			#pragma multi_compile_fog;
			#pragma multi_compile _ LIGHTMAP_ON

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 uv : TEXCOORD0;
				float4 tangent : TANGENT;
				float2 texcoord1 : TEXCOORD1;
				float2 texcoord2 : TEXCOORD2;
				float2 lightMapUV:TEXCOORD3;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 uv : TEXCOORD0;
				float3 normal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 TtoW0 : TEXCOORD3;
				float4 TtoW1 : TEXCOORD4;
				float4 TtoW2 : TEXCOORD5;//xyz 存储着 从切线空间到世界空间的矩阵，w存储着世界坐标
				half4 ambientOrLightmapUV : TEXCOORD6;//存储环境光或光照贴图的UV坐标
				DECLARE_LIGHTMAP_OR_SH(lightMapUV, vertexSH, 8);
				half fogFactor : TEXCOORD7;
			};
			
			float3 fresnelSchlickRoughness(float cosTheta, float3 F0, float roughness)
			{
				return F0 + (max(float3(1 ,1, 1) * (1 - roughness), F0) - F0) * pow(1.0 - cosTheta, 5.0);
			}

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = TransformObjectToHClip(v.vertex);
				o.worldPos =TransformObjectToWorld(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv.zw = v.uv.xy * _NormalMap_ST.xy + _NormalMap_ST.zw; // 法线贴图的纹理坐标
				o.normal = TransformObjectToWorldNormal(v.normal);
				o.normal = normalize(o.normal);
				half3 worldTangent = TransformObjectToWorldDir(v.tangent);
				half3 worldBinormal = cross(o.normal,worldTangent) * v.tangent.w;
				//前3x3存储着从切线空间到世界空间的矩阵，后3x1存储着世界坐标
				o.TtoW0 = float4(worldTangent.x,worldBinormal.x,o.normal.x,o.worldPos.x);
				o.TtoW1 = float4(worldTangent.y,worldBinormal.y,o.normal.y,o.worldPos.y);
				o.TtoW2 = float4(worldTangent.z,worldBinormal.z,o.normal.z,o.worldPos.z);
				//计算环境光照或光照贴图uv坐标
				o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				OUTPUT_LIGHTMAP_UV(v.lightMapUV, unity_LightmapST, o.lightMapUV);
				o.fogFactor = ComputeFogFactor(o.vertex.z);
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				i.normal = normalize(i.normal);
				float perceptualRoughness = 1 - _Smoothness;//粗糙度
				float roughness = perceptualRoughness * perceptualRoughness;//粗糙度的二次方
				float squareRoughness = roughness * roughness;//粗糙度的四次方
				//法线
				real4 normalColor = tex2D(_NormalMap, i.uv.zw); 
				real3 tangentNormal = UnpackNormal(normalColor); // 使用Unity内置的方法，从颜色值得到法线在切线空间的方向
                tangentNormal.xy = tangentNormal.xy * _BumpScale; // 控制凹凸程度
                //tangentNormal = normalize(tangentNormal);
				half3 worldNormal = normalize(half3(dot(i.TtoW0.xyz,tangentNormal),
					dot(i.TtoW1.xyz,tangentNormal),dot(i.TtoW2.xyz,tangentNormal)));

			#ifdef LIGHTMAP_ON
				//lightmap光照方向
				float4 lightDirInTex =SAMPLE_TEXTURE2D(unity_LightmapInd,sampler_LinearClamp,i.ambientOrLightmapUV.xy);
				float4 lightDirTex = float4(2*lightDirInTex-1);
				float mainLightFactor = length(lightDirTex.xyz);
				float3 mainLightDir = normalize(lightDirTex.xyz/max(0.001,mainLightFactor));
				float3 lightDir = mainLightDir;
				//lightmap 环境光
				float3 lightColor = DecodeLightmap(SAMPLE_TEXTURE2D(unity_Lightmap,sampler_LinearClamp,i.ambientOrLightmapUV.xy),half4(LIGHTMAP_HDR_MULTIPLIER, LIGHTMAP_HDR_EXPONENT, 0.0h, 0.0h));
				//float3 lightColor = float3(1,0,0);
				float iblLightFactor = 1-mainLightFactor;
			#else
				Light light = GetMainLight();
				float3 lightDir = -light.direction;//光照方向
				float3 lightColor = LightingLambert(light.color, light.direction, worldNormal);
				uint pixelLightCount = GetAdditionalLightsCount();
				for (uint lightIndex = 0; lightIndex < pixelLightCount; ++lightIndex)
                {
                    Light light1 = GetAdditionalLight(lightIndex, i.worldPos);
                    lightColor += LightingLambert(light1.color, light1.direction, worldNormal);
                    //specColor += LightingSpecular(light1.color, light1.direction, normalize(worldNormal), normalize(viewDir), _Tint, _Smoothness);
                }
				//float3 lightColor =  float3(0,1,0);
				float mainLightFactor =1;
				float iblLightFactor = 1;
			#endif
				float4 testLightColor =normalize( float4(lightColor,1));
				//各向量
				float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);//视角方向
				float3 halfVector = normalize(lightDir + viewDir);  //半角向量

				//原版
				//float nl = max(saturate(dot(i.normal, lightDir)), 0.000001);//防止除0
				//float nv = max(saturate(dot(i.normal, viewDir)), 0.000001);
				//float vh = max(saturate(dot(viewDir, halfVector)), 0.000001);
				//float lh = max(saturate(dot(lightDir, halfVector)), 0.000001);
				//float nh = max(saturate(dot(i.normal, halfVector)), 0.000001);
				//修改版
				float nl = max(saturate(dot(worldNormal , lightDir)), 0.000001);//防止除0
				float nv = max(saturate(dot(worldNormal , viewDir)), 0.000001);
				float vh = max(saturate(dot(viewDir, halfVector)), 0.000001);
				float lh = max(saturate(dot(lightDir, halfVector)), 0.000001);
				float nh = max(saturate(dot(worldNormal , halfVector)), 0.000001);
				
				//直接光漫反射
				float3 Albedo = tex2D(_MainTex, i.uv.xy);// _Tint * tex2D(_MainTex, i.uv);
				//直接光镜面反射
				//D,法线分布函数.它描述的是在受到表面粗糙度的影响下，取向方向与中间向量一致的微平面的数量
				float lerpSquareRoughness = pow(lerp(0.002,1,roughness),2);
				float D = lerpSquareRoughness/(pow((pow(nh,2)*(lerpSquareRoughness-1)+1),2)*3.14159265359);
				//G，G被称为几何函数，描述的是微平面间相互遮蔽的比率,这种遮蔽会消耗掉光的能量导致表面变暗
				float kInDirectLight = pow(squareRoughness+1,2)/8;
				float kInIBL = pow(squareRoughness,2)/8;
				float GLeft = nl/lerp(nl,1,kInDirectLight);
				float GRight = nv/lerp(nv,1,kInDirectLight);
				float G = GLeft*GRight;
				//计算菲涅尔,菲涅尔方程的近似版本
				float3 F0=lerp(float3(0.04, 0.04, 0.04),Albedo,_Metallic);
				float3 F = F0+(1-F0)*exp2((-5.55473*vh-6.98316)*vh);
				//漫反射系数
				float kd = (1-F)*(1-_Metallic);
				#ifdef LIGHTMAP_ON
					float3 diffColor = kd * Albedo * lightColor * nl*mainLightFactor;
				#else
					float3 diffColor = Albedo * lightColor;
				#endif
				//直接光照镜面反射结果
				float3 SpecularResult = (D*G*F*0.25)/(nv*nl);
				float3 specColor = SpecularResult*lightColor*nl*3.14159265359*mainLightFactor;
				
				//直接光照结果
				float3 DirectLightResult =diffColor+specColor;
				//间接光计算
				//half3 ambient_contrib = ShaderSH9(float4(worldNormal,1));
				float3 ambient = 0.03*Albedo;
				float3 iblDiffuse = max(half3(0,0,0),ambient.rgb+lightColor*iblLightFactor);//间接光漫反射
				//间接光镜面反射
				float mip_roughness = perceptualRoughness * (1.7 - 0.7 * perceptualRoughness);
				float3 reflectVec = reflect(-viewDir, worldNormal);
				half mip = mip_roughness * UNITY_SPECCUBE_LOD_STEPS;
				//half4 rgbm = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, reflectVec, mip);
				//float3 iblSpecular = DecodeHDR(rgbm, unity_SpecCube0_HDR);
				//间接光菲涅尔系数
				float3 Flast = fresnelSchlickRoughness(max(nv, 0.0), F0, roughness);
				float kdLast = (1 - Flast) * (1 - _Metallic);
				//间接光照结果
				float3 iblDiffuseReslut = iblDiffuse*kdLast*Albedo;
				//float3 iblSpecularResut = iblSpecular *Flast;
				float3 IndriectReslut = iblDiffuseReslut;//+iblSpecularResut;
				//最终结果
				float4 result = float4(DirectLightResult+IndriectReslut,1);
				//雾
				result.rgb = MixFog(result.rgb, i.fogFactor);
				return float4(diffColor,1);
			}
			ENDHLSL            
		}

		 //Pass
   //     {
   //         Name "DepthOnly"
   //         Tags{"LightMode" = "DepthOnly"}

   //         ZWrite On
   //         ColorMask 0
   //         Cull[_Cull]

   //         HLSLPROGRAM
   //         // Required to compile gles 2.0 with standard srp library
   //         #pragma prefer_hlslcc gles
   //         #pragma exclude_renderers d3d11_9x
   //         #pragma target 2.0

   //         #pragma vertex DepthOnlyVertex
   //         #pragma fragment DepthOnlyFragment

   //         // -------------------------------------
   //         // Material Keywords
   //         #pragma shader_feature _ALPHATEST_ON
   //         #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

   //         //--------------------------------------
   //         // GPU Instancing
   //         #pragma multi_compile_instancing


   //         #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
   //         #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
   //         ENDHLSL
   //     }
	}
}


