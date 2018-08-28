// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/TerrainHeightShader" {

	Properties {
		_fAtt("F Attenuation", range(0,1)) = 1
		_Kd("Kd", range(0,1)) = 1
		_Ka("Ka", range(0,1)) = 1
		_Ks("Ks", range(0,1)) = 1
		_specN("specN", range(0,5)) = 1
		_PointLightColor("Point Light Color", Color) = (0, 0, 0)
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
			//sampler2D _MainTex;
			//float4 _MainTex_ST;
			uniform float3 _PointLightColor;
			uniform float3 _PointLightPosition;

			struct VertexData {
				float4 position : POSITION;
				float4 normal : NORMAL;
				float4 color : COLOR;
			};

			struct Interpolators {
				float4 position : SV_POSITION;
				float4 worldVertex : TEXCOORD0;
				float3 worldNormal : TEXCOORD1;
				float4 color : COLOR;
			};

			Interpolators Vert (VertexData v) {
				Interpolators o;
				float4 worldVertex = mul(unity_ObjectToWorld, v.position);
				float3 worldNormal = normalize(mul(transpose((float3x3)unity_WorldToObject), v.normal.xyz));

				// Combine Phong illumination model components
				o.color = v.color;

				o.worldVertex = worldVertex;
				o.worldNormal = worldNormal;
				// Transform vertex in world coordinates to camera coordinates
				o.position = UnityObjectToClipPos(v.position);
				return o;
			}

			float4 Frag (Interpolators v) : SV_TARGET {
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

				return result;
			}


			ENDCG
		}

	}

}