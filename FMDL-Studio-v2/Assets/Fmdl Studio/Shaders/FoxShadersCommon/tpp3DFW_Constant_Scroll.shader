Shader "FoxShaders/tpp3DFW_Constant_Scroll" {
Properties {
	_SelfColor("SelfColor", Vector) = (0.0, 0.0, 0.0, 0.0)
	_SelfColorIntensity("SelfColorIntensity", Vector) = (0.0, 0.0, 0.0, 0.0)
	_URepeat_UV("URepeat_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
	_VRepeat_UV("VRepeat_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
	_UShift_UV("UShift_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
	_VShift_UV("VShift_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Mask_Tex_LIN("Mask_Tex_LIN", 2D) = "white" {}
	_ScrollLayer_Tex_LIN("ScrollLayer_Tex_LIN", 2D) = "white" {}
	_ScrolMask_Tex_LIN("ScrolMask_Tex_LIN", 2D) = "white" {}
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