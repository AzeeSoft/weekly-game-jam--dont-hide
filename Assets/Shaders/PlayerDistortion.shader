// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/PlayerDistortion"
{
	Properties
	{
		_DistortionAmount("Distortion Amount", Range( 0 , 0.1)) = 0.292
		_DepthFadeDistance("Depth Fade Distance", Float) = 0
		_TextureSample1("Texture Sample 1", 2D) = "bump" {}
		_TimeScale("Time Scale", Float) = 0
		_ForcefieldTint("Forcefield Tint", Color) = (0,0,0,0)
		_IntersectionColor("Intersection Color", Color) = (0.4338235,0.4377282,1,0)
		_FresnelPower("Fresnel Power", Float) = 0
		_FresnelScale("Fresnel Scale", Float) = 0
	}
	
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		
		Pass
		{
			
			Name "First Pass"
			CGINCLUDE
			#pragma target 3.0
			ENDCG
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
			ColorMask RGBA
			ZWrite Off
			ZTest LEqual
			Offset 0 , 0
			
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			

			struct appdata
			{
				float4 vertex : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord : TEXCOORD0;
			};

			uniform float4 _IntersectionColor;
			UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
			uniform float4 _CameraDepthTexture_TexelSize;
			uniform float _DepthFadeDistance;
			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord = screenPos;
				
				
				v.vertex.xyz +=  float3(0,0,0) ;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				fixed4 finalColor;
				float4 IntersectionColor208 = _IntersectionColor;
				float4 screenPos = i.ase_texcoord;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth216 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
				float distanceDepth216 = abs( ( screenDepth216 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _DepthFadeDistance ) );
				float SaturatedDepthFade224 = saturate( distanceDepth216 );
				float4 appendResult231 = (float4((IntersectionColor208).rgb , ( 1.0 - SaturatedDepthFade224 )));
				
				
				finalColor = appendResult231;
				return finalColor;
			}
			ENDCG
		}

		GrabPass{ }

		Pass
		{
			Name "Second Pass"
			
			CGINCLUDE
			#pragma target 3.0
			ENDCG
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Back
			ColorMask RGBA
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			
			CGPROGRAM
			#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
			#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
			#else
			#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
			#endif

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "UnityStandardUtils.cginc"
			#include "UnityShaderVariables.cginc"


			struct appdata
			{
				float4 vertex : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 ase_texcoord : TEXCOORD0;
				float3 ase_normal : NORMAL;
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
			};

			ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
			uniform float _DistortionAmount;
			uniform sampler2D _TextureSample1;
			uniform float _TimeScale;
			uniform float4 _ForcefieldTint;
			uniform float4 _IntersectionColor;
			uniform float _FresnelScale;
			uniform float _FresnelPower;
			UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
			uniform float4 _CameraDepthTexture_TexelSize;
			uniform float _DepthFadeDistance;
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
			
			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord1 = screenPos;
				float3 ase_worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.ase_texcoord2.xyz = ase_worldPos;
				float3 ase_worldNormal = UnityObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord3.xyz = ase_worldNormal;
				
				o.ase_texcoord.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.zw = 0;
				o.ase_texcoord2.w = 0;
				o.ase_texcoord3.w = 0;
				
				v.vertex.xyz +=  float3(0,0,0) ;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				fixed4 finalColor;
				float2 uv0199 = i.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float mulTime197 = _Time.y * _TimeScale;
				float cos201 = cos( mulTime197 );
				float sin201 = sin( mulTime197 );
				float2 rotator201 = mul( uv0199 - float2( 0.5,0.5 ) , float2x2( cos201 , -sin201 , sin201 , cos201 )) + float2( 0.5,0.5 );
				float4 screenPos = i.ase_texcoord1;
				float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( screenPos );
				float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
				float4 screenColor213 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( float4( UnpackScaleNormal( tex2D( _TextureSample1, rotator201 ), _DistortionAmount ) , 0.0 ) + ase_grabScreenPosNorm ).xy);
				float4 IntersectionColor208 = _IntersectionColor;
				float3 ase_worldPos = i.ase_texcoord2.xyz;
				float3 ase_worldViewDir = UnityWorldSpaceViewDir(ase_worldPos);
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 ase_worldNormal = i.ase_texcoord3.xyz;
				float fresnelNdotV214 = dot( ase_worldNormal, ase_worldViewDir );
				float fresnelNode214 = ( 0.0 + _FresnelScale * pow( 1.0 - fresnelNdotV214, _FresnelPower ) );
				float4 lerpResult225 = lerp( ( float4( (screenColor213).rgb , 0.0 ) * _ForcefieldTint ) , ( IntersectionColor208 * fresnelNode214 ) , fresnelNode214);
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth216 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
				float distanceDepth216 = abs( ( screenDepth216 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _DepthFadeDistance ) );
				float SaturatedDepthFade224 = saturate( distanceDepth216 );
				float4 appendResult228 = (float4((lerpResult225).rgb , SaturatedDepthFade224));
				
				
				finalColor = appendResult228;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=17000
