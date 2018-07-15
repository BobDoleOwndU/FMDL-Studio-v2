Shader "FoxShaders/tpp_2d_Map_ScreenMaskPAR" {
Properties {
	HeightLimit0("HeightLimit0", Vector) = (0.0, 0.0, 0.0, 0.0)
	HeightLimit1("HeightLimit1", Vector) = (0.0, 0.0, 0.0, 0.0)
	ParHeight("ParHeight", Vector) = (0.0, 0.0, 0.0, 0.0)
	TangentDir("TangentDir", Vector) = (0.0, 0.0, 0.0, 0.0)
	UCenter("UCenter", Vector) = (0.0, 0.0, 0.0, 0.0)
	VCenter("VCenter", Vector) = (0.0, 0.0, 0.0, 0.0)
	UShift("UShift", Vector) = (0.0, 0.0, 0.0, 0.0)
	VShift("VShift", Vector) = (0.0, 0.0, 0.0, 0.0)
	URepeat("URepeat", Vector) = (0.0, 0.0, 0.0, 0.0)
	VRepeat("VRepeat", Vector) = (0.0, 0.0, 0.0, 0.0)
	UShift_Mask("UShift_Mask", Vector) = (0.0, 0.0, 0.0, 0.0)
	VShift_Mask("VShift_Mask", Vector) = (0.0, 0.0, 0.0, 0.0)
	LoadColor("LoadColor", Vector) = (0.0, 0.0, 0.0, 0.0)
	UShift_AnimMaskTex("UShift_AnimMaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	VShift_AnimMaskTex("VShift_AnimMaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	URepeat_AnimMaskTex("URepeat_AnimMaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	VRepeat_AnimMaskTex("VRepeat_AnimMaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	Blend_AnimMaskTex("Blend_AnimMaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	URepeat_Grid("URepeat_Grid", Vector) = (0.0, 0.0, 0.0, 0.0)
	VRepeat_Grid("VRepeat_Grid", Vector) = (0.0, 0.0, 0.0, 0.0)
	BevelBlendRate("BevelBlendRate", Vector) = (0.0, 0.0, 0.0, 0.0)
	CameraPos("CameraPos", Vector) = (0.0, 0.0, 0.0, 0.0)
	Height_Texture("Height_Texture", 2D) = "white" {}
	ColorTable_Texture("ColorTable_Texture", 2D) = "white" {}
	Mask_Texture("Mask_Texture", 2D) = "white" {}
	AnimMask_Texture("AnimMask_Texture", 2D) = "white" {}
	Grid_Texture("Grid_Texture", 2D) = "white" {}
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