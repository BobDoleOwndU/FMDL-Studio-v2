Shader "FoxShaders/tpp_2d_Map_ScreenMaskPAR"
{
	Properties
	{
		HeightLimit0("HeightLimit0", Vector) = (0.0, 0.0, 0.0, 0.0)
		HeightLimit1("HeightLimit1", Vector) = (0.0, 0.0, 0.0, 0.0)
		ParHeight("ParHeight", Vector) = (0.0, 0.0, 0.0, 0.0)
		TangentDir("TangentDir", Vector) = (0.0, 0.0, 0.0, 0.0)
		UCenter("UCenter", Vector) = (0.0, 0.0, 0.0, 0.0)
		VCenter("VCenter", Vector) = (0.0, 0.0, 0.0, 0.0)
		UShift("UShift", Vector) = (0.0, 0.0, 0.0, 0.0)
		VShift("VShift", Vector) = (0.0, 0.0, 0.0, 0.0)
		URepeat("URepeat", Vector) = (0.0, 0.0, 0.0, 0.0)
		VRepeat("VRepeat", Vector) = (0.0, 0.0, 0.0, 0.0)
		UShift_Mask("UShift_Mask", Vector) = (0.0, 0.0, 0.0, 0.0)
		VShift_Mask("VShift_Mask", Vector) = (0.0, 0.0, 0.0, 0.0)
		LoadColor("LoadColor", Vector) = (0.0, 0.0, 0.0, 0.0)
		UShift_AnimMaskTex("UShift_AnimMaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		VShift_AnimMaskTex("VShift_AnimMaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		URepeat_AnimMaskTex("URepeat_AnimMaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		VRepeat_AnimMaskTex("VRepeat_AnimMaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		Blend_AnimMaskTex("Blend_AnimMaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
		URepeat_Grid("URepeat_Grid", Vector) = (0.0, 0.0, 0.0, 0.0)
		VRepeat_Grid("VRepeat_Grid", Vector) = (0.0, 0.0, 0.0, 0.0)
		BevelBlendRate("BevelBlendRate", Vector) = (0.0, 0.0, 0.0, 0.0)
		CameraPos("CameraPos", Vector) = (0.0, 0.0, 0.0, 0.0)
		Height_Texture("Height_Texture", 2D) = "white" {}
		ColorTable_Texture("ColorTable_Texture", 2D) = "white" {}
		Mask_Texture("Mask_Texture", 2D) = "white" {}
		AnimMask_Texture("AnimMask_Texture", 2D) = "white" {}
		Grid_Texture("Grid_Texture", 2D) = "white" {}
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