// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/TerrainHeightShader" {

	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_fAtt("F Attenuation", range(0,1)) = 1
		_Kd("Diffuse Component Intensity", range(0,1)) = 1
		_Ka("Ambient Component Intensity", range(0,1)) = 1
		_Ks("Specular Component Intensity", range(0,1)) = 0.8
		_Brightness("Brightness", range(0,1)) = 0.9
		_specN("specN", range(0,5)) = 0.8
		_PointLightColor("Point Light Color", Color) = (1, 1, 1)
	}

	SubShader {

		Pass {
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			#include "UnityCG.cginc"

			float _fAtt;
			float _Kd;
			float _Ka;
			float _Ks;
			float _specN;
			float _Brightness;
			sampler2D _MainTex;
			uniform float3 _PointLightColor;
			uniform float3 _PointLightPosition;

			struct VertexData {
				float4 position : POSITION;
				float4 normal : NORMAL;
				float4 color : COLOR;
				float4 uv : TEXCOORD3;
			};

			struct Interpolators {
				float4 position : SV_POSITION;
				float4 worldVertex : TEXCOORD0;
				float3 worldNormal : TEXCOORD1;
				float4 uv : TEXCOORD3;
				float4 color : COLOR;
			};

			Interpolators Vert (VertexData v) {
				Interpolators o;
				float4 worldVertex = mul(unity_ObjectToWorld, v.position);
				float3 worldNormal = normalize(mul(transpose((float3x3)unity_WorldToObject), v.normal.xyz));

				// Combine Phong illumination model components
				o.color = v.color;
				o.uv = v.uv;
				o.worldVertex = worldVertex;
				o.worldNormal = worldNormal;
				// Transform vertex in world coordinates to camera coordinates
				o.position = UnityObjectToClipPos(v.position);
				return o;
			}

			float4 Frag (Interpolators v) : SV_TARGET {

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
				float3 R = normalize(2 * LdotN * interpNormal - L);
				float3 spe = _fAtt * _PointLightColor.rgb * _Ks * pow(saturate(dot(V, R)), _specN);

				float4 result = float4(amb.rgb, 1) + float4(dif.rgb, 1) + float4(spe.rgb, 1);
				fixed4 col = tex2D(_MainTex, v.uv) * _Brightness;

				return result * col;
			}


			ENDCG
		}

	}

}
