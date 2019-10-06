Shader "Jared/WaveShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Amplitude("Amplitude", Range(0, 10)) = 5
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			float4 _LightColor0;

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float4 color : COLOR;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Amplitude;
			
			//This is faster than NormalShader, because we are doing all of our calculations per-vertex rather than per-fragment
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = normalize(mul(float4(v.normal, 0.0), unity_ObjectToWorld).xyz);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				float3 diffuse = _LightColor0.rgb * max(0.0, dot(o.normal, lightDirection));
				float3 clampedNormal = v.normal * 0.5 + 0.5; //Clamp between 0 and 1

				o.color = float4(clampedNormal, 0.0) * float4(diffuse, 1.0);
				o.vertex.x += sin(o.uv.x * _Amplitude + _Time.w);
				o.vertex.x += sin(o.uv.y * _Amplitude + _Time.w);
				
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return tex2D(_MainTex, i.uv); //* i.color;
			}
			ENDCG
		}
	}
}
