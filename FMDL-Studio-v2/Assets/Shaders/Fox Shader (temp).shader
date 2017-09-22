Shader "Custom/Fox Shader (temp)" {
	Properties {
		_MainTex ("Albedo (RGBA)", 2D) = "white" {}
        _SRM ("SRM", 2D) = "white" {}
        _Normal ("Normal", 2D)  = "bump" {}
        } 
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
        sampler2D _SRM;
        sampler2D _Normal;

		struct Input {
			float2 uv_MainTex;
		};
              
		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			float4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = 0.0f;
			o.Smoothness = tex2D (_SRM, IN.uv_MainTex).g * -1.0f;
			o.Alpha = c.a;
            
            float4 mgsvNormal = tex2D(_Normal, IN.uv_MainTex);

            float4 normalNormal = float4(mgsvNormal.a, mgsvNormal.g, 1.0f, 1.0f);
            o.Normal = UnpackNormal(normalNormal);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
