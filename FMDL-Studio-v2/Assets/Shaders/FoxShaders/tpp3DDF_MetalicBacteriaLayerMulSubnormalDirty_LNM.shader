Shader "FoxShaders/tpp3DDF_MetalicBacteriaLayerMulSubnormalDirty_LNM"
{
	Properties
	{
		MatParamIndex_0("MatParamIndex_0", Vector) = (0.0, 0.0, 0.0, 0.0)
		URepeat_UV("URepeat_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
		VRepeat_UV("VRepeat_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
		URepeat_SubNorm_UV("URepeat_SubNorm_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
		VRepeat_SubNorm_UV("VRepeat_SubNorm_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
		URepeatMetalic_UV("URepeatMetalic_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
		VRepeatMetalic_UV("VRepeatMetalic_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
		URepeatMetalicAlpha_UV("URepeatMetalicAlpha_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
		VRepeatMetalicAlpha_UV("VRepeatMetalicAlpha_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
		EdgeRoughness("EdgeRoughness", Vector) = (0.0, 0.0, 0.0, 0.0)
		EdgeTrans("EdgeTrans", Vector) = (0.0, 0.0, 0.0, 0.0)
		MimeticCenter("MimeticCenter", Vector) = (0.0, 0.0, 0.0, 0.0)
		MimeticRadiusMin("MimeticRadiusMin", Vector) = (0.0, 0.0, 0.0, 0.0)
		MimeticRadiusMax("MimeticRadiusMax", Vector) = (0.0, 0.0, 0.0, 0.0)
		InnerMimeticRadiusMin("InnerMimeticRadiusMin", Vector) = (0.0, 0.0, 0.0, 0.0)
		InnerMimeticRadiusMax("InnerMimeticRadiusMax", Vector) = (0.0, 0.0, 0.0, 0.0)
		MimeticRateAlphaRadiusMin("MimeticRateAlphaRadiusMin", Vector) = (0.0, 0.0, 0.0, 0.0)
		MimeticRateAlphaRadiusMax("MimeticRateAlphaRadiusMax", Vector) = (0.0, 0.0, 0.0, 0.0)
		MimeticAlphaOffseet("MimeticAlphaOffseet", Vector) = (0.0, 0.0, 0.0, 0.0)
		Base_Tex_SRGB("Base_Tex_SRGB", 2D) = "white" {}
		NormalMap_Tex_NRM("NormalMap_Tex_NRM", 2D) = "white" {}
		SpecularMap_Tex_LIN("SpecularMap_Tex_LIN", 2D) = "white" {}
		Layer_Tex_SRGB("Layer_Tex_SRGB", 2D) = "white" {}
		LayerMask_Tex_LIN("LayerMask_Tex_LIN", 2D) = "white" {}
		SubNormalMap_Tex_NRM("SubNormalMap_Tex_NRM", 2D) = "white" {}
		MetalicLayer_Tex_LIN("MetalicLayer_Tex_LIN", 2D) = "white" {}
		Dirty_Tex_LIN("Dirty_Tex_LIN", 2D) = "white" {}
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