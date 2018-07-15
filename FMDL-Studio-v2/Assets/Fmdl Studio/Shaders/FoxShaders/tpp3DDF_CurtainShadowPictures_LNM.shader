Shader "FoxShaders/tpp3DDF_CurtainShadowPictures_LNM" {
Properties {
	MatParamIndex_0("MatParamIndex_0", Vector) = (0.0, 0.0, 0.0, 0.0)
	SubNormal_Blend("SubNormal_Blend", Vector) = (0.0, 0.0, 0.0, 0.0)
	URepeat_UV("URepeat_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
	VRepeat_UV("VRepeat_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
	CylinderCenter("CylinderCenter", Vector) = (0.0, 0.0, 0.0, 0.0)
	CylinderScale_U("CylinderScale_U", Vector) = (0.0, 0.0, 0.0, 0.0)
	CylinderScale_V("CylinderScale_V", Vector) = (0.0, 0.0, 0.0, 0.0)
	CylinderShiftAngle("CylinderShiftAngle", Vector) = (0.0, 0.0, 0.0, 0.0)
	CylinderShiftAngleUp("CylinderShiftAngleUp", Vector) = (0.0, 0.0, 0.0, 0.0)
	LightScale_0("LightScale_0", Vector) = (0.0, 0.0, 0.0, 0.0)
	LightScale_1("LightScale_1", Vector) = (0.0, 0.0, 0.0, 0.0)
	Translucent_Value("Translucent_Value", Vector) = (0.0, 0.0, 0.0, 0.0)
	ShadowColor("ShadowColor", Vector) = (0.0, 0.0, 0.0, 0.0)
	LightColor("LightColor", Vector) = (0.0, 0.0, 0.0, 0.0)
	Base_Tex_SRGB("Base_Tex_SRGB", 2D) = "white" {}
	NormalMap_Tex_NRM("NormalMap_Tex_NRM", 2D) = "white" {}
	SpecularMap_Tex_LIN("SpecularMap_Tex_LIN", 2D) = "white" {}
	RoughnessMap_Tex_LIN("RoughnessMap_Tex_LIN", 2D) = "white" {}
	CylindricalLayer_Tex_LIN("CylindricalLayer_Tex_LIN", 2D) = "white" {}
	LightAdd_Tex_LIN("LightAdd_Tex_LIN", 2D) = "white" {}
	SubNormalMap_Tex_NRM("SubNormalMap_Tex_NRM", 2D) = "white" {}
	Dirty_Tex_LIN("Dirty_Tex_LIN", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}

SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	LOD 400

CGPROGRAM
#pragma surface surf BlinnPhong alphatest:_Cutoff
#pragma target 3.0

sampler2D Base_Tex_SRGB;
sampler2D NormalMap_Tex_NRM;
sampler2D SpecularMap_Tex_LIN;

struct Input {
	float2 uvBase_Tex_SRGB;
	float2 uvNormalMap_Tex_NRM;
	float2 uvSpecularMap_Tex_LIN;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(Base_Tex_SRGB, IN.uvBase_Tex_SRGB);
	o.Albedo = tex.rgb;
	o.Gloss = tex.a;
	o.Alpha = tex.a;
	fixed4 normalTex = tex2D(NormalMap_Tex_NRM, IN.uvNormalMap_Tex_NRM);
	normalTex.r = normalTex.a;
	normalTex.g = 1.0f - normalTex.g;
	normalTex.b = 1.0f;
	normalTex.a = 1.0f;
	o.Normal = UnpackNormal(normalTex);
	fixed4 specularTex = tex2D(SpecularMap_Tex_LIN, IN.uvSpecularMap_Tex_LIN);
	o.Specular = specularTex.r;
}
ENDCG
}

FallBack "Standard"
}