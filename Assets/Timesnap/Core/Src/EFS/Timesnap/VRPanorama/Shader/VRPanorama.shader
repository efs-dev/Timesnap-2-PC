// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TimeSnap/VRPanorama"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_Alpha("Alpha", Range( 0 , 1)) = 1
		[HDR]_LeftTexture("LeftTexture", 2D) = "white" {}
		_LeftEyeOffset("LeftEyeOffset", Vector) = (0,0,0,0)
		_LeftEyeTiling("LeftEyeTiling", Vector) = (1,0.5,0,0)
		[HDR]_RightTexture("RightTexture", 2D) = "white" {}
		_RightEyeOffset("RightEyeOffset", Vector) = (0,0.5,0,0)
		_RightEyeTiling("RightEyeTiling", Vector) = (1,0.5,0,0)
		[HideInInspector]_TargetEye("TargetEye", Int) = 0
		_LeftToRightEye("LeftToRightEye", Range( 0 , 1)) = 0
	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" }
		LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Back
		ColorMask RGBA
		ZWrite Off
		ZTest LEqual
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "Unlit"
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			

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

			uniform float4 _Color;
			uniform sampler2D _LeftTexture;
			uniform float2 _LeftEyeTiling;
			uniform float2 _LeftEyeOffset;
			uniform sampler2D _RightTexture;
			uniform float2 _RightEyeTiling;
			uniform float2 _RightEyeOffset;
			uniform int _TargetEye;
			uniform float _LeftToRightEye;
			uniform float _Alpha;
			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.ase_texcoord.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.zw = 0;
				float3 vertexValue =  float3(0,0,0) ;
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
				fixed4 finalColor;
				float2 uv03 = i.ase_texcoord.xy * _LeftEyeTiling + _LeftEyeOffset;
				float2 uv04 = i.ase_texcoord.xy * _RightEyeTiling + _RightEyeOffset;
				float4 lerpResult18 = lerp( tex2D( _LeftTexture, uv03 ) , tex2D( _RightTexture, uv04 ) , min( ( ( unity_StereoEyeIndex + _TargetEye ) + _LeftToRightEye ) , 1.0 ));
				float4 break26 = lerpResult18;
				float4 appendResult28 = (float4(break26.r , break26.g , break26.b , ( break26.a * _Alpha )));
				
				
				finalColor = ( _Color * appendResult28 );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=16700
7;29;2546;1364;1842.734;1021.88;1.373575;True;False
Node;AmplifyShaderEditor.CommentaryNode;11;-1313.195,81.56255;Float;False;496;114;Always 0 in build, set to 1 when viewing right eye in editor;1;8;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;10;-1308.507,-87.65472;Float;False;343;120;0 = left eye, 1 = right eye, set by Unity;1;1;;1,1,1,1;0;0
Node;AmplifyShaderEditor.IntNode;1;-1287.507,-45.65464;Float;False;Global;unity_StereoEyeIndex;unity_StereoEyeIndex;0;0;Fetch;True;0;0;False;0;0;0;0;1;INT;0
Node;AmplifyShaderEditor.IntNode;8;-1282.195,118.5626;Float;False;Property;_TargetEye;TargetEye;8;1;[HideInInspector];Create;True;0;0;False;0;0;0;0;1;INT;0
Node;AmplifyShaderEditor.CommentaryNode;13;-870.3973,-854.3347;Float;False;252.0563;162.731;Left Eye UV;1;3;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;21;-614.5802,-82.10503;Float;False;2;2;0;INT;0;False;1;INT;0;False;1;INT;0
Node;AmplifyShaderEditor.CommentaryNode;12;-817.4576,-560.7999;Float;False;249;156;Right Eye UV;1;4;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-764.9813,59.70327;Float;False;Property;_LeftToRightEye;LeftToRightEye;9;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;38;-1178.497,-689.9641;Float;False;Property;_LeftEyeOffset;LeftEyeOffset;3;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;39;-1027.447,-544.2025;Float;False;Property;_RightEyeTiling;RightEyeTiling;7;0;Create;True;0;0;False;0;1,0.5;2,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;37;-1106.669,-905.4363;Float;False;Property;_LeftEyeTiling;LeftEyeTiling;4;0;Create;True;0;0;False;0;1,0.5;2,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;40;-1026.391,-366.756;Float;False;Property;_RightEyeOffset;RightEyeOffset;6;0;Create;True;0;0;False;0;0,0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;4;-798.4573,-523.7996;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,0.5;False;1;FLOAT2;0,0.5;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;24;-364.5301,-85.51887;Float;False;2;2;0;INT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;3;-855.2847,-815.2785;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,0.5;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;16;-473.7349,-566.3733;Float;True;Property;_RightTexture;RightTexture;5;1;[HDR];Create;True;0;0;False;0;None;fd51e4d64642d0041a1a4132a7ca7550;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;17;-473.3745,-903.7554;Float;True;Property;_LeftTexture;LeftTexture;2;1;[HDR];Create;True;0;0;False;0;None;13bee053c422674469d95f64a1863594;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMinOpNode;25;-234.1823,-88.38333;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;18;-3.506718,-652.1935;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;26;212.7234,-654.1799;Float;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;29;237.7508,-357.6742;Float;False;Property;_Alpha;Alpha;1;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;537.874,-466.5355;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;41;626.6103,-864.238;Float;False;Property;_Color;Color;0;0;Create;True;0;0;True;0;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;28;691.1394,-657.0439;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0.5;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;897.0051,-756.5042;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;36;1098.109,-610.7905;Float;False;True;2;Float;ASEMaterialInspector;0;1;TimeSnap/VRPanorama;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;True;False;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;2;False;-1;True;0;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Opaque=RenderType;True;2;0;False;False;False;False;False;False;False;False;False;True;0;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;21;0;1;0
WireConnection;21;1;8;0
WireConnection;4;0;39;0
WireConnection;4;1;40;0
WireConnection;24;0;21;0
WireConnection;24;1;22;0
WireConnection;3;0;37;0
WireConnection;3;1;38;0
WireConnection;16;1;4;0
WireConnection;17;1;3;0
WireConnection;25;0;24;0
WireConnection;18;0;17;0
WireConnection;18;1;16;0
WireConnection;18;2;25;0
WireConnection;26;0;18;0
WireConnection;30;0;26;3
WireConnection;30;1;29;0
WireConnection;28;0;26;0
WireConnection;28;1;26;1
WireConnection;28;2;26;2
WireConnection;28;3;30;0
WireConnection;43;0;41;0
WireConnection;43;1;28;0
WireConnection;36;0;43;0
ASEEND*/
//CHKSM=A34291B8D56F99345183B24DF82C879334BC52E1