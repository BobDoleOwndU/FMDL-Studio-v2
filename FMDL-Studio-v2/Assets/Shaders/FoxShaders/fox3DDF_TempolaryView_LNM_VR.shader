Shader "FoxShaders/fox3DDF_TempolaryView_LNM_VR" {
Properties {
	TempolaryBaseColor("TempolaryBaseColor", Vector) = (0.0, 0.0, 0.0, 0.0)
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