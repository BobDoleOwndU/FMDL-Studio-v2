Shader "FoxShaders/tpp3DFW_Star" {
Properties {
	SelfColor("SelfColor", Vector) = (0.0, 0.0, 0.0, 0.0)
	TwincleWeight("TwincleWeight", Vector) = (0.0, 0.0, 0.0, 0.0)
	TwincleOffsetX("TwincleOffsetX", Vector) = (0.0, 0.0, 0.0, 0.0)
	TwincleOffsetZ("TwincleOffsetZ", Vector) = (0.0, 0.0, 0.0, 0.0)
	TppTonemap("TppTonemap", Vector) = (0.0, 0.0, 0.0, 0.0)
	Dummy0("Dummy0", Vector) = (0.0, 0.0, 0.0, 0.0)
	Dummy1("Dummy1", Vector) = (0.0, 0.0, 0.0, 0.0)
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