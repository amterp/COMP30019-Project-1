Shader "Unlit/WavesShader"
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
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			half _Amplitude;
			half _Frequency;
			float4 _Tint;
			half _Wavelength;
			float _Transparency;
			float _GameTime;
			
			v2f vert (appdata v)
			{
				v2f o;
				// Transform our vertex according to time

				float l = 2 * UNITY_PI / _Wavelength;
				float wave = l * v.vertex.x + (_GameTime * _Frequency);
				v.vertex.y = sin(wave)  * _Amplitude;
				v.vertex.x += cos(wave) * _Amplitude * 0.5;
				//v.vertex.z += sin(k * v.vertex.x + (_Time.y * _Frequency)) * _Amplitude * 0.1;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				col.a = _Transparency;
				return col * _Tint;
			}
			ENDCG
		}
	}
}
