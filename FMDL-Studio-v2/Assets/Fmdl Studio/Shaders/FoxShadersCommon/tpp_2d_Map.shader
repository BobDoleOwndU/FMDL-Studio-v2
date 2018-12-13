Shader "FoxShaders/tpp_2d_Map" {
Properties {
	_HeightLimit0("HeightLimit0", Vector) = (0.0, 0.0, 0.0, 0.0)
	_HeightLimit1("HeightLimit1", Vector) = (0.0, 0.0, 0.0, 0.0)
	_ParHeight("ParHeight", Vector) = (0.0, 0.0, 0.0, 0.0)
	_TangentDir("TangentDir", Vector) = (0.0, 0.0, 0.0, 0.0)
	_UCenter("UCenter", Vector) = (0.0, 0.0, 0.0, 0.0)
	_VCenter("VCenter", Vector) = (0.0, 0.0, 0.0, 0.0)
	_UShift("UShift", Vector) = (0.0, 0.0, 0.0, 0.0)
	_VShift("VShift", Vector) = (0.0, 0.0, 0.0, 0.0)
	_URepeat("URepeat", Vector) = (0.0, 0.0, 0.0, 0.0)
	_VRepeat("VRepeat", Vector) = (0.0, 0.0, 0.0, 0.0)
	_UShift_Mask("UShift_Mask", Vector) = (0.0, 0.0, 0.0, 0.0)
	_VShift_Mask("VShift_Mask", Vector) = (0.0, 0.0, 0.0, 0.0)
	_LoadColor("LoadColor", Vector) = (0.0, 0.0, 0.0, 0.0)
	_UShift_AnimMaskTex("UShift_AnimMaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	_VShift_AnimMaskTex("VShift_AnimMaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	_URepeat_AnimMaskTex("URepeat_AnimMaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	_VRepeat_AnimMaskTex("VRepeat_AnimMaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Blend_AnimMaskTex("Blend_AnimMaskTex", Vector) = (0.0, 0.0, 0.0, 0.0)
	_URepeat_Grid("URepeat_Grid", Vector) = (0.0, 0.0, 0.0, 0.0)
	_VRepeat_Grid("VRepeat_Grid", Vector) = (0.0, 0.0, 0.0, 0.0)
	_BevelBlendRate("BevelBlendRate", Vector) = (0.0, 0.0, 0.0, 0.0)
	_CameraPos("CameraPos", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Height_Texture("Height_Texture", 2D) = "white" {}
	_ColorTable_Texture("ColorTable_Texture", 2D) = "white" {}
	_Mask_Texture("Mask_Texture", 2D) = "white" {}
	_AnimMask_Texture("AnimMask_Texture", 2D) = "white" {}
	_Grid_Texture("Grid_Texture", 2D) = "white" {}
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