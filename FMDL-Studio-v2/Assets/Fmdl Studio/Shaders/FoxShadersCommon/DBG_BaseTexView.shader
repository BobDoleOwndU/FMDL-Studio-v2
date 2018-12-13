Shader "FoxShaders/DBG_BaseTexView" {
Properties {
	_Lamp0Color("Lamp0Color", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Lamp0Intensity("Lamp0Intensity", Vector) = (0.0, 0.0, 0.0, 0.0)
	_AmbientLight("AmbientLight", Vector) = (0.0, 0.0, 0.0, 0.0)
	_SubNormal_UScale("SubNormal_UScale", Vector) = (0.0, 0.0, 0.0, 0.0)
	_SubNormal_VScale("SubNormal_VScale", Vector) = (0.0, 0.0, 0.0, 0.0)
	_SubNormalBlend("SubNormalBlend", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Base_Tex_SRGB("Base_Tex_SRGB", 2D) = "white" {}
	_NormalMap_Tex_LIN("NormalMap_Tex_LIN", 2D) = "white" {}
	_SubNormalMap_Tex_LIN("SubNormalMap_Tex_LIN", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}

SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	LOD 400

CGPROGRAM
#pragma surface surf BlinnPhong alphatest:_Cutoff
#pragma target 3.0

sampler2D _Base_Tex_SRGB;

struct Input {
	float2 uv_Base_Tex_SRGB;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_Base_Tex_SRGB, IN.uv_Base_Tex_SRGB);
	o.Albedo = tex.rgb;
	o.Gloss = tex.a;
	o.Alpha = tex.a;
}
ENDCG
}

FallBack "Standard"
}