Shader "FoxShaders/tpp3DFW_MetalicBacteriaGlass_CommonRef_LNM" {
Properties {
	MatParamIndex_0("MatParamIndex_0", Vector) = (0.0, 0.0, 0.0, 0.0)
	ReflectionColor("ReflectionColor", Vector) = (0.0, 0.0, 0.0, 0.0)
	GlassColor("GlassColor", Vector) = (0.0, 0.0, 0.0, 0.0)
	URepeatMetalic_UV("URepeatMetalic_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
	VRepeatMetalic_UV("VRepeatMetalic_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
	URepeatMetalicAlpha_UV("URepeatMetalicAlpha_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
	VRepeatMetalicAlpha_UV("VRepeatMetalicAlpha_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
	EdgeRoughness("EdgeRoughness", Vector) = (0.0, 0.0, 0.0, 0.0)
	EdgeTrans("EdgeTrans", Vector) = (0.0, 0.0, 0.0, 0.0)
	GlassRoughness("GlassRoughness", Vector) = (0.0, 0.0, 0.0, 0.0)
	MimeticCenter("MimeticCenter", Vector) = (0.0, 0.0, 0.0, 0.0)
	MimeticRadiusMin("MimeticRadiusMin", Vector) = (0.0, 0.0, 0.0, 0.0)
	MimeticRadiusMax("MimeticRadiusMax", Vector) = (0.0, 0.0, 0.0, 0.0)
	InnerMimeticRadiusMin("InnerMimeticRadiusMin", Vector) = (0.0, 0.0, 0.0, 0.0)
	InnerMimeticRadiusMax("InnerMimeticRadiusMax", Vector) = (0.0, 0.0, 0.0, 0.0)
	MimeticRateAlphaRadiusMin("MimeticRateAlphaRadiusMin", Vector) = (0.0, 0.0, 0.0, 0.0)
	MimeticRateAlphaRadiusMax("MimeticRateAlphaRadiusMax", Vector) = (0.0, 0.0, 0.0, 0.0)
	MimeticAlphaOffseet("MimeticAlphaOffseet", Vector) = (0.0, 0.0, 0.0, 0.0)
	Base_Tex_LIN("Base_Tex_LIN", 2D) = "white" {}
	NormalMap_Tex_NRM("NormalMap_Tex_NRM", 2D) = "white" {}
	MetalicLayer_Tex_LIN("MetalicLayer_Tex_LIN", 2D) = "white" {}
	MetalicBacteria_Tex_LIN("MetalicBacteria_Tex_LIN", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}

SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	LOD 400

CGPROGRAM
#pragma surface surf BlinnPhong alphatest:_Cutoff
#pragma target 3.0

sampler2D NormalMap_Tex_NRM;

struct Input {
	float2 uvNormalMap_Tex_NRM;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 normalTex = tex2D(NormalMap_Tex_NRM, IN.uvNormalMap_Tex_NRM);
	normalTex.r = normalTex.a;
	normalTex.g = 1.0f - normalTex.g;
	normalTex.b = 1.0f;
	normalTex.a = 1.0f;
	o.Normal = UnpackNormal(normalTex);
}
ENDCG
}

FallBack "Standard"
}