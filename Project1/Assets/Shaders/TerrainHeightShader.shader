Shader "Custom/TerrainHeightShader" {

	Properties {
		_Tint("Tint", Color) = (1, 1, 1, 1)
		//_Color("sandColor", Color) = (1, 1, 1, 1)
		//_Color("snowColor", Color) = (1, 1, 1, 1)
		//_Color("defaultColor", Color) = (1, 1, 1, 1)
		//_Color("mountainColor", Color) = (1, 1, 1, 1)

		//_MainTex ("Texture", 2D) = "white" {}
	}

	SubShader {

		Pass {
			CGPROGRAM
			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram
			#include "UnityCG.cginc"

			float4 _Tint;
			//sampler2D _MainTex;
			//float4 _MainTex_ST;

			struct Interpolators {
				float4 position : SV_POSITION;
				//float2 uv : TEXCOORD0;
				//float3 localPos : TEXCOORD1;
				float4 color : COLOR;
			};

			struct VertexData {
				float4 position : POSITION;
				//float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			Interpolators MyVertexProgram (VertexData v) {
				Interpolators i;
				i.color = v.color;
				//i.localPos = v.position.xyz;
				i.position = UnityObjectToClipPos(v.position);
				//i.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return i;
			}

			float4 MyFragmentProgram (Interpolators i) : SV_TARGET {
				return i.color * (_Tint);
			}


			ENDCG
		}

	}

}