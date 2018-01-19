Shader "FoxShaders/fox3DFW_WaterCover_LNM"
{
	Properties
	{
		MatParamIndex_0("MatParamIndex_0", Vector) = (0.0, 0.0, 0.0, 0.0)
		WaterTrans("WaterTrans", Vector) = (0.0, 0.0, 0.0, 0.0)
		WaterRoughness("WaterRoughness", Vector) = (0.0, 0.0, 0.0, 0.0)
		Incidence_Roughness("Incidence_Roughness", Vector) = (0.0, 0.0, 0.0, 0.0)
		Incidence_Color("Incidence_Color", Vector) = (0.0, 0.0, 0.0, 0.0)
		SubNormal_Blend("SubNormal_Blend", Vector) = (0.0, 0.0, 0.0, 0.0)
		URepeat_UV("URepeat_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
		VRepeat_UV("VRepeat_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
		UShift_UV("UShift_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
		VShift_UV("VShift_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
		Base_Tex_LIN("Base_Tex_LIN", 2D) = "white" {}
		NormalMap_Tex_NRM("NormalMap_Tex_NRM", 2D) = "white" {}
		SpheremapReflection_Tex_LIN("SpheremapReflection_Tex_LIN", 2D) = "white" {}
		SubNormalMap_Tex_NRM("SubNormalMap_Tex_NRM", 2D) = "white" {}
		SubNormalMask_Tex_LIN("SubNormalMask_Tex_LIN", 2D) = "white" {}
		AnimMask_Tex_LIN("AnimMask_Tex_LIN", 2D) = "white" {}
	}
		
	Subshader
	{
		Tags{ "Queue" = "AlphaTest" "Ignore Projector" = "True" "RenderType" = "Opaque" }
		LOD 200

		Blend SrcAlpha OneMinusSrcAlpha

		UsePass "Legacy Shaders/Transparent/Cutout/Diffuse/FORWARD"

		Pass
		{
			ZWrite On
			ColorMask 0
		}

		CGPROGRAM

		#pragma surface surf Standard fullforwardshadows alpha:premul
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