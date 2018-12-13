Shader "FoxShaders/pes3DFW_Trophy_VR" {
Properties {
	_ReflectionIntensity("ReflectionIntensity", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Roughness("Roughness", Vector) = (0.0, 0.0, 0.0, 0.0)
	_NormalMap_Tex_NRM("NormalMap_Tex_NRM", 2D) = "white" {}
	_Reflection_Tex_SRGB("Reflection_Tex_SRGB", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}

SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	LOD 400

CGPROGRAM
#pragma surface surf BlinnPhong alphatest:_Cutoff
#pragma target 3.0

sampler2D _NormalMap_Tex_NRM;

struct Input {
	float2 uv_Base_Tex_SRGB;
	float2 uv_NormalMap_Tex_NRM;
};

void surf (Input IN, inout SurfaceOutput o) {
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