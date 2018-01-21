Shader "FoxShaders/fox_2d_Basic_LyADD"
{
	Properties
	{
		UCenter_BaseTex("UCenter_BaseTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		VCenter_BaseTex("VCenter_BaseTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		UShift_BaseTex("UShift_BaseTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		VShift_BaseTex("VShift_BaseTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		URepeat_BaseTex("URepeat_BaseTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		VRepeat_BaseTex("VRepeat_BaseTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		UCenter_LayerTex("UCenter_LayerTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		VCenter_LayerTex("VCenter_LayerTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		UShift_LayerTex("UShift_LayerTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		VShift_LayerTex("VShift_LayerTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		URepeat_LayerTex("URepeat_LayerTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		VRepeat_LayerTex("VRepeat_LayerTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		UCenter_MaskTex("UCenter_MaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		VCenter_MaskTex("VCenter_MaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		UShift_MaskTex("UShift_MaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		VShift_MaskTex("VShift_MaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		URepeat_MaskTex("URepeat_MaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		VRepeat_MaskTex("VRepeat_MaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		UCenter_ScreenTex("UCenter_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		VCenter_ScreenTex("VCenter_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		UShift_ScreenTex("UShift_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		VShift_ScreenTex("VShift_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		URepeat_ScreenTex("URepeat_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		VRepeat_ScreenTex("VRepeat_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		Blend_BaseTex("Blend_BaseTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		Blend_LayerTex("Blend_LayerTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		Blend_MaskTex("Blend_MaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		Blend_ScreenTex("Blend_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		Base_Texture("Base_Texture", 2D) = "white" {}
		Layer_Texture("Layer_Texture", 2D) = "white" {}
		Mask_Texture("Mask_Texture", 2D) = "white" {}
		Screen_Texture("Screen_Texture", 2D) = "white" {}
	}
		
	Subshader
	{
		Tags{ "Queue" = "Geometry" "Ignore Projector" = "True" "RenderType" = "Opaque" }
		LOD 200

		Blend SrcAlpha OneMinusSrcAlpha

		AlphaToMask On

		Pass
		{
			ZWrite On
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