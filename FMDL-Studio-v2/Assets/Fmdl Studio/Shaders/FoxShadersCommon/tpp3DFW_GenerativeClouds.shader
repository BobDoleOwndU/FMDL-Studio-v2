Shader "FoxShaders/tpp3DFW_GenerativeClouds" {
Properties {
	_p0("p0", Vector) = (0.0, 0.0, 0.0, 0.0)
	_p1("p1", Vector) = (0.0, 0.0, 0.0, 0.0)
	_p2("p2", Vector) = (0.0, 0.0, 0.0, 0.0)
	_p3("p3", Vector) = (0.0, 0.0, 0.0, 0.0)
	_p4("p4", Vector) = (0.0, 0.0, 0.0, 0.0)
	_p5("p5", Vector) = (0.0, 0.0, 0.0, 0.0)
	_p6("p6", Vector) = (0.0, 0.0, 0.0, 0.0)
	_p7("p7", Vector) = (0.0, 0.0, 0.0, 0.0)
	_t_0("t_0", 2D) = "white" {}
	_t_1("t_1", 2D) = "white" {}
	_t_2("t_2", 2D) = "white" {}
	_t_3("t_3", 2D) = "white" {}
	_t_4("t_4", 2D) = "white" {}
	_t_5("t_5", 2D) = "white" {}
	_t_6("t_6", 2D) = "white" {}
	_t_7("t_7", 2D) = "white" {}
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