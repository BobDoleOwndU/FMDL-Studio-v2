Shader "FoxShaders/tpp3DFW_Constant_Sky"
{
	Properties
	{
		p0("p0", Vector) = (0.0, 0.0, 0.0, 0.0)
		p1("p1", Vector) = (0.0, 0.0, 0.0, 0.0)
		p2("p2", Vector) = (0.0, 0.0, 0.0, 0.0)
		p3("p3", Vector) = (0.0, 0.0, 0.0, 0.0)
		p4("p4", Vector) = (0.0, 0.0, 0.0, 0.0)
		p5("p5", Vector) = (0.0, 0.0, 0.0, 0.0)
		p6("p6", Vector) = (0.0, 0.0, 0.0, 0.0)
		p7("p7", Vector) = (0.0, 0.0, 0.0, 0.0)
		t_0("t_0", 2D) = "white" {}
		t_1("t_1", 2D) = "white" {}
		t_2("t_2", 2D) = "white" {}
		t_3("t_3", 2D) = "white" {}
		t_4("t_4", 2D) = "white" {}
		t_5("t_5", 2D) = "white" {}
		t_6("t_6", 2D) = "white" {}
		t_7("t_7", 2D) = "white" {}
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