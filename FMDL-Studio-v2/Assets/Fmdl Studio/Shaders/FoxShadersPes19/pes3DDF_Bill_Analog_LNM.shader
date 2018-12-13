Shader "FoxShaders/pes3DDF_Bill_Analog_LNM" {
Properties {
	_MatParamIndex_0("MatParamIndex_0", Vector) = (0.0, 0.0, 0.0, 0.0)
	_MatParamIndex_1("MatParamIndex_1", Vector) = (0.0, 0.0, 0.0, 0.0)
	_UvMatrix_0("UvMatrix_0", Vector) = (0.0, 0.0, 0.0, 0.0)
	_UvMatrix_1("UvMatrix_1", Vector) = (0.0, 0.0, 0.0, 0.0)
	_UvMatrix_2("UvMatrix_2", Vector) = (0.0, 0.0, 0.0, 0.0)
	_UvMatrix_3("UvMatrix_3", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Base_Tex_SRGB_1("Base_Tex_SRGB_1", 2D) = "white" {}
	_NormalMap_Tex_NRM("NormalMap_Tex_NRM", 2D) = "white" {}
	_SpecularMap_Tex_LIN("SpecularMap_Tex_LIN", 2D) = "white" {}
	_Base_Tex_SRGB_2("Base_Tex_SRGB_2", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}

SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	LOD 400

CGPROGRAM
#pragma surface surf BlinnPhong alphatest:_Cutoff
#pragma target 3.0

sampler2D _NormalMap_Tex_NRM;
sampler2D _SpecularMap_Tex_LIN;

struct Input {
	float2 uv_Base_Tex_SRGB;
	float2 uv_NormalMap_Tex_NRM;
	float2 uv_SpecularMap_Tex_LIN;
};

void surf (Input IN, inout SurfaceOutput o) {
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