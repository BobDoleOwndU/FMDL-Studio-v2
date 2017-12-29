<<<<<<< HEAD
﻿// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "CustomShaders/FoxShaders" 
{
=======
﻿Shader "CustomShaders/FoxShaders" {
>>>>>>> parent of 2a5c352... Merge pull request #18 from BobDoleOwndU/master
	Properties
	{
		_Color("Color", Color) = (1.0, 1.0, 1.0, 0)
		_MainTex("Albedo", 2D) = "white" {}
		_LayerColor("Secondary Color", Color) = (1.0, 1.0, 1.0, 0)
		_LayerTex("Secondary Albedo", 2D) = "white" {}
		_LayerMask("Secondary Albedo Mask", 2D) = "black" {}
		_BumpMap("Normal Map", 2D) = "bump" {}
		_Metalness("Metalness", Range(0,1)) = 0
		_SRM("Roughness", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows
		//#pragma surface surf Lambert alphatest:_Cutoff

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _LayerTex;
		sampler2D _LayerMask;
		sampler2D _BumpMap;
		sampler2D _SRM;

		struct Input
		{
			float2 uv_MainTex;
		};

		half _Metallic;
		fixed4 _Color;
		fixed4 _LayerColor;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
		// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		void surf(Input IN, inout SurfaceOutputStandard o)
		//void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 c = lerp(tex2D(_MainTex, IN.uv_MainTex) * _Color, tex2D(_LayerTex, IN.uv_MainTex) * _LayerColor, tex2D(_LayerMask, IN.uv_MainTex));
			o.Albedo = c.rgb;
			o.Metallic = 0.0f;
			o.Smoothness = 1.0f - tex2D(_SRM, IN.uv_MainTex).g;
			o.Alpha = 0.0f;
			float4 FixedNormal = tex2D(_BumpMap, IN.uv_MainTex);
			FixedNormal.r = FixedNormal.a;
			FixedNormal.g = 1.0f - FixedNormal.g;
			FixedNormal.b = 1.0f;
			FixedNormal.a = 1.0f;
			o.Normal = UnpackNormal(FixedNormal);
		}
		ENDCG
	}
	FallBack "Diffuse"
}