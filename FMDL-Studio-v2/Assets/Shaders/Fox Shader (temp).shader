// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "CustomShaders/FoxShaders" {
Properties {
	_MainTex("Albedo", 2D) = "white" {}
	_LayerTex("Secondary Albedo", 2D) = "white" {}
	_LayerMask("Secondary Albedo Mask", 2D) = "black" {}
	_Cutoff("Alpha Cutoff", Range(0,1)) = 0.5
	_BumpMap("Normal Map", 2D) = "bump" {}
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 0)
    _SRM ("Roughness", 2D) = "white" {}
}

SubShader {
    Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
    LOD 400

CGPROGRAM
#pragma surface surf BlinnPhong alphatest:_Cutoff
#pragma target 3.0

sampler2D _MainTex;
sampler2D _BumpMap;
sampler2D _SRM;
sampler2D _LayerTex;
sampler2D _LayerMask;

struct Input {
    float2 uv_MainTex;
    float2 uv_BumpMap;
};

void surf (Input IN, inout SurfaceOutput o) {
    fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	fixed4 layerTex = tex2D(_LayerTex, IN.uv_MainTex);
	fixed layerMask = tex2D(_LayerMask, IN.uv_MainTex).r;
	tex = lerp(tex, layerTex, layerMask);
	o.Albedo = tex.rgb;
    fixed3 _Roughness = tex2D(_SRM, IN.uv_MainTex);
    o.Gloss = 1.0f - _Roughness.g;
	o.Alpha = tex.a;
    float4 NormalMap = tex2D(_BumpMap, IN.uv_BumpMap);
    NormalMap.r = NormalMap.a;
    NormalMap.g = 1.0f - NormalMap.g;
    NormalMap.b = 1.0f;
    o.Normal = UnpackNormal(NormalMap);
}

ENDCG
}

FallBack "Legacy Shaders/Transparent/Cutout/Diffuse"
}