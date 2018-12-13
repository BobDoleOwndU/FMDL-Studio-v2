Shader "FoxShaders/pes3DDF_Crowd_Mob_LNM" {
Properties {
	_MatParamIndex_0("MatParamIndex_0", Vector) = (0.0, 0.0, 0.0, 0.0)
	_FaceId("FaceId", Vector) = (0.0, 0.0, 0.0, 0.0)
	_FacialId("FacialId", Vector) = (0.0, 0.0, 0.0, 0.0)
	_ShirtId("ShirtId", Vector) = (0.0, 0.0, 0.0, 0.0)
	_ClothId("ClothId", Vector) = (0.0, 0.0, 0.0, 0.0)
	_IsUniform("IsUniform", Vector) = (0.0, 0.0, 0.0, 0.0)
	_IsAway("IsAway", Vector) = (0.0, 0.0, 0.0, 0.0)
	_MufflerId("MufflerId", Vector) = (0.0, 0.0, 0.0, 0.0)
	_ColorId("ColorId", Vector) = (0.0, 0.0, 0.0, 0.0)
	_InvisibleFlags("InvisibleFlags", Vector) = (0.0, 0.0, 0.0, 0.0)
	_RaceId("RaceId", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Base_Tex_SRGB("Base_Tex_SRGB", 2D) = "white" {}
	_NormalMap_Tex_NRM("NormalMap_Tex_NRM", 2D) = "white" {}
	_SpecularMap_Tex_LIN("SpecularMap_Tex_LIN", 2D) = "white" {}
	_Composite_Tex_SRGB("Composite_Tex_SRGB", 2D) = "white" {}
	_VariationFace_Tex_SRGB("VariationFace_Tex_SRGB", 2D) = "white" {}
	_Mask_Tex_LIN("Mask_Tex_LIN", 2D) = "white" {}
	_ColorParameter_Tex_SRGB("ColorParameter_Tex_SRGB", 2D) = "white" {}
	_TeamColorParameter_Tex_SRGB("TeamColorParameter_Tex_SRGB", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}

SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	LOD 400

CGPROGRAM
#pragma surface surf BlinnPhong alphatest:_Cutoff
#pragma target 3.0

sampler2D _Base_Tex_SRGB;
sampler2D _NormalMap_Tex_NRM;
sampler2D _SpecularMap_Tex_LIN;

struct Input {
	float2 uv_Base_Tex_SRGB;
	float2 uv_NormalMap_Tex_NRM;
	float2 uv_SpecularMap_Tex_LIN;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_Base_Tex_SRGB, IN.uv_Base_Tex_SRGB);
	o.Albedo = tex.rgb;
	o.Gloss = tex.a;
	o.Alpha = tex.a;
	fixed4 normalTex = tex2D(_NormalMap_Tex_NRM, IN.uv_NormalMap_Tex_NRM);
	normalTex.r = normalTex.a;
	normalTex.g = 1.0f - normalTex.g;
	normalTex.b = 1.0f;
	normalTex.a = 1.0f;
	o.Normal = UnpackNormal(normalTex);
	fixed4 specularTex = tex2D(_SpecularMap_Tex_LIN, IN.uv_SpecularMap_Tex_LIN);
	o.Specular = specularTex.r;
}
ENDCG
}

FallBack "Standard"
}