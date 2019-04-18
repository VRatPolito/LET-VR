Shader "Custom/Custom Texture Blend" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_EmitColor ("Emit Color", Color) = (.5,.5,.5,1)
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0

		_Element1Tex("Element 1 Tex", 2D) = "white" {}
	_Element2Tex("Element 2 Tex", 2D) = "white" {}
	_Element3Tex("Element 3 Tex", 2D) = "white" {}
	_Element4Tex("Element 4 Tex", 2D) = "white" {}
	_Element5Tex("Element 5 Tex", 2D) = "white" {}
	_NormalTex("Normal Tex", 2D) = "bump" {}
	_EmitTex("Emit Tex", 2D) = "black" {}

	_Element1Level("Element 1 Level", Range(0,1)) = 1
		_Element2Level("Element 2 Level", Range(0,1)) = 0
		_Element3Level("Element 3 Level", Range(0,1)) = 0
		_Element4Level("Element 4 Level", Range(0,1)) = 0
		_Element5Level("Element 5 Level", Range(0,1)) = 0
		_NormalTexLevel("Normal Tex Level", Range(-2,2)) = 1
		_EmitTexLevel("Emit Tex Level", Range(0,2)) = 0
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard fullforwardshadows
#pragma target 3.0
		sampler2D _Element1Tex;
	sampler2D _Element2Tex;
	sampler2D _Element3Tex;
	sampler2D _Element4Tex;
	sampler2D _Element5Tex;
	sampler2D _NormalTex;
	sampler2D _EmitTex;

	float _Element1Level;
	float _Element2Level;
	float _Element3Level;
	float _Element4Level;
	float _Element5Level;
	float _NormalTexLevel;
	float _EmitTexLevel;
	
	uniform float3 _EmitColor;

	struct Input {
		float2 uv_Element1Tex;
		float2 uv_NormalTex;
		float2 uv_EmitTex;
		// assume all textures will use the same UV coordinate, so only pass one
	};

	half _Glossiness;
	half _Metallic;

	void surf(Input IN, inout SurfaceOutputStandard o) {
		//          float total = _Element1Level + _Element2Level + _Element3Level + _Element4Level + _Element5Level;
		//          if(total == 0)
		//          {
		//          ///Avoid dividing by zero!
		//              total = 1;
		//          }

		fixed4 c =
			tex2D(_Element1Tex, IN.uv_Element1Tex) * (_Element1Level)+
			tex2D(_Element2Tex, IN.uv_Element1Tex) * (_Element2Level)+
			tex2D(_Element3Tex, IN.uv_Element1Tex) * (_Element3Level)+
			tex2D(_Element4Tex, IN.uv_Element1Tex) * (_Element4Level)+
			tex2D(_Element5Tex, IN.uv_Element1Tex) * (_Element5Level);
		// c /= total;
		o.Albedo = c.rgb;
		// Metallic and smoothness come from slider variables
		o.Metallic = _Metallic;
		o.Smoothness = _Glossiness;
		o.Alpha = c.a;
		fixed3 n = UnpackNormal(tex2D(_NormalTex, IN.uv_NormalTex)).rgb;
		n.x *= _NormalTexLevel;
		n.y *= _NormalTexLevel;
		o.Normal = normalize(n);
		o.Emission = (tex2D(_EmitTex, IN.uv_EmitTex) * (_EmitTexLevel)) * _EmitColor;
	}
	ENDCG
	}
		FallBack "Diffuse"
}