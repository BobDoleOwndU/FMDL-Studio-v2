Shader "FoxShaders/tpp3DFW_DepthAnim" {
Properties {
	Incidence_Roughness("Incidence_Roughness", Vector) = (0.0, 0.0, 0.0, 0.0)
	FadeStart("FadeStart", Vector) = (0.0, 0.0, 0.0, 0.0)
	FadeEnd("FadeEnd", Vector) = (0.0, 0.0, 0.0, 0.0)
	FadeExp("FadeExp", Vector) = (0.0, 0.0, 0.0, 0.0)
	Base_Tex_LIN("Base_Tex_LIN", 2D) = "white" {}
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