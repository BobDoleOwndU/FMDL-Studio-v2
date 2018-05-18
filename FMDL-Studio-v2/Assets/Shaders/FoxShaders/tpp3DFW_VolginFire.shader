Shader "FoxShaders/tpp3DFW_VolginFire" {
Properties {
	DistortionVelocity("DistortionVelocity", Vector) = (0.0, 0.0, 0.0, 0.0)
	DistortionAmount("DistortionAmount", Vector) = (0.0, 0.0, 0.0, 0.0)
	DistortionAmountFireTexture("DistortionAmountFireTexture", Vector) = (0.0, 0.0, 0.0, 0.0)
	DistortionAmountDepthSampling("DistortionAmountDepthSampling", Vector) = (0.0, 0.0, 0.0, 0.0)
	FireLayerIntensity("FireLayerIntensity", Vector) = (0.0, 0.0, 0.0, 0.0)
	BurnLayerIntensity("BurnLayerIntensity", Vector) = (0.0, 0.0, 0.0, 0.0)
	ViewAttenuationIntensity("ViewAttenuationIntensity", Vector) = (0.0, 0.0, 0.0, 0.0)
	Time("Time", Vector) = (0.0, 0.0, 0.0, 0.0)
	Burn_Tex_LIN("Burn_Tex_LIN", 2D) = "white" {}
	Fire_Tex_LIN("Fire_Tex_LIN", 2D) = "white" {}
	Distortion_Tex_LIN("Distortion_Tex_LIN", 2D) = "white" {}
	Mask_Tex_LIN("Mask_Tex_LIN", 2D) = "white" {}
	Depth_Tex_LIN("Depth_Tex_LIN", 2D) = "white" {}
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