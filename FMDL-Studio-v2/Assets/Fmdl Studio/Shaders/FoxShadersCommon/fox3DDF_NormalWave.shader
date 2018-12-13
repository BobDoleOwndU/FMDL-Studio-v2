Shader "FoxShaders/fox3DDF_NormalWave" {
Properties {
	_MatParamIndex_0("MatParamIndex_0", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Specular_Value("Specular_Value", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Roughness_Value("Roughness_Value", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Translucent_Value("Translucent_Value", Vector) = (0.0, 0.0, 0.0, 0.0)
	_WindDir("WindDir", Vector) = (0.0, 0.0, 0.0, 0.0)
	_WindAnimTime("WindAnimTime", Vector) = (0.0, 0.0, 0.0, 0.0)
	_WindAmplitude("WindAmplitude", Vector) = (0.0, 0.0, 0.0, 0.0)
	_WeightDiffusion("WeightDiffusion", Vector) = (0.0, 0.0, 0.0, 0.0)
	_WeightOffset("WeightOffset", Vector) = (0.0, 0.0, 0.0, 0.0)
	_WindOffset("WindOffset", Vector) = (0.0, 0.0, 0.0, 0.0)
	_OldWindDir("OldWindDir", Vector) = (0.0, 0.0, 0.0, 0.0)
	_NormalRotRate("NormalRotRate", Vector) = (0.0, 0.0, 0.0, 0.0)
	_OldWindAnimTime("OldWindAnimTime", Vector) = (0.0, 0.0, 0.0, 0.0)
	_OldWindAmplitude("OldWindAmplitude", Vector) = (0.0, 0.0, 0.0, 0.0)
	_WindRandAmplitude("WindRandAmplitude", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Base_Tex_SRGB("Base_Tex_SRGB", 2D) = "white" {}
	_NormalMap_Tex_NRM("NormalMap_Tex_NRM", 2D) = "white" {}
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