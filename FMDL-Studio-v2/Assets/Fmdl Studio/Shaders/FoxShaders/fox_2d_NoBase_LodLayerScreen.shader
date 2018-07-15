Shader "FoxShaders/fox_2d_NoBase_LodLayerScreen" {
Properties {
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