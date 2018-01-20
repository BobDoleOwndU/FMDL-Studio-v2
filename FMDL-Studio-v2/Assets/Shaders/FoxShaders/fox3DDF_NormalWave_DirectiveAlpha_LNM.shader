Shader "FoxShaders/fox3DDF_NormalWave_DirectiveAlpha_LNM"
{
	Properties
	{
		MatParamIndex_0("MatParamIndex_0", Vector) = (0.0, 0.0, 0.0, 0.0)
		Specular_Value("Specular_Value", Vector) = (0.0, 0.0, 0.0, 0.0)
		Roughness_Value("Roughness_Value", Vector) = (0.0, 0.0, 0.0, 0.0)
		Translucent_Value("Translucent_Value", Vector) = (0.0, 0.0, 0.0, 0.0)
		WindDir("WindDir", Vector) = (0.0, 0.0, 0.0, 0.0)
		WindAnimTime("WindAnimTime", Vector) = (0.0, 0.0, 0.0, 0.0)
		WindAmplitude("WindAmplitude", Vector) = (0.0, 0.0, 0.0, 0.0)
		WeightDiffusion("WeightDiffusion", Vector) = (0.0, 0.0, 0.0, 0.0)
		WeightOffset("WeightOffset", Vector) = (0.0, 0.0, 0.0, 0.0)
		WindOffset("WindOffset", Vector) = (0.0, 0.0, 0.0, 0.0)
		OldWindDir("OldWindDir", Vector) = (0.0, 0.0, 0.0, 0.0)
		NormalRotRate("NormalRotRate", Vector) = (0.0, 0.0, 0.0, 0.0)
		OldWindAnimTime("OldWindAnimTime", Vector) = (0.0, 0.0, 0.0, 0.0)
		OldWindAmplitude("OldWindAmplitude", Vector) = (0.0, 0.0, 0.0, 0.0)
		WindRandAmplitude("WindRandAmplitude", Vector) = (0.0, 0.0, 0.0, 0.0)
		StartFadeDot("StartFadeDot", Vector) = (0.0, 0.0, 0.0, 0.0)
		EndFadeDot("EndFadeDot", Vector) = (0.0, 0.0, 0.0, 0.0)
		Base_Tex_SRGB("Base_Tex_SRGB", 2D) = "white" {}
		NormalMap_Tex_NRM("NormalMap_Tex_NRM", 2D) = "white" {}
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