// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/DemonAttackLine"
{
	Properties
	{
		_Color("Color", Color) = (0,0,0,0)
		_Multiplier("Multiplier", Range( 0 , 50)) = 10
		_Speed("Speed", Range( -3 , 3)) = 0.5
		_MainTex("MainTex", 2D) = "white" {}
		_VScale("V Scale", Float) = 3
		_AlphaClip("Alpha Clip", Range( 0 , 1)) = 0.1
	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Transparent" }
		LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha OneMinusSrcAlpha , SrcAlpha OneMinusSrcAlpha
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "Unlit"
			Tags { "LightMode"="ForwardBase" }
			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 ase_texcoord : TEXCOORD0;
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord : TEXCOORD0;
			};

			uniform float _Multiplier;
			uniform sampler2D _MainTex;
			uniform float _Speed;
			uniform float _VScale;
			uniform float _AlphaClip;
			uniform float4 _Color;
			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.ase_texcoord.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.zw = 0;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				float mulTime48 = _Time.y * _Speed;
				float temp_output_50_0 = abs( ( i.ase_texcoord.xy.x + mulTime48 ) );
				float4 appendResult53 = (float4(temp_output_50_0 , ( i.ase_texcoord.xy.y / _VScale ) , 0.0 , 0.0));
				float4 tex2DNode52 = tex2D( _MainTex, appendResult53.xy );
				clip( tex2DNode52.r - _AlphaClip);
				
				
				finalColor = ( tex2DNode52 * _Color );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=17000
275;201;1152;665;1893.736;-105.4276;1.994027;True;False
Node;AmplifyShaderEditor.RangedFloatNode;49;-1189.829,285.8897;Float;False;Property;_Speed;Speed;2;0;Create;True;0;0;True;0;0.5;1.5;-3;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;17;-1145.295,63.63601;Float;True;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;48;-699.6657,334.4666;Float;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;47;-672.2198,90.08504;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;55;-1361.682,567.795;Float;False;Property;_VScale;V Scale;4;0;Create;True;0;0;True;0;3;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;50;-428.0029,18.8761;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;54;-1161.588,550.5455;Float;False;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;53;-988.0394,567.1551;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;52;-730.1632,725.8271;Float;True;Property;_MainTex;MainTex;3;0;Create;True;0;0;True;0;e28dc97a9541e3642a48c0e3886688c5;8276ff1f6d104ac4289bf3f0482df4f0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;59;-432.115,609.9164;Float;False;Property;_AlphaClip;Alpha Clip;5;0;Create;True;0;0;True;0;0.1;0.1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1;-641.851,964.0938;Float;False;Property;_Color;Color;0;0;Create;True;0;0;True;0;0,0,0,0;0.3018867,0.1034769,0.05838373,0.8078431;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClipNode;58;-298.2571,716.3203;Float;False;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0.1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleRemainderNode;39;65.09646,88.89487;Float;True;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;-310.2264,882.6202;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;42;-483.3275,257.9166;Float;False;Property;_Multiplier;Multiplier;1;0;Create;True;0;0;True;0;10;4.9;0;50;0;1;FLOAT;0
Node;AmplifyShaderEditor.LengthOpNode;51;-870.5432,120.3867;Float;True;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-161.5414,89.75977;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;16.68;False;1;FLOAT;0
Node;AmplifyShaderEditor.FloorOpNode;40;283.5804,88.76421;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;196.4961,872.6036;Float;False;True;2;Float;ASEMaterialInspector;0;1;Custom/DemonAttackLine;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;2;5;False;-1;10;False;-1;2;5;False;-1;10;False;-1;True;0;False;-1;0;False;-1;True;False;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Transparent=RenderType;True;2;0;False;False;False;False;False;False;False;False;False;True;1;LightMode=ForwardBase;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;48;0;49;0
WireConnection;47;0;17;1
WireConnection;47;1;48;0
WireConnection;50;0;47;0
WireConnection;54;0;17;2
WireConnection;54;1;55;0
WireConnection;53;0;50;0
WireConnection;53;1;54;0
WireConnection;52;1;53;0
WireConnection;58;0;52;0
WireConnection;58;1;52;1
WireConnection;58;2;59;0
WireConnection;39;0;37;0
WireConnection;57;0;58;0
WireConnection;57;1;1;0
WireConnection;51;0;17;0
WireConnection;37;0;50;0
WireConnection;37;1;42;0
WireConnection;40;0;39;0
WireConnection;0;0;57;0
ASEEND*/
//CHKSM=2B2260996CDA6A2B63A8906A098A24ABEFC450A4