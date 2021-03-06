﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ImageEffect/Camouflage"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_ScanStartDistance("Scan Start Distance", float) = 0
		_ScanDistance("Scan Distance", float) = 0
		_ScanWidth("Scan Width", float) = 10
		_InnerEdgeFadeWidth("Inner Edge Fading Width", float) = 4
		_LeadSharp("Leading Edge Sharpness", float) = 10
		_LeadColor("Leading Edge Color", Color) = (1, 1, 1, 0)
		_AdditionalRedness("Additional Redness", Range(0, 1)) = 0
		_MidColor("Mid Color", Color) = (1, 1, 1, 0)
		_TrailColor("Trail Color", Color) = (1, 1, 1, 0)
		_HBarColor("Horizontal Bar Color", Color) = (0.5, 0.5, 0.5, 0)
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct VertIn
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 ray : TEXCOORD1;
			};

			struct VertOut
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 uv_depth : TEXCOORD1;
				float4 interpolatedRay : TEXCOORD2;
			};

			float4 _MainTex_TexelSize;
			float4 _CameraWS;

			VertOut vert(VertIn v)
			{
				VertOut o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv.xy;
				o.uv_depth = v.uv.xy;

				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					o.uv.y = 1 - o.uv.y;
				#endif

				o.interpolatedRay = v.ray;

				return o;
			}

			sampler2D _MainTex;
			sampler2D_float _CameraDepthTexture;
			float4 _WorldSpaceOriginPos;
			float _ScanStartDistance;
			float _ScanDistance;
			float _ScanWidth;
			float _InnerEdgeFadeWidth;
			float _LeadSharp;
			float _AdditionalRedness;
			float4 _LeadColor;
			float4 _MidColor;
			float4 _TrailColor;
			float4 _HBarColor;

			half4 frag (VertOut i) : SV_Target
			{
				half4 col = tex2D(_MainTex, i.uv);

				float rawDepth = DecodeFloatRG(tex2D(_CameraDepthTexture, i.uv_depth));
				float linearDepth = Linear01Depth(rawDepth);
				float4 wsDir = linearDepth * i.interpolatedRay;
				float3 wsPos = _WorldSpaceCameraPos + wsDir;

				float dist = distance(wsPos, _WorldSpaceOriginPos);

				/*if (dist < _ScanDistance && dist > _ScanStartDistance && linearDepth < 1)
				{
					float diff = 1 - (_ScanDistance - dist) / (_ScanWidth);
					
					half4 edge = lerp(_MidColor, _LeadColor, pow(diff, _LeadSharp));
					scannerCol = lerp(_TrailColor, edge, diff) + _MidColor;
					scannerCol *= diff;
					
					float alpha = lerp(0, 1, (dist - _ScanStartDistance) / _InnerEdgeFadeWidth);			
					if (alpha > 1) {
					    alpha = 1;
					}	
					scannerCol *= alpha;
				}*/

				if (dist < _ScanDistance && dist > _ScanStartDistance && linearDepth < 1) 
				{
					col.r += col.g + col.b + _AdditionalRedness;
					col.g = 0;
					col.b = 0;
				}

				return col;
			}
			ENDCG
		}
	}
}
