Shader "FoxShaders/tpp3DFW_TargetMarker" {
Properties {
	SelfColor("SelfColor", Vector) = (0.0, 0.0, 0.0, 0.0)
	ColorOffset("ColorOffset", Vector) = (0.0, 0.0, 0.0, 0.0)
	SelfColorIntensity("SelfColorIntensity", Vector) = (0.0, 0.0, 0.0, 0.0)
	Incidence_Roughness("Incidence_Roughness", Vector) = (0.0, 0.0, 0.0, 0.0)
	URepeat_ScreenTex("URepeat_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	VRepeat_ScreenTex("VRepeat_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	UShift_ScreenTex("UShift_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	VShift_ScreenTex("VShift_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	SelfColor2("SelfColor2", Vector) = (0.0, 0.0, 0.0, 0.0)
	SelfColorBlend("SelfColorBlend", Vector) = (0.0, 0.0, 0.0, 0.0)
	SelfColorBlendScale("SelfColorBlendScale", Vector) = (0.0, 0.0, 0.0, 0.0)
	ViewRangeMax("ViewRangeMax", Vector) = (0.0, 0.0, 0.0, 0.0)
	ViewRangeMin("ViewRangeMin", Vector) = (0.0, 0.0, 0.0, 0.0)
	ViewRangeLong("ViewRangeLong", Vector) = (0.0, 0.0, 0.0, 0.0)
	Noise_Tex_LIN("Noise_Tex_LIN", 2D) = "white" {}
	Gradient_Tex_LIN("Gradient_Tex_LIN", 2D) = "white" {}
	Depth_Tex_LIN("Depth_Tex_LIN", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}

SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	LOD 400

CGPROGRAM
#pragma surface surf BlinnPhong alphatest:_Cutoff
#pragma target 3.0

void surf (Input IN, inout SurfaceOutput o) {
}
ENDCG
}

FallBack "Standard"
}