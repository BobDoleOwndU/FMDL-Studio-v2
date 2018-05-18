Shader "FoxShaders/fox3DFW_Constant_Auto" {
Properties {
	SelfColor("SelfColor", Vector) = (0.0, 0.0, 0.0, 0.0)
	SelfColorIntensityMin("SelfColorIntensityMin", Vector) = (0.0, 0.0, 0.0, 0.0)
	SelfColorIntensityMax("SelfColorIntensityMax", Vector) = (0.0, 0.0, 0.0, 0.0)
	ToneMapExposureMin("ToneMapExposureMin", Vector) = (0.0, 0.0, 0.0, 0.0)
	ToneMapExposureMax("ToneMapExposureMax", Vector) = (0.0, 0.0, 0.0, 0.0)
	Mask_Tex_LIN("Mask_Tex_LIN", 2D) = "white" {}
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