Shader "FoxShaders/tpp3DDF_CurtainShadowPictures" {
Properties {
	_MatParamIndex_0("MatParamIndex_0", Vector) = (0.0, 0.0, 0.0, 0.0)
	_SubNormal_Blend("SubNormal_Blend", Vector) = (0.0, 0.0, 0.0, 0.0)
	_URepeat_UV("URepeat_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
	_VRepeat_UV("VRepeat_UV", Vector) = (0.0, 0.0, 0.0, 0.0)
	_CylinderCenter("CylinderCenter", Vector) = (0.0, 0.0, 0.0, 0.0)
	_CylinderScale_U("CylinderScale_U", Vector) = (0.0, 0.0, 0.0, 0.0)
	_CylinderScale_V("CylinderScale_V", Vector) = (0.0, 0.0, 0.0, 0.0)
	_CylinderShiftAngle("CylinderShiftAngle", Vector) = (0.0, 0.0, 0.0, 0.0)
	_CylinderShiftAngleUp("CylinderShiftAngleUp", Vector) = (0.0, 0.0, 0.0, 0.0)
	_LightScale_0("LightScale_0", Vector) = (0.0, 0.0, 0.0, 0.0)
	_LightScale_1("LightScale_1", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Translucent_Value("Translucent_Value", Vector) = (0.0, 0.0, 0.0, 0.0)
	_ShadowColor("ShadowColor", Vector) = (0.0, 0.0, 0.0, 0.0)
	_LightColor("LightColor", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Base_Tex_SRGB("Base_Tex_SRGB", 2D) = "white" {}
	_NormalMap_Tex_NRM("NormalMap_Tex_NRM", 2D) = "white" {}
	_SpecularMap_Tex_LIN("SpecularMap_Tex_LIN", 2D) = "white" {}
	_RoughnessMap_Tex_LIN("RoughnessMap_Tex_LIN", 2D) = "white" {}
	_CylindricalLayer_Tex_LIN("CylindricalLayer_Tex_LIN", 2D) = "white" {}
	_LightAdd_Tex_LIN("LightAdd_Tex_LIN", 2D) = "white" {}
	_SubNormalMap_Tex_NRM("SubNormalMap_Tex_NRM", 2D) = "white" {}
	_Dirty_Tex_LIN("Dirty_Tex_LIN", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}

SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	LOD 400

CGPROGRAM
#pragma surface surf BlinnPhong alphatest:_Cutoff
#pragma target 3.0

sampler2D _Base_Tex_SRGB;
sampler2D _NormalMap_Tex_NRM;
sampler2D _SpecularMap_Tex_LIN;

struct Input {
	float2 uv_Base_Tex_SRGB;
	float2 uv_NormalMap_Tex_NRM;
	float2 uv_SpecularMap_Tex_LIN;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_Base_Tex_SRGB, IN.uv_Base_Tex_SRGB);
	o.Albedo = tex.rgb;
	o.Gloss = tex.a;
	o.Alpha = tex.a;
	fixed4 normalTex = tex2D(_NormalMap_Tex_NRM, IN.uv_NormalMap_Tex_NRM);
	normalTex.r = normalTex.a;
	normalTex.g = 1.0f - normalTex.g;
	normalTex.b = 1.0f;
	normalTex.a = 1.0f;
	o.Normal = UnpackNormal(normalTex);
	fixed4 specularTex = tex2D(_SpecularMap_Tex_LIN, IN.uv_SpecularMap_Tex_LIN);
	o.Specular = specularTex.r;
}
ENDCG
}

FallBack "Standard"
}