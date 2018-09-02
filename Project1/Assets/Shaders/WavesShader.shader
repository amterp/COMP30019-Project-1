Shader "Custom/WavesShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Tint ("Color", Color) = (1,1,1,1)
		_Amplitude ("Amplitude", Range(0,2)) = 0.25
		_Frequency ("Frequency", Range(0,5)) = 1
		_Wavelength ("Wavelength", Range(0,10)) = 4
		_Transparency ("Transparency", Range(0,1)) = 1
		_GameTime ("GameTime", float) = 0
		_fAtt("F Attenuation", range(0,1)) = 1
		_Kd("Kd", range(0,1)) = 1
		_Ka("Ka", range(0,1)) = 1
		_Ks("Ks", range(0,1)) = 1
		_specN("specN", range(0,5)) = 1
		_PointLightColor("Point Light Color", Color) = (0, 0, 0)

	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
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
			float4 _Tint;
			half _Wavelength;
			float _Transparency;
			float _GameTime;
			float _fAtt;
			float _Kd;
			float _Ka;
			float _Ks;
			float _specN;
			uniform float3 _PointLightColor;

			Interpolators Vert(VertexData v) {
				Interpolators o;
				float4 worldVertex = mul(unity_ObjectToWorld, v.position);
				float3 worldNormal = normalize(mul(transpose((float3x3)unity_WorldToObject), v.normal.xyz));
				float l = 2 * UNITY_PI / _Wavelength;
				float wave = l * v.position.x + (_GameTime * _Frequency);
				v.position.y = sin(wave)  * _Amplitude;
				v.position.x += cos(wave) * _Amplitude * 0.5;
				// Combine Phong illumination model components
				o.color = v.color;
				o.worldVertex = worldVertex;
				o.worldNormal = worldNormal;
				// Transform vertex in world coordinates to camera coordinates
				o.position = UnityObjectToClipPos(v.position);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			float4 Frag(Interpolators v) : SV_TARGET{
				// Our interpolated normal might not be of length 1
				float3 interpNormal = normalize(v.worldNormal);

				// Calculate ambient RGB intensities
				// float Ka = 1;
				float3 amb = v.color.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb * _Ka;

				// Calculate diffuse RBG reflections, we save the results of L.N because we will use it again
				// (when calculating the reflected ray in our specular component)
				//float fAtt = 1;
				//float Kd = 1;
				float3 L = normalize(_WorldSpaceLightPos0);
				float LdotN = dot(L, interpNormal);
				float3 dif = _fAtt * _PointLightColor.rgb * _Kd * v.color.rgb * saturate(LdotN);

				// Calculate specular reflections
				//float Ks = 1;
				//float specN = 2; // Values>>1 give tighter highlights
				float3 V = normalize(_WorldSpaceCameraPos - v.worldVertex.xyz);
				float3 R = normalize(2 * LdotN*interpNormal - L);
				float3 spe = _fAtt * _PointLightColor.rgb * _Ks * pow(saturate(dot(V, R)), _specN);

				float4 result = float4(amb.rgb, 1) + float4(dif.rgb, 1) + float4(spe.rgb, 1);
				fixed4 col = tex2D(_MainTex, v.uv);

				return result * col * float4(1,1,1,_Transparency);
			}
			ENDCG
		}
	}
}
