Shader "FoxShaders/pes3DDF_Pitch_LNM" {
Properties {
	_MatParamIndex_0("MatParamIndex_0", Vector) = (0.0, 0.0, 0.0, 0.0)
	_AOValue("AOValue", Vector) = (0.0, 0.0, 0.0, 0.0)
	_AOFresnel("AOFresnel", Vector) = (0.0, 0.0, 0.0, 0.0)
	_SubNormal_Blend("SubNormal_Blend", Vector) = (0.0, 0.0, 0.0, 0.0)
	_URepeat_UV("URepeat_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
	_VRepeat_UV("VRepeat_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
	_URepeat_SubNorm_UV("URepeat_SubNorm_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
	_VRepeat_SubNorm_UV("VRepeat_SubNorm_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
	_SphereScaleX("SphereScaleX", Vector) = (0.0, 0.0, 0.0, 0.0)
	_SphereScaleY("SphereScaleY", Vector) = (0.0, 0.0, 0.0, 0.0)
	_SphereScaleZ("SphereScaleZ", Vector) = (0.0, 0.0, 0.0, 0.0)
	_BackFaceScaleX("BackFaceScaleX", Vector) = (0.0, 0.0, 0.0, 0.0)
	_BackFaceScaleY("BackFaceScaleY", Vector) = (0.0, 0.0, 0.0, 0.0)
	_BackFaceScaleZ("BackFaceScaleZ", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Base_Tex_SRGB("Base_Tex_SRGB", 2D) = "white" {}
	_NormalMap_Tex_NRM("NormalMap_Tex_NRM", 2D) = "white" {}
	_SpecularMap_Tex_LIN("SpecularMap_Tex_LIN", 2D) = "white" {}
	_Base_Tex_2_SRGB("Base_Tex_2_SRGB", 2D) = "white" {}
	_GrainNormal_Tex_LIN("GrainNormal_Tex_LIN", 2D) = "white" {}
	_GroundBase_Tex_SRGB("GroundBase_Tex_SRGB", 2D) = "white" {}
	_Mask_Tex_SRGB("Mask_Tex_SRGB", 2D) = "white" {}
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