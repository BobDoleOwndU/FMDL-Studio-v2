Shader "FoxShaders/fox_2d_NoBase" {
Properties {
	_UCenter_MaskTex("UCenter_MaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	_VCenter_MaskTex("VCenter_MaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	_UShift_MaskTex("UShift_MaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	_VShift_MaskTex("VShift_MaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	_URepeat_MaskTex("URepeat_MaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	_VRepeat_MaskTex("VRepeat_MaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	_UCenter_ScreenTex("UCenter_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	_VCenter_ScreenTex("VCenter_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	_UShift_ScreenTex("UShift_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	_VShift_ScreenTex("VShift_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	_URepeat_ScreenTex("URepeat_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	_VRepeat_ScreenTex("VRepeat_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Blend_BaseTex("Blend_BaseTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Blend_LayerTex("Blend_LayerTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Blend_MaskTex("Blend_MaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Blend_ScreenTex("Blend_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Mask_Texture("Mask_Texture", 2D) = "white" {}
	_Screen_Texture("Screen_Texture", 2D) = "white" {}
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