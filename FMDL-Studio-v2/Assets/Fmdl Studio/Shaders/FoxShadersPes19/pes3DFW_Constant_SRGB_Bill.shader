Shader "FoxShaders/pes3DFW_Constant_SRGB_Bill" {
Properties {
	_SelfColor("SelfColor", Vector) = (0.0, 0.0, 0.0, 0.0)
	_SelfColorIntensity("SelfColorIntensity", Vector) = (0.0, 0.0, 0.0, 0.0)
	_MaskRepeatParam("MaskRepeatParam", Vector) = (0.0, 0.0, 0.0, 0.0)
	_UvMatrix_0("UvMatrix_0", Vector) = (0.0, 0.0, 0.0, 0.0)
	_UvMatrix_1("UvMatrix_1", Vector) = (0.0, 0.0, 0.0, 0.0)
	_UvMatrix_2("UvMatrix_2", Vector) = (0.0, 0.0, 0.0, 0.0)
	_UvMatrix_3("UvMatrix_3", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Base_Tex_SRGB_1("Base_Tex_SRGB_1", 2D) = "white" {}
	_Base_Tex_SRGB_2("Base_Tex_SRGB_2", 2D) = "white" {}
	_Led_Mask_Tex_LIN("Led_Mask_Tex_LIN", 2D) = "white" {}
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