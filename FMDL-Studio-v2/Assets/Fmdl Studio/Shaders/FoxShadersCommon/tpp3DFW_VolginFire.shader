Shader "FoxShaders/tpp3DFW_VolginFire" {
Properties {
	_DistortionVelocity("DistortionVelocity", Vector) = (0.0, 0.0, 0.0, 0.0)
	_DistortionAmount("DistortionAmount", Vector) = (0.0, 0.0, 0.0, 0.0)
	_DistortionAmountFireTexture("DistortionAmountFireTexture", Vector) = (0.0, 0.0, 0.0, 0.0)
	_DistortionAmountDepthSampling("DistortionAmountDepthSampling", Vector) = (0.0, 0.0, 0.0, 0.0)
	_FireLayerIntensity("FireLayerIntensity", Vector) = (0.0, 0.0, 0.0, 0.0)
	_BurnLayerIntensity("BurnLayerIntensity", Vector) = (0.0, 0.0, 0.0, 0.0)
	_ViewAttenuationIntensity("ViewAttenuationIntensity", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Time("Time", Vector) = (0.0, 0.0, 0.0, 0.0)
	_Burn_Tex_LIN("Burn_Tex_LIN", 2D) = "white" {}
	_Fire_Tex_LIN("Fire_Tex_LIN", 2D) = "white" {}
	_Distortion_Tex_LIN("Distortion_Tex_LIN", 2D) = "white" {}
	_Mask_Tex_LIN("Mask_Tex_LIN", 2D) = "white" {}
	_Depth_Tex_LIN("Depth_Tex_LIN", 2D) = "white" {}
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