Shader "FoxShaders/fox3DDF_PixcelWave" {
Properties {
	MatParamIndex_0("MatParamIndex_0", Vector) = (0.0, 0.0, 0.0, 0.0)
	Specular_Value("Specular_Value", Vector) = (0.0, 0.0, 0.0, 0.0)
	Roughness_Value("Roughness_Value", Vector) = (0.0, 0.0, 0.0, 0.0)
	Translucent_Value("Translucent_Value", Vector) = (0.0, 0.0, 0.0, 0.0)
	WindDir("WindDir", Vector) = (0.0, 0.0, 0.0, 0.0)
	WindAnimTime("WindAnimTime", Vector) = (0.0, 0.0, 0.0, 0.0)
	WindAmplitude("WindAmplitude", Vector) = (0.0, 0.0, 0.0, 0.0)
	WeightDiffusion("WeightDiffusion", Vector) = (0.0, 0.0, 0.0, 0.0)
	WeightOffset("WeightOffset", Vector) = (0.0, 0.0, 0.0, 0.0)
	WindOffset("WindOffset", Vector) = (0.0, 0.0, 0.0, 0.0)
	OldWindDir("OldWindDir", Vector) = (0.0, 0.0, 0.0, 0.0)
	NormalRotRate("NormalRotRate", Vector) = (0.0, 0.0, 0.0, 0.0)
	OldWindAnimTime("OldWindAnimTime", Vector) = (0.0, 0.0, 0.0, 0.0)
	OldWindAmplitude("OldWindAmplitude", Vector) = (0.0, 0.0, 0.0, 0.0)
	WindRandAmplitude("WindRandAmplitude", Vector) = (0.0, 0.0, 0.0, 0.0)
	PixcelWaveCenterV("PixcelWaveCenterV", Vector) = (0.0, 0.0, 0.0, 0.0)
	PixcelWaveScale("PixcelWaveScale", Vector) = (0.0, 0.0, 0.0, 0.0)
	NegWaveScale("NegWaveScale", Vector) = (0.0, 0.0, 0.0, 0.0)
	Base_Tex_SRGB("Base_Tex_SRGB", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}

SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	LOD 400

CGPROGRAM
#pragma surface surf BlinnPhong alphatest:_Cutoff
#pragma target 3.0

sampler2D Base_Tex_SRGB;

struct Input {
	float2 uvBase_Tex_SRGB;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(Base_Tex_SRGB, IN.uvBase_Tex_SRGB);
	o.Albedo = tex.rgb;
	o.Gloss = tex.a;
	o.Alpha = tex.a;
}
ENDCG
}

FallBack "Standard"
}