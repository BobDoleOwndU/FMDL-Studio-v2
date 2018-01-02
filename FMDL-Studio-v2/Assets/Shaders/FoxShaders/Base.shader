Shader "FoxShaders/Base"
{
	Properties
	{
		_Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_MainTex("Albedo", 2D) = "white" {}
		_LayerColor("Secondary Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_LayerTex("Secondary Albedo", 2D) = "white" {}
		_LayerMask("Secondary Albedo Mask", 2D) = "black" {}
		_BumpMap("Normal Map", 2D) = "bump" {}
		_Metalness("Metalness", Range(0,1)) = 0
		_SRM("Roughness", 2D) = "white" {}
	}

	SubShader
	{
		Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "Opaque" }
		LOD 200

		Blend SrcAlpha OneMinusSrcAlpha     // Alpha blending

		// paste in forward rendering passes from Transparent/Diffuse
		UsePass "Legacy Shaders/Transparent/Cutout/Diffuse/FORWARD"

		// extra pass that renders to depth buffer only
		Pass
		{
			ZWrite On
			ColorMask 0
		}

		CGPROGRAM

		#pragma surface surf Standard fullforwardshadows alpha:premul
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

		half _Metalness;
		fixed4 _Color;
		fixed4 _LayerColor;

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
			_Color.a = 1.0f;
			fixed4 mainTinted = mainTex * _Color;
			fixed4 layerTex = tex2D(_LayerTex, IN.uv_MainTex);
			_LayerColor.a = 1.0f;
			fixed4 layerTinted = layerTex * _LayerColor;
			fixed4 layerMask = tex2D(_LayerMask, IN.uv_MainTex);
			fixed4 c = lerp(mainTinted, layerTinted, layerMask);
			o.Albedo = c.rgb;
			//o.Albedo = 1.0f;
			o.Metallic = _Metalness;
			o.Smoothness = 1.0f - tex2D(_SRM, IN.uv_MainTex).g;
			o.Alpha = c.a;
			fixed4 fixedNormal = tex2D(_BumpMap, IN.uv_MainTex);
			fixedNormal.r = fixedNormal.a;
			fixedNormal.g = 1.0f - fixedNormal.g;
			fixedNormal.b = 1.0f;
			fixedNormal.a = 1.0f;
			o.Normal = UnpackNormal(fixedNormal);
			//o.Emission = c.a;
		}
		ENDCG
	}
	FallBack "Standard"
}