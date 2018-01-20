Shader "FoxShaders/tpp3DFW_TargetMarker"
{
	Properties
	{
		SelfColor("SelfColor", Vector) = (0.0, 0.0, 0.0, 0.0)
		ColorOffset("ColorOffset", Vector) = (0.0, 0.0, 0.0, 0.0)
		SelfColorIntensity("SelfColorIntensity", Vector) = (0.0, 0.0, 0.0, 0.0)
		Incidence_Roughness("Incidence_Roughness", Vector) = (0.0, 0.0, 0.0, 0.0)
		URepeat_ScreenTex("URepeat_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		VRepeat_ScreenTex("VRepeat_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		UShift_ScreenTex("UShift_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		VShift_ScreenTex("VShift_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		SelfColor2("SelfColor2", Vector) = (0.0, 0.0, 0.0, 0.0)
		SelfColorBlend("SelfColorBlend", Vector) = (0.0, 0.0, 0.0, 0.0)
		SelfColorBlendScale("SelfColorBlendScale", Vector) = (0.0, 0.0, 0.0, 0.0)
		ViewRangeMax("ViewRangeMax", Vector) = (0.0, 0.0, 0.0, 0.0)
		ViewRangeMin("ViewRangeMin", Vector) = (0.0, 0.0, 0.0, 0.0)
		ViewRangeLong("ViewRangeLong", Vector) = (0.0, 0.0, 0.0, 0.0)
		Noise_Tex_LIN("Noise_Tex_LIN", 2D) = "white" {}
		Gradient_Tex_LIN("Gradient_Tex_LIN", 2D) = "white" {}
		Depth_Tex_LIN("Depth_Tex_LIN", 2D) = "white" {}
	}
		
	Subshader
	{
		Tags{ "Queue" = "AlphaTest" "Ignore Projector" = "True" "RenderType" = "Opaque" }
		LOD 200

		Blend SrcAlpha OneMinusSrcAlpha

		AlphaToMask On

		Pass
		{
			ZWrite Off
			ColorMask 0
		}

		CGPROGRAM

		#pragma surface surf Standard fullforwardshadows alpha
		#pragma target 3.0

		sampler2D Base_Tex_SRGB;
		sampler2D NormalMap_Tex_NRM;
		sampler2D SpecularMap_Tex_LIN;
		sampler2D Translucent_Tex_LIN;

		struct Input
		{
			float2 uvBase_Tex_SRGB;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 mainTex = tex2D(Base_Tex_SRGB, IN.uvBase_Tex_SRGB);
			o.Albedo = mainTex.rgb;
			o.Alpha = mainTex.a;
			o.Metallic = 0.0f;
			o.Smoothness = 1.0f - tex2D(SpecularMap_Tex_LIN, IN.uvBase_Tex_SRGB).g;
			fixed4 finalNormal = tex2D(NormalMap_Tex_NRM, IN.uvBase_Tex_SRGB);
			finalNormal.r = finalNormal.g;
			finalNormal.g = 1.0f - finalNormal.g;
			finalNormal.b = 1.0f;
			finalNormal.a = 1.0f;
			o.Normal = UnpackNormal(finalNormal);
		}
		ENDCG
	}
	FallBack "Standard"
}