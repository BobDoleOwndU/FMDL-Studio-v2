Shader "FoxShaders/fox_2d_Basic_LyMUL_LodMask" {
Properties {
	UCenter_BaseTex("UCenter_BaseTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	VCenter_BaseTex("VCenter_BaseTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	UShift_BaseTex("UShift_BaseTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	VShift_BaseTex("VShift_BaseTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	URepeat_BaseTex("URepeat_BaseTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	VRepeat_BaseTex("VRepeat_BaseTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	UCenter_LayerTex("UCenter_LayerTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	VCenter_LayerTex("VCenter_LayerTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	UShift_LayerTex("UShift_LayerTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	VShift_LayerTex("VShift_LayerTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	URepeat_LayerTex("URepeat_LayerTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	VRepeat_LayerTex("VRepeat_LayerTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	UCenter_MaskTex("UCenter_MaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	VCenter_MaskTex("VCenter_MaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	UShift_MaskTex("UShift_MaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	VShift_MaskTex("VShift_MaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	URepeat_MaskTex("URepeat_MaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	VRepeat_MaskTex("VRepeat_MaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	UCenter_ScreenTex("UCenter_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	VCenter_ScreenTex("VCenter_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	UShift_ScreenTex("UShift_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	VShift_ScreenTex("VShift_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	URepeat_ScreenTex("URepeat_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	VRepeat_ScreenTex("VRepeat_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	Blend_BaseTex("Blend_BaseTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	Blend_LayerTex("Blend_LayerTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	Blend_MaskTex("Blend_MaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	Blend_ScreenTex("Blend_ScreenTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	Base_Texture("Base_Texture", 2D) = "white" {}
	Layer_Texture("Layer_Texture", 2D) = "white" {}
	Mask_Texture("Mask_Texture", 2D) = "white" {}
	Screen_Texture("Screen_Texture", 2D) = "white" {}
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