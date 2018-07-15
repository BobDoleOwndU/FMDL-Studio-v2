Shader "FoxShaders/fox3DFW_WaterCover_VR" {
Properties {
	MatParamIndex_0("MatParamIndex_0", Vector) = (0.0, 0.0, 0.0, 0.0)
	WaterTrans("WaterTrans", Vector) = (0.0, 0.0, 0.0, 0.0)
	WaterRoughness("WaterRoughness", Vector) = (0.0, 0.0, 0.0, 0.0)
	Incidence_Roughness("Incidence_Roughness", Vector) = (0.0, 0.0, 0.0, 0.0)
	Incidence_Color("Incidence_Color", Vector) = (0.0, 0.0, 0.0, 0.0)
	SubNormal_Blend("SubNormal_Blend", Vector) = (0.0, 0.0, 0.0, 0.0)
	URepeat_UV("URepeat_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
	VRepeat_UV("VRepeat_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
	UShift_UV("UShift_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
	VShift_UV("VShift_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
	Base_Tex_LIN("Base_Tex_LIN", 2D) = "white" {}
	NormalMap_Tex_NRM("NormalMap_Tex_NRM", 2D) = "white" {}
	SpheremapReflection_Tex_LIN("SpheremapReflection_Tex_LIN", 2D) = "white" {}
	SubNormalMap_Tex_NRM("SubNormalMap_Tex_NRM", 2D) = "white" {}
	SubNormalMask_Tex_LIN("SubNormalMask_Tex_LIN", 2D) = "white" {}
	AnimMask_Tex_LIN("AnimMask_Tex_LIN", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}

SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	LOD 400

CGPROGRAM
#pragma surface surf BlinnPhong alphatest:_Cutoff
#pragma target 3.0

sampler2D NormalMap_Tex_NRM;

struct Input {
	float2 uvNormalMap_Tex_NRM;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 normalTex = tex2D(NormalMap_Tex_NRM, IN.uvNormalMap_Tex_NRM);
	normalTex.r = normalTex.a;
	normalTex.g = 1.0f - normalTex.g;
	normalTex.b = 1.0f;
	normalTex.a = 1.0f;
	o.Normal = UnpackNormal(normalTex);
}
ENDCG
}

FallBack "Standard"
}