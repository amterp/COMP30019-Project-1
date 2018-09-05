Shader "Custom/WavesShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Tint ("Color", Color) = (1,1,1,1)
		_Brightness ("Brightness", Range(0,1)) = 0.8
		_Amplitude ("Amplitude", Range(0,2)) = 0.36
		_Frequency ("Frequency", Range(0,5)) = 2
		_Wavelength ("Wavelength", Range(0,10)) = 1.5
		_TideAmplitude ("Tide Amplitude", Range(0,2)) = 0.1
		_TideFrequency ("Tide Frequency", Range(0,1)) = 0.5
		_Direction ("Wave Direction", Vector) = (1,1,0,0)
		_GameTime ("GameTime", float) = 0
		_fAtt("F Attenuation", range(0,1)) = 1
		_Kd("Diffuse Component Intensity", range(0,1)) = 1
		_Ka("Ambient Component Intensity", range(0,1)) = 1
		_Ks("Specular Component Intensity", range(0,1)) = 1
		_specN("specN", range(0,5)) = 1
		_PointLightColor("Point Light Color", Color) = (1, 1, 1)
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			
			struct VertexData {
				float4 position : POSITION;
				float4 normal : NORMAL;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			struct Interpolators {
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 worldVertex : TEXCOORD1;
				float3 worldNormal : TEXCOORD2;
				float4 color : COLOR;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			half _Amplitude;
			half _Frequency;
			float _TideAmplitude;
			float _TideFrequency;
			float4 _Tint;
			half _Wavelength;
			float _Brightness;
			float _GameTime;
			float _fAtt;
			float _Kd;
			float _Ka;
			float _Ks;
			float _specN;
			uniform float3 _PointLightColor;
			float2 _Direction;

			Interpolators Vert(VertexData v) {
				Interpolators o;
				
				// Calculate wave behaviour (gerstner waves)
				float l = 2 * UNITY_PI / _Wavelength;
				// Calculate our wave direction
				float2 d = normalize(_Direction);
				float wave = l * (dot(d, v.position.xz)) + (_GameTime * _Frequency);   
				float flat_a = _Amplitude / l;				// Used to prevent waves from having too high amplitude
				v.position.y = sin(wave)  * flat_a;			
				v.position.x += d.x * cos(wave) * flat_a * 0.5;
				v.position.z += d.y * cos(wave) * flat_a * 0.5;
				// Add our overall tide behaviour
				v.position.y += sin((v.position.xy) + (_GameTime * _TideFrequency)) * _TideAmplitude;

				// Prepare Phong model components
				float4 worldVertex = mul(unity_ObjectToWorld, v.position);
				float3 worldNormal = normalize(mul(transpose((float3x3)unity_WorldToObject), v.normal.xyz));
				o.color = v.color;
				o.worldVertex = worldVertex;
				o.worldNormal = worldNormal;

				// Transform vertex in world coordinates to camera coordinates
				o.position = UnityObjectToClipPos(v.position);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			float4 Frag(Interpolators v) : SV_TARGET{

				// This is inspired by the Phong shader as provided in Tutorial
				float3 interpNormal = normalize(v.worldNormal);

				// Calculate ambient RGB intensities
				float3 amb = v.color.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb * _Ka;

				// Calculate diffuse RBG reflections, we save the results of L.N because we will use it again
				// (when calculating the reflected ray in our specular component)
				float3 L = normalize(_WorldSpaceLightPos0);
				float LdotN = dot(L, interpNormal);
				float3 dif = _fAtt * _PointLightColor.rgb * _Kd * v.color.rgb * saturate(LdotN);

				// Calculate specular reflections
				float3 V = normalize(_WorldSpaceCameraPos - v.worldVertex.xyz);
				float3 R = normalize(2 * LdotN*interpNormal - L);
				float3 spe = _fAtt * _PointLightColor.rgb * _Ks * pow(saturate(dot(V, R)), _specN);

				float4 result = float4(amb.rgb, 1) + float4(dif.rgb, 1) + float4(spe.rgb, 1);
				fixed4 col = tex2D(_MainTex, v.uv) * _Brightness;

				return result * col;
			}
			ENDCG
		}
	}
}
