Shader "FoxShaders/DBG_TangentView" {
Properties {
	_Lamp0Color("Lamp0Color", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Lamp0Intensity("Lamp0Intensity", Vector) = (0.0, 0.0, 0.0, 0.0)
	_AmbientLight("AmbientLight", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Base_Texrue("Base_Texrue", 2D) = "white" {}
	_Normal_Texture("Normal_Texture", 2D) = "white" {}
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