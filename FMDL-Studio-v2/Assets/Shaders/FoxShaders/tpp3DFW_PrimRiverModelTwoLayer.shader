Shader "FoxShaders/tpp3DFW_PrimRiverModelTwoLayer" {
Properties {
	Base_Tex_SRGB("Base_Tex_SRGB", 2D) = "white" {}
	SecondBase_Tex_SRGB("SecondBase_Tex_SRGB", 2D) = "white" {}
	NormalMap_Tex_NRM("NormalMap_Tex_NRM", 2D) = "white" {}
	Mask_Tex_LIN("Mask_Tex_LIN", 2D) = "white" {}
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

struct Input {
	float2 uvBase_Tex_SRGB;
	float2 uvNormalMap_Tex_NRM;
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
}
ENDCG
}

FallBack "Standard"
}