Shader "FoxShaders/tpp_2d_DirDisp" {
Properties {
	refrectionR("refrectionR", Vector) = (0.0, 0.0, 0.0, 0.0)
	refrectionG("refrectionG", Vector) = (0.0, 0.0, 0.0, 0.0)
	refrectionB("refrectionB", Vector) = (0.0, 0.0, 0.0, 0.0)
	Base_Texture("Base_Texture", 2D) = "white" {}
	NormalMap_Tex_NRM("NormalMap_Tex_NRM", 2D) = "white" {}
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