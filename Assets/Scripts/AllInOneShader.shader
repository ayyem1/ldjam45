Shader "Jared/AllInOneShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Main Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_EffectTex("Effect Texture", 2D) = "white" {}
		_SilColor("Silhouette Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_DissolveAmount("Dissolve Amount", Range(0, 1)) = 0
		_DissolveOutlineColor("Dissolve Outline Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Intensity("Intensity", Range(0, 0.1)) = 0.01
		_Frequency("Frequency", Range(0, 100)) = 20
	}
		SubShader
	{
		CGINCLUDE
#include "UnityCG.cginc"



		struct appdata
	{
		float4 vertex : POSITION;
		float4 color : COLOR;
		float3 normal : NORMAL;
		float2 uv : TEXCOORD0;
		float2 effectUV : TEXCOORD1;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float2 effectUV : TEXCOORD1;
		float4 vertex : SV_POSITION;
		float3 normal : NORMAL;
		float4 color : COLOR;
	};

	float4 _LightColor0;
	float4 _Color;
	float4 _SilColor;
	sampler2D _MainTex;
	float4 _MainTex_ST;
	sampler2D _EffectTex;
	float4 _EffectTex_ST;
	float4 _DissolveOutlineColor;
	half _DissolveAmount;
	float _Intensity;
	float _Frequency;



	//This is faster than NormalShader, because we are doing all of our calculations per-vertex rather than per-fragment
	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.normal = normalize(mul(float4(v.normal, 0.0), unity_ObjectToWorld).xyz);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		o.effectUV = TRANSFORM_TEX(v.effectUV, _EffectTex);

		float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);

		float4 newVertex = mul(v.vertex, unity_ObjectToWorld) + _Intensity * ((float4(o.normal, 0.0) * sin(o.uv.x * _Frequency + _Time.w)) + (float4(o.normal, 0.0) * sin(o.uv.y * _Frequency + _Time.w)));

		o.vertex = UnityObjectToClipPos(newVertex);
		o.normal = normalize(newVertex);
		float3 diffuse = _LightColor0.rgb * max(0.0, dot(o.normal, lightDirection));
		o.color = float4(diffuse, 1.0);

		return o;
	}
		ENDCG

		//Pass 1: Ensure that the objects vertices are all rendered on top of everything.
		//		  Set the color of all of the fragments as the Silhouette Color.  Apply checkered texture.
		Pass{
		//Cull Off
		ZWrite Off
		ZTest Always

		CGPROGRAM
#pragma vertex vert //Remember we're using the vert function from the CGINCLUDE section here
#pragma fragment frag

		half4 frag(v2f i) : COLOR{

		//i.effectUV.y = i.effectUV.y + _Time.x;

		float4 effectTexColor = tex2D(_EffectTex, i.effectUV);

		clip(effectTexColor.g - 0.3);

		// sample the dissolve texture.  The texture is greyscale so r == b == g
		float dissolveColor = tex2D(_EffectTex, i.effectUV).r;

		clip(dissolveColor - _DissolveAmount);

		return _SilColor;
	}
		ENDCG
	}

		//Pass 2: Apply texture to all of the fragments that are higher (closer to screen) in the depth buffer
		//		  by re-enabling ZTesting and ZWriting to the depth buffer
		Pass{
		ZWrite On
		ZTest LEqual
	
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		half4 frag(v2f i) : COLOR{

		i.uv.x = i.uv.x + _Time.x;
		//i.effectUV.x = i.effectUV.x - _Time.x;

		// sample the dissolve texture.  The texture is greyscale so r == b == g
		float dissolveColor = tex2D(_EffectTex, i.effectUV).r;

		clip(dissolveColor - _DissolveAmount);

		if (dissolveColor - _DissolveAmount < 0.05)
		{
			return _DissolveOutlineColor;
		}
		else
		{
			return tex2D(_MainTex, i.uv);//* i.color;
		}
	}
		ENDCG
	}
	}
}
