// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/VRUI" {
	Properties {
		_Color("Main Color", Color) = (.5,.5,.5,1)
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_MainTex("Base (RGB)", 2D) = "white" { }
		_EmitMap("Emission Texture", 2D) = "black" {}
		_EmitColor("Emission Color", Color) = (.5,.5,.5,1)
		_EmitStrength("Emission Strength", Range(0.0, 5.0)) = 0
	}
	SubShader {
		Tags { "RenderType"="Transparent" }
		LOD 200
		//Cull Front
		ZWrite Off
		ZTest LEqual
		
		// you can choose what kind of blending mode you want for the outline
			Blend SrcAlpha OneMinusSrcAlpha // Normal
											//Blend One One // Additive
											//Blend One OneMinusDstColor // Soft Additive
											//Blend DstColor Zero // Multiplicative
											//Blend DstColor SrcColor // 2x Multiplicative

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _EmitMap;

		struct Input {
			float2 uv_MainTex;
			float2 uv_EmitMap;
		};

		fixed4 _Color;
		float _EmitStrength;
		uniform float3 _EmitColor;
		half _Glossiness;
		half _Metallic;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			// Albedo comes from a texture tinted by color
			o.Albedo = c.rgb;
			o.Emission = (tex2D(_EmitMap, IN.uv_EmitMap) * _EmitStrength).rgb * _EmitColor  * c.a;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
