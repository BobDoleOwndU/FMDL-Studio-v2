Shader "FoxShaders/Base"
{
	Properties
	{
		_MainTex("Albedo", 2D) = "white" {}
		NormalMap_Tex_NRM("Normal Map", 2D) = "bump" {}
		Metalness("Metalness", Range(0,1)) = 0
		SpecularMap_Tex_LIN("Roughness", 2D) = "white" {}
		Translucent_Tex_LIN("Transmissive - placeholder", 2D) = "white" {}
		ViewReflection_Tex_LIN("Precomputed reflection (not a cubemap) - placeholder", 2D) = "white" {}
		LensHeight_Tex_LIN("Parallax height map - placeholder", 2D) = "white" {}
		Base_Tex2_SRGB("Iris texture - placeholder", 2D) = "white" {}
	}

	SubShader
	{
		Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "Opaque" }
		LOD 200

		// Alpha blending
		Blend SrcAlpha OneMinusSrcAlpha    

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

		//Normal textures; Translucent_Tex_LIN is still temporary
		sampler2D _MainTex;
		sampler2D NormalMap_Tex_NRM;
		sampler2D SpecularMap_Tex_LIN;
		sampler2D Translucent_Tex_LIN;

		//Placeholder textures for eye material writing
		sampler2D ViewReflection_Tex_LIN;
		sampler2D LensHeight_Tex_LIN;
		sampler2D Base_Tex2_SRGB;

		struct Input
		{
		float2 uv_Main;
		};

		half Metalness;

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			//Albedo
			fixed4 mainTex = tex2D(_MainTex, IN.uv_Main);
			o.Albedo = mainTex.rgb;
			o.Alpha = mainTex.a;
	
			//Specular
			o.Metallic = Metalness;
			o.Smoothness = 1.0f - tex2D(SpecularMap_Tex_LIN, IN.uv_Main).g;

			//Normal
			fixed4 finalNormal = tex2D(NormalMap_Tex_NRM, IN.uv_Main);
			finalNormal.r = finalNormal.a;
			finalNormal.g = 1.0f - finalNormal.g;
			finalNormal.b = 1.0f;
			finalNormal.a = 1.0f;
			o.Normal = UnpackNormal(finalNormal);
		}
		ENDCG
	}
	FallBack "Standard"
}