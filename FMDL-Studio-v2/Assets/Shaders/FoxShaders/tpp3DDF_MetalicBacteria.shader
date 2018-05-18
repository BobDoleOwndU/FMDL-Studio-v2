Shader "FoxShaders/tpp3DDF_MetalicBacteria" {
Properties {
	MatParamIndex_0("MatParamIndex_0", Vector) = (0.0, 0.0, 0.0, 0.0)
	Incidence_Roughness("Incidence_Roughness", Vector) = (0.0, 0.0, 0.0, 0.0)
	TensionRate("TensionRate", Vector) = (0.0, 0.0, 0.0, 0.0)
	TensionShift("TensionShift", Vector) = (0.0, 0.0, 0.0, 0.0)
	TensionController("TensionController", Vector) = (0.0, 0.0, 0.0, 0.0)
	Incidence_Color("Incidence_Color", Vector) = (0.0, 0.0, 0.0, 0.0)
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
	Translucent_Tex_LIN("Translucent_Tex_LIN", 2D) = "white" {}
	MetalicLayer_Tex_LIN("MetalicLayer_Tex_LIN", 2D) = "white" {}
	MetalicBacteria_Tex_LIN("MetalicBacteria_Tex_LIN", 2D) = "white" {}
	TensionSubNormalMap_Tex_NRM("TensionSubNormalMap_Tex_NRM", 2D) = "white" {}
	Dirty_Tex_LIN("Dirty_Tex_LIN", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}

SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	LOD 400

CGPROGRAM
#pragma surface surf BlinnPhong alphatest:_Cutoff
#pragma target 3.0

sampler2D Base_Tex_SRGB;
sampler2D NormalMap_Tex_NRM;
sampler2D SpecularMap_Tex_LIN;

struct Input {
	float2 uvBase_Tex_SRGB;
	float2 uvNormalMap_Tex_NRM;
	float2 uvSpecularMap_Tex_LIN;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(Base_Tex_SRGB, IN.uvBase_Tex_SRGB);
	o.Albedo = tex.rgb;
	o.Gloss = tex.a;
	o.Alpha = tex.a;
	fixed4 normalTex = tex2D(NormalMap_Tex_NRM, IN.uvNormalMap_Tex_NRM);
	normalTex.r = normalTex.a;
	normalTex.g = 1.0f - normalTex.g;
	normalTex.b = 1.0f;
	normalTex.a = 1.0f;
	o.Normal = UnpackNormal(normalTex);
	fixed4 specularTex = tex2D(SpecularMap_Tex_LIN, IN.uvSpecularMap_Tex_LIN);
	o.Specular = specularTex.r;
}
ENDCG
}

FallBack "Standard"
}