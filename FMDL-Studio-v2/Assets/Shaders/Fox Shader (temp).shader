// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "CustomShaders/FoxShaders" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _BumpMap ("Normalmap", 2D) = "bump" {}
}

SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    LOD 300

CGPROGRAM
#pragma surface surf Lambert alpha:fade

sampler2D _MainTex;
sampler2D _BumpMap;
fixed4 _Color;

struct Input {
    float2 uv_MainTex;
    float2 uv_BumpMap;
};

void surf (Input IN, inout SurfaceOutput o) {
    fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
    o.Albedo = c.rgb;
    o.Alpha = c.a;
	float4 Normal = tex2D(_BumpMap, IN.uv_BumpMap);
	Normal .r = Normal.a;
	Normal.g = Normal.g * -1;
    Normal.b = 1.0f;
    o.Normal = UnpackNormal(Normal);
}
ENDCG
}

FallBack "Legacy Shaders/Transparent/Diffuse"
}