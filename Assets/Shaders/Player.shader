// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/Player"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,0)
		_MainTex("Main Tex", 2D) = "white" {}
		_Metallic("Metallic", 2D) = "white" {}
		_MetallicMultiply("MetallicMultiply", Range( 0 , 1)) = 0
		_Glossiness("Glossiness", 2D) = "white" {}
		_GlossinessMultiply("GlossinessMultiply", Range( 0 , 1)) = 0.8
		[Normal]_NormalMap("Normal Map", 2D) = "bump" {}
		_Occlusion("Occlusion", 2D) = "white" {}
		[HDR]_EmissionColor("EmissionColor", Color) = (0,0,0,0)
		[HDR]_EmissionMask("EmissionMask", 2D) = "white" {}
		_EmissiveValue("EmissiveValue", Range( 0 , 1)) = 0
		_CamouflageValue("Camouflage Value", Range( 0 , 1)) = 0
		_CamouflageTransitionTexture("Camouflage Transition Texture", 2D) = "bump" {}
		_DistortionAmount("Distortion Amount", Range( 0 , 0.1)) = 0.292
		_DepthFadeDistance("Depth Fade Distance", Float) = 0
		_DistortionTexture("Distortion Texture", 2D) = "white" {}
		_TimeScale("Time Scale", Float) = 0
		_ForcefieldTint("Forcefield Tint", Color) = (0,0,0,0)
		_RimColor("Rim Color", Color) = (0.2990388,0.594023,0.6037736,1)
		_FresnelPower("Fresnel Power", Float) = 0
		_FresnelScale("Fresnel Scale", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ }
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform float _CamouflageValue;
		uniform sampler2D _CamouflageTransitionTexture;
		uniform float4 _CamouflageTransitionTexture_ST;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float4 _Color;
		uniform sampler2D _EmissionMask;
		uniform float4 _EmissionMask_ST;
		uniform float4 _EmissionColor;
		uniform float _EmissiveValue;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float _DistortionAmount;
		uniform sampler2D _DistortionTexture;
		uniform float _TimeScale;
		uniform float4 _ForcefieldTint;
		uniform float4 _RimColor;
		uniform float _FresnelScale;
		uniform float _FresnelPower;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _DepthFadeDistance;
		uniform sampler2D _Metallic;
		uniform float4 _Metallic_ST;
		uniform float _MetallicMultiply;
		uniform sampler2D _Glossiness;
		uniform float4 _Glossiness_ST;
		uniform float _GlossinessMultiply;
		uniform sampler2D _Occlusion;
		uniform float4 _Occlusion_ST;


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			o.Normal = UnpackNormal( tex2D( _NormalMap, uv_NormalMap ) );
			float2 uv_CamouflageTransitionTexture = i.uv_texcoord * _CamouflageTransitionTexture_ST.xy + _CamouflageTransitionTexture_ST.zw;
			float3 tex2DNode98 = UnpackNormal( tex2D( _CamouflageTransitionTexture, uv_CamouflageTransitionTexture ) );
			half camouflageMode75 =  ( _CamouflageValue - 0.0 > abs( tex2DNode98.r ) ? 1.0 : _CamouflageValue - 0.0 <= abs( tex2DNode98.r ) && _CamouflageValue + 0.0 >= abs( tex2DNode98.r ) ? 0.0 : 0.0 ) ;
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 temp_output_25_0 = ( tex2D( _MainTex, uv_MainTex ) * _Color );
			float4 ifLocalVar78 = 0;
			if( camouflageMode75 != 1.0 )
				ifLocalVar78 = temp_output_25_0;
			o.Albedo = ifLocalVar78.rgb;
			float2 uv_EmissionMask = i.uv_texcoord * _EmissionMask_ST.xy + _EmissionMask_ST.zw;
			float4 temp_output_17_0 = ( ( tex2D( _EmissionMask, uv_EmissionMask ) * _EmissionColor ) * _EmissiveValue );
			float mulTime30 = _Time.y * _TimeScale;
			float cos33 = cos( mulTime30 );
			float sin33 = sin( mulTime30 );
			float2 rotator33 = mul( i.uv_texcoord - float2( 0.5,0.5 ) , float2x2( cos33 , -sin33 , sin33 , cos33 )) + float2( 0.5,0.5 );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float4 screenColor44 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( float4( UnpackScaleNormal( tex2D( _DistortionTexture, rotator33 ), _DistortionAmount ) , 0.0 ) + ase_grabScreenPosNorm ).xy);
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelNdotV46 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode46 = ( 0.0 + _FresnelScale * pow( 1.0 - fresnelNdotV46, _FresnelPower ) );
			float4 lerpResult57 = lerp( ( float4( (screenColor44).rgb , 0.0 ) * _ForcefieldTint ) , ( _RimColor * fresnelNode46 ) , fresnelNode46);
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth66 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth66 = abs( ( screenDepth66 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _DepthFadeDistance ) );
			float SaturatedDepthFade68 = saturate( distanceDepth66 );
			float4 appendResult62 = (float4((lerpResult57).rgb , SaturatedDepthFade68));
			float4 camouflageEffect69 = appendResult62;
			float4 ifLocalVar86 = 0;
			if( camouflageMode75 == 1.0 )
				ifLocalVar86 = camouflageEffect69;
			else
				ifLocalVar86 = temp_output_17_0;
			o.Emission = ifLocalVar86.rgb;
			float2 uv_Metallic = i.uv_texcoord * _Metallic_ST.xy + _Metallic_ST.zw;
			float4 temp_output_2_0 = ( tex2D( _Metallic, uv_Metallic ) * _MetallicMultiply );
			float4 color97 = IsGammaSpace() ? float4(0.8962264,0.866634,0.866634,0) : float4(0.7799658,0.7229937,0.7229937,0);
			float4 ifLocalVar90 = 0;
			if( camouflageMode75 == 1.0 )
				ifLocalVar90 = color97;
			else
				ifLocalVar90 = temp_output_2_0;
			o.Metallic = ifLocalVar90.r;
			float2 uv_Glossiness = i.uv_texcoord * _Glossiness_ST.xy + _Glossiness_ST.zw;
			float4 temp_output_7_0 = ( tex2D( _Glossiness, uv_Glossiness ) * _GlossinessMultiply );
			float4 color94 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
			float4 ifLocalVar96 = 0;
			if( camouflageMode75 == 1.0 )
				ifLocalVar96 = color94;
			else
				ifLocalVar96 = temp_output_7_0;
			o.Smoothness = ifLocalVar96.r;
			float2 uv_Occlusion = i.uv_texcoord * _Occlusion_ST.xy + _Occlusion_ST.zw;
			o.Occlusion = tex2D( _Occlusion, uv_Occlusion ).r;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 screenPos : TEXCOORD2;
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				surfIN.screenPos = IN.screenPos;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17000
108;397;1152;689;4217.853;-878.5013;1.749383;True;False
Node;AmplifyShaderEditor.CommentaryNode;28;-3669.887,1338.464;Float;False;1820.48;681.5086;;17;30;31;29;32;55;35;33;49;50;41;38;27;44;65;66;67;68;Calculate and apply Distortion;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-3619.887,1644.464;Float;False;Property;_TimeScale;Time Scale;16;0;Create;True;0;0;False;0;0;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;30;-3459.887,1644.464;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;32;-3507.887,1388.464;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;31;-3459.887,1516.464;Float;False;Constant;_Vector0;Vector 0;-1;0;Create;True;0;0;False;0;0.5,0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RotatorNode;33;-3235.887,1468.464;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-3235.887,1596.464;Float;False;Property;_DistortionAmount;Distortion Amount;13;0;Create;True;0;0;False;0;0.292;0.01662117;0;0.1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;27;-2947.887,1404.464;Float;True;Property;_DistortionTexture;Distortion Texture;15;0;Create;True;0;0;False;0;None;d01457b88b1c5174ea4235d140b5fab8;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GrabScreenPosition;38;-2883.887,1612.464;Float;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;36;-2638.35,2148.947;Float;False;752.7268;415.7021;;5;48;46;42;39;64;Adding Rim effect;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;39;-2588.35,2441.235;Float;False;Property;_FresnelPower;Fresnel Power;19;0;Create;True;0;0;False;0;0;2.01;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;42;-2572.35,2345.234;Float;False;Property;_FresnelScale;Fresnel Scale;20;0;Create;True;0;0;False;0;0;1.99;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;41;-2627.887,1500.464;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FresnelNode;46;-2376.831,2311.648;Float;True;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenColorNode;44;-2481.97,1425.859;Float;False;Global;_GrabScreen0;Grab Screen 0;2;0;Create;True;0;0;False;0;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;64;-2568.313,2179.277;Float;False;Property;_RimColor;Rim Color;18;0;Create;True;0;0;True;0;0.2990388,0.594023,0.6037736,1;0.2990388,0.594023,0.6037736,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;65;-3596.895,1864.67;Float;False;Property;_DepthFadeDistance;Depth Fade Distance;14;0;Create;True;0;0;False;0;0;0.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;50;-2266.925,1436.301;Float;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;49;-2282.911,1523.244;Float;False;Property;_ForcefieldTint;Forcefield Tint;17;0;Create;True;0;0;False;0;0,0,0,0;0.3225347,0.3812954,0.4528302,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-2054.623,2198.947;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DepthFade;66;-3356.895,1848.67;Float;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;52;-1792.068,2115.28;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;51;-1666.112,1700.561;Float;False;668.4583;299.6378;;4;62;59;58;57;Combining everything through generated alphas;1,1,1,1;0;0
Node;AmplifyShaderEditor.SaturateNode;67;-3077.088,1852.488;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;54;-1742.48,2192.593;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;-2023.092,1505.335;Float;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;98;-3669.295,897.2449;Float;True;Property;_CamouflageTransitionTexture;Camouflage Transition Texture;12;0;Create;True;0;0;True;0;e24b2c680edaa90458d31f11544d79ca;5b653e484c8e303439ef414b62f969f0;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;57;-1616.112,1793.346;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;68;-2870.545,1851.03;Float;False;SaturatedDepthFade;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;101;-3018.346,768.8405;Float;False;Property;_CamouflageValue;Camouflage Value;11;0;Create;True;0;0;True;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;107;-3037.921,904.4436;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;58;-1407.902,1750.561;Float;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;59;-1438.521,1885.2;Float;False;68;SaturatedDepthFade;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;22;-910.798,178.6008;Float;True;Property;_EmissionMask;EmissionMask;9;1;[HDR];Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;11;-612.5485,266.6861;Float;False;Property;_EmissionColor;EmissionColor;8;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0,0,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;62;-1170.654,1808.97;Float;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TFHCIf;106;-2625.433,877.7598;Float;False;6;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;19;-934.5571,680.4174;Float;True;Property;_Metallic;Metallic;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;24;-830.9921,-449.6961;Float;False;Property;_Color;Color;0;0;Create;True;0;0;False;0;1,1,1,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;8;-603.257,895.4177;Float;False;Property;_GlossinessMultiply;GlossinessMultiply;5;0;Create;True;0;0;False;0;0.8;0.8;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-444.1677,177.8004;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;69;-889.4333,1806.896;Float;False;camouflageEffect;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;14;-916.3,-268.5001;Float;True;Property;_MainTex;Main Tex;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;18;-384.9488,311.1863;Float;False;Property;_EmissiveValue;EmissiveValue;10;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-597.757,678.4174;Float;False;Property;_MetallicMultiply;MetallicMultiply;3;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;20;-939.5571,883.4177;Float;True;Property;_Glossiness;Glossiness;4;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;75;-2360.444,870.6075;Half;False;camouflageMode;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;80;-135.6576,-516.4658;Float;False;75;camouflageMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2;-369.9399,670.2346;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;23;-934.455,1108.317;Float;True;Property;_Occlusion;Occlusion;7;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;94;-145.9401,834.3527;Float;False;Constant;_Color0;Color 0;20;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-325.257,867.4177;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;89;-158.4591,486.9277;Float;False;75;camouflageMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-519.2061,-264.8799;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;87;-219.1065,-74.00826;Float;False;69;camouflageEffect;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ColorNode;97;-374.719,439.786;Float;False;Constant;_Color1;Color 1;20;0;Create;True;0;0;False;0;0.8962264,0.866634,0.866634,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;85;-218.038,-142.5238;Float;False;75;camouflageMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-247.949,181.1865;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;95;-118.8825,731.3589;Float;False;75;camouflageMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;78;122.0494,-511.9845;Float;False;False;5;0;FLOAT;0;False;1;FLOAT;1;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ConditionalIfNode;86;116.1123,-139.7413;Float;False;False;5;0;FLOAT;0;False;1;FLOAT;1;False;2;COLOR;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ConditionalIfNode;96;135.8386,732.8542;Float;False;False;5;0;FLOAT;0;False;1;FLOAT;1;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;91;416.1364,835.3423;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;15;-913.3,-49.50001;Float;True;Property;_NormalMap;Normal Map;6;1;[Normal];Create;True;0;0;False;0;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ConditionalIfNode;90;96.26196,488.423;Float;False;False;5;0;FLOAT;0;False;1;FLOAT;1;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCGrayscale;103;-3346.52,895.9008;Float;True;1;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1011.535,-242.876;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Custom/Player;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;30;0;29;0
WireConnection;33;0;32;0
WireConnection;33;1;31;0
WireConnection;33;2;30;0
WireConnection;27;1;33;0
WireConnection;27;5;35;0
WireConnection;41;0;27;0
WireConnection;41;1;38;0
WireConnection;46;2;42;0
WireConnection;46;3;39;0
WireConnection;44;0;41;0
WireConnection;50;0;44;0
WireConnection;48;0;64;0
WireConnection;48;1;46;0
WireConnection;66;0;65;0
WireConnection;52;0;48;0
WireConnection;67;0;66;0
WireConnection;54;0;46;0
WireConnection;55;0;50;0
WireConnection;55;1;49;0
WireConnection;57;0;55;0
WireConnection;57;1;52;0
WireConnection;57;2;54;0
WireConnection;68;0;67;0
WireConnection;107;0;98;1
WireConnection;58;0;57;0
WireConnection;62;0;58;0
WireConnection;62;3;59;0
WireConnection;106;0;101;0
WireConnection;106;1;107;0
WireConnection;26;0;22;0
WireConnection;26;1;11;0
WireConnection;69;0;62;0
WireConnection;75;0;106;0
WireConnection;2;0;19;0
WireConnection;2;1;4;0
WireConnection;7;0;20;0
WireConnection;7;1;8;0
WireConnection;25;0;14;0
WireConnection;25;1;24;0
WireConnection;17;0;26;0
WireConnection;17;1;18;0
WireConnection;78;0;80;0
WireConnection;78;2;25;0
WireConnection;78;4;25;0
WireConnection;86;0;85;0
WireConnection;86;2;17;0
WireConnection;86;3;87;0
WireConnection;86;4;17;0
WireConnection;96;0;95;0
WireConnection;96;2;7;0
WireConnection;96;3;94;0
WireConnection;96;4;7;0
WireConnection;91;0;23;0
WireConnection;90;0;89;0
WireConnection;90;2;2;0
WireConnection;90;3;97;0
WireConnection;90;4;2;0
WireConnection;103;0;98;0
WireConnection;0;0;78;0
WireConnection;0;1;15;0
WireConnection;0;2;86;0
WireConnection;0;3;90;0
WireConnection;0;4;96;0
WireConnection;0;5;91;0
ASEEND*/
//CHKSM=8843270EAD8AE0026A321DC64CBF9B9E3B9875CC