Shader "FoxShaders/fox3DDF_PixcelWave_LNM" {
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
	_PixcelWaveCenterV("PixcelWaveCenterV", Vector) = (0.0, 0.0, 0.0, 0.0)
	_PixcelWaveScale("PixcelWaveScale", Vector) = (0.0, 0.0, 0.0, 0.0)
	_NegWaveScale("NegWaveScale", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Base_Tex_SRGB("Base_Tex_SRGB", 2D) = "white" {}
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