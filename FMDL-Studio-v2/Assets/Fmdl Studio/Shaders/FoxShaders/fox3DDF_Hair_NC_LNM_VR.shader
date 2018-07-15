Shader "FoxShaders/fox3DDF_Hair_NC_LNM_VR" {
Properties {
	MatParamIndex_0("MatParamIndex_0", Vector) = (0.0, 0.0, 0.0, 0.0)
	Anistropic_MainLightDir("Anistropic_MainLightDir", Vector) = (0.0, 0.0, 0.0, 0.0)
	Anistropic_Diffusion("Anistropic_Diffusion", Vector) = (0.0, 0.0, 0.0, 0.0)
	HairShiftScale("HairShiftScale", Vector) = (0.0, 0.0, 0.0, 0.0)
	Incidence_Roughness("Incidence_Roughness", Vector) = (0.0, 0.0, 0.0, 0.0)
	Incidence_Color("Incidence_Color", Vector) = (0.0, 0.0, 0.0, 0.0)
	URepeat_UV("URepeat_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
	VRepeat_UV("VRepeat_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
	Base_Tex_SRGB("Base_Tex_SRGB", 2D) = "white" {}
	SpecularMap_Tex_LIN("SpecularMap_Tex_LIN", 2D) = "white" {}
	Translucent_Tex_LIN("Translucent_Tex_LIN", 2D) = "white" {}
	Shift_Tex_LIN("Shift_Tex_LIN", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}

SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	LOD 400

CGPROGRAM
#pragma surface surf BlinnPhong alphatest:_Cutoff
#pragma target 3.0

sampler2D Base_Tex_SRGB;
sampler2D SpecularMap_Tex_LIN;

struct Input {
	float2 uvBase_Tex_SRGB;
	float2 uvSpecularMap_Tex_LIN;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(Base_Tex_SRGB, IN.uvBase_Tex_SRGB);
	o.Albedo = tex.rgb;
	o.Gloss = tex.a;
	o.Alpha = tex.a;
	fixed4 specularTex = tex2D(SpecularMap_Tex_LIN, IN.uvSpecularMap_Tex_LIN);
	o.Specular = specularTex.r;
}
ENDCG
}

FallBack "Standard"
}