Shader "FoxShaders/pes3DDC_Wet_LNM_VR" {
Properties {
	_Specular("Specular", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Roughness("Roughness", Vector) = (0.0, 0.0, 0.0, 0.0)
	_AlbedoCubeScale("AlbedoCubeScale", Vector) = (0.0, 0.0, 0.0, 0.0)
	_SubNormal_Blend("SubNormal_Blend", Vector) = (0.0, 0.0, 0.0, 0.0)
	_URepeat_UV("URepeat_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
	_VRepeat_UV("VRepeat_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
	_UScroll_UV("UScroll_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
	_VScroll_UV("VScroll_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Dirty_Mask("Dirty_Mask", Vector) = (0.0, 0.0, 0.0, 0.0)
	_TimeSecond("TimeSecond", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Base_Tex_SRGB("Base_Tex_SRGB", 2D) = "white" {}
	_NormalMap_Tex_NRM("NormalMap_Tex_NRM", 2D) = "white" {}
	_Mask_Tex_LIN("Mask_Tex_LIN", 2D) = "white" {}
	_SubNormalMap_Tex_NRM("SubNormalMap_Tex_NRM", 2D) = "white" {}
	_CubeMap_Tex_LIN("CubeMap_Tex_LIN", 2D) = "white" {}
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

struct Input {
	float2 uv_Base_Tex_SRGB;
	float2 uv_NormalMap_Tex_NRM;
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
}
ENDCG
}

FallBack "Standard"
}