Shader "FoxShaders/tpp3DFW_TargetMarker" {
Properties {
	_SelfColor("SelfColor", Vector) = (0.0, 0.0, 0.0, 0.0)
	_ColorOffset("ColorOffset", Vector) = (0.0, 0.0, 0.0, 0.0)
	_SelfColorIntensity("SelfColorIntensity", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Incidence_Roughness("Incidence_Roughness", Vector) = (0.0, 0.0, 0.0, 0.0)
	_URepeat_ScreenTex("URepeat_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	_VRepeat_ScreenTex("VRepeat_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	_UShift_ScreenTex("UShift_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	_VShift_ScreenTex("VShift_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	_SelfColor2("SelfColor2", Vector) = (0.0, 0.0, 0.0, 0.0)
	_SelfColorBlend("SelfColorBlend", Vector) = (0.0, 0.0, 0.0, 0.0)
	_SelfColorBlendScale("SelfColorBlendScale", Vector) = (0.0, 0.0, 0.0, 0.0)
	_ViewRangeMax("ViewRangeMax", Vector) = (0.0, 0.0, 0.0, 0.0)
	_ViewRangeMin("ViewRangeMin", Vector) = (0.0, 0.0, 0.0, 0.0)
	_ViewRangeLong("ViewRangeLong", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Noise_Tex_LIN("Noise_Tex_LIN", 2D) = "white" {}
	_Gradient_Tex_LIN("Gradient_Tex_LIN", 2D) = "white" {}
	_Depth_Tex_LIN("Depth_Tex_LIN", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}

SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	LOD 400

CGPROGRAM
#pragma surface surf BlinnPhong alphatest:_Cutoff
#pragma target 3.0


struct Input {
	float2 uv_Base_Tex_SRGB;
};

void surf (Input IN, inout SurfaceOutput o) {
}
ENDCG
}

FallBack "Standard"
}