197;264;1152;718;-764.0685;1127.045;1.344538;True;False
Node;AmplifyShaderEditor.CommentaryNode;195;938.2596,-937.1357;Float;False;1820.48;460.2398;;13;221;218;215;213;207;206;203;202;201;199;198;197;196;Calculate and apply Distortion;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;196;988.2596,-631.1358;Float;False;Property;_TimeScale;Time Scale;3;0;Create;True;0;0;False;0;0;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;197;1148.26,-631.1358;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;198;1148.26,-759.1357;Float;False;Constant;_Vector0;Vector 0;-1;0;Create;True;0;0;False;0;0.5,0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;199;1100.26,-887.1356;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RotatorNode;201;1372.26,-807.1356;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;200;1463.236,-1529.107;Float;False;1268.871;416.1864;;9;231;230;229;224;220;216;212;208;204;First Pass only renders intersection;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;202;1372.26,-679.1358;Float;False;Property;_DistortionAmount;Distortion Amount;0;0;Create;True;0;0;False;0;0.292;0.0167;0;0.1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;205;2071.596,-382.168;Float;False;752.7268;415.7021;;5;217;214;211;210;209;Adding Rim effect;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;203;1660.26,-871.1356;Float;True;Property;_TextureSample1;Texture Sample 1;2;0;Create;True;0;0;False;0;None;d01457b88b1c5174ea4235d140b5fab8;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;204;1817.236,-1476.355;Float;False;Property;_IntersectionColor;Intersection Color;5;0;Create;True;0;0;False;0;0.4338235,0.4377282,1,0;0.4858491,1,0.955263,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GrabScreenPosition;206;1724.26,-663.1358;Float;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;209;2121.596,-89.88044;Float;False;Property;_FresnelPower;Fresnel Power;6;0;Create;True;0;0;False;0;0;2.32;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;208;2057.236,-1476.355;Float;False;IntersectionColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;207;1980.26,-775.1357;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;210;2137.596,-185.8804;Float;False;Property;_FresnelScale;Fresnel Scale;7;0;Create;True;0;0;False;0;0;1.99;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;212;1513.236,-1220.355;Float;False;Property;_DepthFadeDistance;Depth Fade Distance;1;0;Create;True;0;0;False;0;0;0.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenColorNode;213;2126.177,-849.7404;Float;False;Global;_GrabScreen0;Grab Screen 0;2;0;Create;True;0;0;False;0;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;211;2305.056,-324.148;Float;False;208;IntersectionColor;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FresnelNode;214;2333.115,-219.4659;Float;True;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;216;1753.236,-1236.355;Float;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;217;2655.323,-332.168;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;218;2325.236,-752.3558;Float;False;Property;_ForcefieldTint;Forcefield Tint;4;0;Create;True;0;0;False;0;0,0,0,0;0.6040851,0.6363767,0.7075472,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;215;2341.223,-839.2983;Float;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;223;2942.035,-575.0389;Float;False;668.4583;299.6378;;4;228;227;226;225;Combining everything through generated alphas;1,1,1,1;0;0
Node;AmplifyShaderEditor.WireNode;222;2849.297,-367.9385;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;220;1945.236,-1236.355;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;219;2832.448,-240.7967;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;221;2581.237,-768.3557;Float;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;224;2121.236,-1268.355;Float;False;SaturatedDepthFade;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;225;2992.035,-482.2546;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;226;3200.245,-525.0389;Float;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;227;3169.626,-390.4009;Float;False;224;SaturatedDepthFade;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;230;2377.477,-1292.328;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;231;2559.853,-1412.239;Float;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;228;3437.493,-466.6308;Float;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ComponentMaskNode;229;2291.796,-1479.107;Float;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;138;3778.501,-515.9439;Float;False;False;2;Float;ASEMaterialInspector;0;9;ASETemplateShaders/DoublePassUnlit;003dfa9c16768d048b74f75c088119d8;True;Second;0;1;Second Pass;2;False;False;False;False;False;False;False;False;False;True;1;RenderType=Opaque=RenderType;False;0;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;True;False;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;0;True;2;0;;0;0;Standard;0;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;135;2913.303,-1447.391;Float;False;True;2;Float;ASEMaterialInspector;0;9;Custom/PlayerDistortion;003dfa9c16768d048b74f75c088119d8;True;First;0;0;First Pass;2;False;False;False;False;False;False;False;False;False;True;2;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;False;0;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;True;False;True;2;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;2;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;0;True;2;0;;0;0;Standard;0;0;2;True;True;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;197;0;196;0
WireConnection;201;0;199;0
WireConnection;201;1;198;0
WireConnection;201;2;197;0
WireConnection;203;1;201;0
WireConnection;203;5;202;0
WireConnection;208;0;204;0
WireConnection;207;0;203;0
WireConnection;207;1;206;0
WireConnection;213;0;207;0
WireConnection;214;2;210;0
WireConnection;214;3;209;0
WireConnection;216;0;212;0
WireConnection;217;0;211;0
WireConnection;217;1;214;0
WireConnection;215;0;213;0
WireConnection;222;0;217;0
WireConnection;220;0;216;0
WireConnection;219;0;214;0
WireConnection;221;0;215;0
WireConnection;221;1;218;0
WireConnection;224;0;220;0
WireConnection;225;0;221;0
WireConnection;225;1;222;0
WireConnection;225;2;219;0
WireConnection;226;0;225;0
WireConnection;230;0;224;0
WireConnection;231;0;229;0
WireConnection;231;3;230;0
WireConnection;228;0;226;0
WireConnection;228;3;227;0
WireConnection;229;0;208;0
WireConnection;138;0;228;0
WireConnection;135;0;231;0
ASEEND*/
//CHKSM=C4FC7D65DB0A0C8D74D1B27D45D52B4A70F2BDF0