// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/Diffuse_Addition" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_AdditonColor ("Additon Color", Color) = (0.3,0.3,0.3,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_LightIntensity ("LightIntensity", Range(0.1, 5.0)) = 1.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf BlinnPhong fullforwardshadows nolightmap

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		//fixed4 _Color;
		//fixed4 _AdditonColor;
		fixed _LightIntensity;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
			UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
#define _Color_arr Props
			UNITY_DEFINE_INSTANCED_PROP(fixed4, _AdditonColor)
#define _AdditonColor_arr Props
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutput o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Color);
			o.Albedo = (UNITY_ACCESS_INSTANCED_PROP(_AdditonColor_arr, _AdditonColor)*c.rgb + c.rgb)*_LightIntensity;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Mobile/Diffuse"
}
