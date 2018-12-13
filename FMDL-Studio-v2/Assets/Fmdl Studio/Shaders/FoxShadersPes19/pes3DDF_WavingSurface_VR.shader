Shader "FoxShaders/pes3DDF_WavingSurface_VR" {
Properties {
	_MatParamIndex_0("MatParamIndex_0", Vector) = (0.0, 0.0, 0.0, 0.0)
	_SelfColor("SelfColor", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Wave0Amplitude("Wave0Amplitude", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Wave0Frequency("Wave0Frequency", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Wave0Speed("Wave0Speed", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Wave0Direction("Wave0Direction", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Wave1Amplitude("Wave1Amplitude", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Wave1Frequency("Wave1Frequency", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Wave1Speed("Wave1Speed", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Wave1Direction("Wave1Direction", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Wave2Amplitude("Wave2Amplitude", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Wave2Frequency("Wave2Frequency", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Wave2Speed("Wave2Speed", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Wave2Direction("Wave2Direction", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Wave3Amplitude("Wave3Amplitude", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Wave3Frequency("Wave3Frequency", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Wave3Speed("Wave3Speed", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Wave3Direction("Wave3Direction", Vector) = (0.0, 0.0, 0.0, 0.0)
	_WaveCurTime("WaveCurTime", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Base_Tex_SRGB("Base_Tex_SRGB", 2D) = "white" {}
	_NormalMap_Tex_NRM("NormalMap_Tex_NRM", 2D) = "white" {}
	_SpecularMap_Tex_LIN("SpecularMap_Tex_LIN", 2D) = "white" {}
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