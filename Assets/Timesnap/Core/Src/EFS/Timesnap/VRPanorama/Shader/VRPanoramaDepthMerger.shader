// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "VRPanoramaDepthMerger"
{
    Properties
    {
		[HideInInspector] _VTInfoBlock( "VT( auto )", Vector ) = ( 0, 0, 0, 0 )
		_TextureSize("TextureSize", Vector) = (2048,1,0,0)
		_TextureSample6("Texture Sample 6", 2D) = "white" {}
		_TextureSample7("Texture Sample 7", 2D) = "white" {}
		_Left("Left", 2D) = "white" {}
		_Right("Right", 2D) = "white" {}
		_BridgeX("BridgeX", Range( 0 , 2)) = 0
		_ExpandY("ExpandY", Range( 0 , 5)) = 0
    }

	SubShader
	{
		
		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend Off
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		
        Pass
        {
			Name "Custom RT Update"
            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"
            #pragma vertex ASECustomRenderTextureVertexShader
            #pragma fragment frag
            #pragma target 3.0
			

			struct ase_appdata_customrendertexture
			{
				uint vertexID : SV_VertexID;
				
			};

			struct ase_v2f_customrendertexture
			{
				float4 vertex           : SV_POSITION;
				float3 localTexcoord    : TEXCOORD0;    // Texcoord local to the update zone (== globalTexcoord if no partial update zone is specified)
				float3 globalTexcoord   : TEXCOORD1;    // Texcoord relative to the complete custom texture
				uint primitiveID        : TEXCOORD2;    // Index of the update zone (correspond to the index in the updateZones of the Custom Texture)
				float3 direction        : TEXCOORD3;    // For cube textures, direction of the pixel being rendered in the cubemap
				
			};

			uniform sampler2D _TextureSample6;
			uniform sampler2D _TextureSample7;
			uniform float2 _TextureSize;
			uniform sampler2D _Left;
			uniform float _BridgeX;
			uniform float _ExpandY;
			uniform sampler2D _Right;

			ase_v2f_customrendertexture ASECustomRenderTextureVertexShader(ase_appdata_customrendertexture IN  )
			{
				ase_v2f_customrendertexture OUT;
				
			#if UNITY_UV_STARTS_AT_TOP
				const float2 vertexPositions[6] =
				{
					{ -1.0f,  1.0f },
					{ -1.0f, -1.0f },
					{  1.0f, -1.0f },
					{  1.0f,  1.0f },
					{ -1.0f,  1.0f },
					{  1.0f, -1.0f }
				};

				const float2 texCoords[6] =
				{
					{ 0.0f, 0.0f },
					{ 0.0f, 1.0f },
					{ 1.0f, 1.0f },
					{ 1.0f, 0.0f },
					{ 0.0f, 0.0f },
					{ 1.0f, 1.0f }
				};
			#else
				const float2 vertexPositions[6] =
				{
					{  1.0f,  1.0f },
					{ -1.0f, -1.0f },
					{ -1.0f,  1.0f },
					{ -1.0f, -1.0f },
					{  1.0f,  1.0f },
					{  1.0f, -1.0f }
				};

				const float2 texCoords[6] =
				{
					{ 1.0f, 1.0f },
					{ 0.0f, 0.0f },
					{ 0.0f, 1.0f },
					{ 0.0f, 0.0f },
					{ 1.0f, 1.0f },
					{ 1.0f, 0.0f }
				};
			#endif

				uint primitiveID = IN.vertexID / 6;
				uint vertexID = IN.vertexID % 6;
				float3 updateZoneCenter = CustomRenderTextureCenters[primitiveID].xyz;
				float3 updateZoneSize = CustomRenderTextureSizesAndRotations[primitiveID].xyz;
				float rotation = CustomRenderTextureSizesAndRotations[primitiveID].w * UNITY_PI / 180.0f;

			#if !UNITY_UV_STARTS_AT_TOP
				rotation = -rotation;
			#endif

				// Normalize rect if needed
				if (CustomRenderTextureUpdateSpace > 0.0) // Pixel space
				{
					// Normalize xy because we need it in clip space.
					updateZoneCenter.xy /= _CustomRenderTextureInfo.xy;
					updateZoneSize.xy /= _CustomRenderTextureInfo.xy;
				}
				else // normalized space
				{
					// Un-normalize depth because we need actual slice index for culling
					updateZoneCenter.z *= _CustomRenderTextureInfo.z;
					updateZoneSize.z *= _CustomRenderTextureInfo.z;
				}

				// Compute rotation

				// Compute quad vertex position
				float2 clipSpaceCenter = updateZoneCenter.xy * 2.0 - 1.0;
				float2 pos = vertexPositions[vertexID] * updateZoneSize.xy;
				pos = CustomRenderTextureRotate2D(pos, rotation);
				pos.x += clipSpaceCenter.x;
			#if UNITY_UV_STARTS_AT_TOP
				pos.y += clipSpaceCenter.y;
			#else
				pos.y -= clipSpaceCenter.y;
			#endif

				// For 3D texture, cull quads outside of the update zone
				// This is neeeded in additional to the preliminary minSlice/maxSlice done on the CPU because update zones can be disjointed.
				// ie: slices [1..5] and [10..15] for two differents zones so we need to cull out slices 0 and [6..9]
				if (CustomRenderTextureIs3D > 0.0)
				{
					int minSlice = (int)(updateZoneCenter.z - updateZoneSize.z * 0.5);
					int maxSlice = minSlice + (int)updateZoneSize.z;
					if (_CustomRenderTexture3DSlice < minSlice || _CustomRenderTexture3DSlice >= maxSlice)
					{
						pos.xy = float2(1000.0, 1000.0); // Vertex outside of ncs
					}
				}

				OUT.vertex = float4(pos, 0.0, 1.0);
				OUT.primitiveID = asuint(CustomRenderTexturePrimitiveIDs[primitiveID]);
				OUT.localTexcoord = float3(texCoords[vertexID], CustomRenderTexture3DTexcoordW);
				OUT.globalTexcoord = float3(pos.xy * 0.5 + 0.5, CustomRenderTexture3DTexcoordW);
			#if UNITY_UV_STARTS_AT_TOP
				OUT.globalTexcoord.y = 1.0 - OUT.globalTexcoord.y;
			#endif
				OUT.direction = CustomRenderTextureComputeCubeDirection(OUT.globalTexcoord.xy);

				return OUT;
			}

            float4 frag(ase_v2f_customrendertexture IN ) : COLOR
            {
				float4 finalColor;
				float2 break12_g20 = float2( 0,0 );
				float2 appendResult36 = (float2(( _BridgeX / 4096.0 ) , ( _ExpandY / 2048.0 )));
				float2 Pixels27 = appendResult36;
				float2 appendResult61_g20 = (float2(break12_g20.x , ( -3.0 * Pixels27.y )));
				float2 uv058_g20 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult61_g20;
				float grayscale60_g20 = Luminance(tex2D( _Left, uv058_g20 ).rgb);
				float2 appendResult55_g20 = (float2(break12_g20.x , ( 3.0 * Pixels27.y )));
				float2 uv052_g20 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult55_g20;
				float grayscale53_g20 = Luminance(tex2D( _Left, uv052_g20 ).rgb);
				float2 appendResult45_g20 = (float2(break12_g20.x , ( -2.0 * Pixels27.y )));
				float2 uv042_g20 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult45_g20;
				float grayscale43_g20 = Luminance(tex2D( _Left, uv042_g20 ).rgb);
				float2 appendResult20_g20 = (float2(break12_g20.x , ( 2.0 * Pixels27.y )));
				float2 uv021_g20 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult20_g20;
				float grayscale23_g20 = Luminance(tex2D( _Left, uv021_g20 ).rgb);
				float2 appendResult13_g20 = (float2(break12_g20.x , break12_g20.y));
				float2 uv06_g20 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult13_g20;
				float grayscale1_g20 = Luminance(tex2D( _Left, uv06_g20 ).rgb);
				float2 appendResult14_g20 = (float2(-break12_g20.x , 0.0));
				float2 uv07_g20 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult14_g20;
				float grayscale2_g20 = Luminance(tex2D( _Right, uv07_g20 ).rgb);
				float2 appendResult26_g20 = (float2(-break12_g20.x , ( Pixels27.y * -2.0 )));
				float2 uv027_g20 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult26_g20;
				float grayscale28_g20 = Luminance(tex2D( _Right, uv027_g20 ).rgb);
				float2 appendResult36_g20 = (float2(-break12_g20.x , ( Pixels27.y * 2.0 )));
				float2 uv038_g20 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult36_g20;
				float grayscale40_g20 = Luminance(tex2D( _Right, uv038_g20 ).rgb);
				float temp_output_41_0_g20 = max( max( max( max( max( max( max( grayscale60_g20 , grayscale53_g20 ) , grayscale43_g20 ) , grayscale23_g20 ) , grayscale1_g20 ) , grayscale2_g20 ) , grayscale28_g20 ) , grayscale40_g20 );
				float2 break12_g19 = ( float2( 1,0 ) * Pixels27 );
				float2 appendResult61_g19 = (float2(break12_g19.x , ( -3.0 * Pixels27.y )));
				float2 uv058_g19 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult61_g19;
				float grayscale60_g19 = Luminance(tex2D( _Left, uv058_g19 ).rgb);
				float2 appendResult55_g19 = (float2(break12_g19.x , ( 3.0 * Pixels27.y )));
				float2 uv052_g19 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult55_g19;
				float grayscale53_g19 = Luminance(tex2D( _Left, uv052_g19 ).rgb);
				float2 appendResult45_g19 = (float2(break12_g19.x , ( -2.0 * Pixels27.y )));
				float2 uv042_g19 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult45_g19;
				float grayscale43_g19 = Luminance(tex2D( _Left, uv042_g19 ).rgb);
				float2 appendResult20_g19 = (float2(break12_g19.x , ( 2.0 * Pixels27.y )));
				float2 uv021_g19 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult20_g19;
				float grayscale23_g19 = Luminance(tex2D( _Left, uv021_g19 ).rgb);
				float2 appendResult13_g19 = (float2(break12_g19.x , break12_g19.y));
				float2 uv06_g19 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult13_g19;
				float grayscale1_g19 = Luminance(tex2D( _Left, uv06_g19 ).rgb);
				float2 appendResult14_g19 = (float2(-break12_g19.x , 0.0));
				float2 uv07_g19 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult14_g19;
				float grayscale2_g19 = Luminance(tex2D( _Right, uv07_g19 ).rgb);
				float2 appendResult26_g19 = (float2(-break12_g19.x , ( Pixels27.y * -2.0 )));
				float2 uv027_g19 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult26_g19;
				float grayscale28_g19 = Luminance(tex2D( _Right, uv027_g19 ).rgb);
				float2 appendResult36_g19 = (float2(-break12_g19.x , ( Pixels27.y * 2.0 )));
				float2 uv038_g19 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult36_g19;
				float grayscale40_g19 = Luminance(tex2D( _Right, uv038_g19 ).rgb);
				float temp_output_41_0_g19 = max( max( max( max( max( max( max( grayscale60_g19 , grayscale53_g19 ) , grayscale43_g19 ) , grayscale23_g19 ) , grayscale1_g19 ) , grayscale2_g19 ) , grayscale28_g19 ) , grayscale40_g19 );
				float2 break12_g21 = ( float2( 2,0 ) * Pixels27 );
				float2 appendResult61_g21 = (float2(break12_g21.x , ( -3.0 * Pixels27.y )));
				float2 uv058_g21 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult61_g21;
				float grayscale60_g21 = Luminance(tex2D( _Left, uv058_g21 ).rgb);
				float2 appendResult55_g21 = (float2(break12_g21.x , ( 3.0 * Pixels27.y )));
				float2 uv052_g21 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult55_g21;
				float grayscale53_g21 = Luminance(tex2D( _Left, uv052_g21 ).rgb);
				float2 appendResult45_g21 = (float2(break12_g21.x , ( -2.0 * Pixels27.y )));
				float2 uv042_g21 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult45_g21;
				float grayscale43_g21 = Luminance(tex2D( _Left, uv042_g21 ).rgb);
				float2 appendResult20_g21 = (float2(break12_g21.x , ( 2.0 * Pixels27.y )));
				float2 uv021_g21 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult20_g21;
				float grayscale23_g21 = Luminance(tex2D( _Left, uv021_g21 ).rgb);
				float2 appendResult13_g21 = (float2(break12_g21.x , break12_g21.y));
				float2 uv06_g21 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult13_g21;
				float grayscale1_g21 = Luminance(tex2D( _Left, uv06_g21 ).rgb);
				float2 appendResult14_g21 = (float2(-break12_g21.x , 0.0));
				float2 uv07_g21 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult14_g21;
				float grayscale2_g21 = Luminance(tex2D( _Right, uv07_g21 ).rgb);
				float2 appendResult26_g21 = (float2(-break12_g21.x , ( Pixels27.y * -2.0 )));
				float2 uv027_g21 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult26_g21;
				float grayscale28_g21 = Luminance(tex2D( _Right, uv027_g21 ).rgb);
				float2 appendResult36_g21 = (float2(-break12_g21.x , ( Pixels27.y * 2.0 )));
				float2 uv038_g21 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult36_g21;
				float grayscale40_g21 = Luminance(tex2D( _Right, uv038_g21 ).rgb);
				float temp_output_41_0_g21 = max( max( max( max( max( max( max( grayscale60_g21 , grayscale53_g21 ) , grayscale43_g21 ) , grayscale23_g21 ) , grayscale1_g21 ) , grayscale2_g21 ) , grayscale28_g21 ) , grayscale40_g21 );
				float2 break12_g22 = ( float2( 3,0 ) * Pixels27 );
				float2 appendResult61_g22 = (float2(break12_g22.x , ( -3.0 * Pixels27.y )));
				float2 uv058_g22 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult61_g22;
				float grayscale60_g22 = Luminance(tex2D( _Left, uv058_g22 ).rgb);
				float2 appendResult55_g22 = (float2(break12_g22.x , ( 3.0 * Pixels27.y )));
				float2 uv052_g22 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult55_g22;
				float grayscale53_g22 = Luminance(tex2D( _Left, uv052_g22 ).rgb);
				float2 appendResult45_g22 = (float2(break12_g22.x , ( -2.0 * Pixels27.y )));
				float2 uv042_g22 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult45_g22;
				float grayscale43_g22 = Luminance(tex2D( _Left, uv042_g22 ).rgb);
				float2 appendResult20_g22 = (float2(break12_g22.x , ( 2.0 * Pixels27.y )));
				float2 uv021_g22 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult20_g22;
				float grayscale23_g22 = Luminance(tex2D( _Left, uv021_g22 ).rgb);
				float2 appendResult13_g22 = (float2(break12_g22.x , break12_g22.y));
				float2 uv06_g22 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult13_g22;
				float grayscale1_g22 = Luminance(tex2D( _Left, uv06_g22 ).rgb);
				float2 appendResult14_g22 = (float2(-break12_g22.x , 0.0));
				float2 uv07_g22 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult14_g22;
				float grayscale2_g22 = Luminance(tex2D( _Right, uv07_g22 ).rgb);
				float2 appendResult26_g22 = (float2(-break12_g22.x , ( Pixels27.y * -2.0 )));
				float2 uv027_g22 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult26_g22;
				float grayscale28_g22 = Luminance(tex2D( _Right, uv027_g22 ).rgb);
				float2 appendResult36_g22 = (float2(-break12_g22.x , ( Pixels27.y * 2.0 )));
				float2 uv038_g22 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult36_g22;
				float grayscale40_g22 = Luminance(tex2D( _Right, uv038_g22 ).rgb);
				float temp_output_41_0_g22 = max( max( max( max( max( max( max( grayscale60_g22 , grayscale53_g22 ) , grayscale43_g22 ) , grayscale23_g22 ) , grayscale1_g22 ) , grayscale2_g22 ) , grayscale28_g22 ) , grayscale40_g22 );
				float2 break12_g23 = ( float2( 4,0 ) * Pixels27 );
				float2 appendResult61_g23 = (float2(break12_g23.x , ( -3.0 * Pixels27.y )));
				float2 uv058_g23 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult61_g23;
				float grayscale60_g23 = Luminance(tex2D( _Left, uv058_g23 ).rgb);
				float2 appendResult55_g23 = (float2(break12_g23.x , ( 3.0 * Pixels27.y )));
				float2 uv052_g23 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult55_g23;
				float grayscale53_g23 = Luminance(tex2D( _Left, uv052_g23 ).rgb);
				float2 appendResult45_g23 = (float2(break12_g23.x , ( -2.0 * Pixels27.y )));
				float2 uv042_g23 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult45_g23;
				float grayscale43_g23 = Luminance(tex2D( _Left, uv042_g23 ).rgb);
				float2 appendResult20_g23 = (float2(break12_g23.x , ( 2.0 * Pixels27.y )));
				float2 uv021_g23 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult20_g23;
				float grayscale23_g23 = Luminance(tex2D( _Left, uv021_g23 ).rgb);
				float2 appendResult13_g23 = (float2(break12_g23.x , break12_g23.y));
				float2 uv06_g23 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult13_g23;
				float grayscale1_g23 = Luminance(tex2D( _Left, uv06_g23 ).rgb);
				float2 appendResult14_g23 = (float2(-break12_g23.x , 0.0));
				float2 uv07_g23 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult14_g23;
				float grayscale2_g23 = Luminance(tex2D( _Right, uv07_g23 ).rgb);
				float2 appendResult26_g23 = (float2(-break12_g23.x , ( Pixels27.y * -2.0 )));
				float2 uv027_g23 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult26_g23;
				float grayscale28_g23 = Luminance(tex2D( _Right, uv027_g23 ).rgb);
				float2 appendResult36_g23 = (float2(-break12_g23.x , ( Pixels27.y * 2.0 )));
				float2 uv038_g23 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult36_g23;
				float grayscale40_g23 = Luminance(tex2D( _Right, uv038_g23 ).rgb);
				float temp_output_41_0_g23 = max( max( max( max( max( max( max( grayscale60_g23 , grayscale53_g23 ) , grayscale43_g23 ) , grayscale23_g23 ) , grayscale1_g23 ) , grayscale2_g23 ) , grayscale28_g23 ) , grayscale40_g23 );
				float2 break12_g24 = ( float2( 5,0 ) * Pixels27 );
				float2 appendResult61_g24 = (float2(break12_g24.x , ( -3.0 * Pixels27.y )));
				float2 uv058_g24 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult61_g24;
				float grayscale60_g24 = Luminance(tex2D( _Left, uv058_g24 ).rgb);
				float2 appendResult55_g24 = (float2(break12_g24.x , ( 3.0 * Pixels27.y )));
				float2 uv052_g24 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult55_g24;
				float grayscale53_g24 = Luminance(tex2D( _Left, uv052_g24 ).rgb);
				float2 appendResult45_g24 = (float2(break12_g24.x , ( -2.0 * Pixels27.y )));
				float2 uv042_g24 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult45_g24;
				float grayscale43_g24 = Luminance(tex2D( _Left, uv042_g24 ).rgb);
				float2 appendResult20_g24 = (float2(break12_g24.x , ( 2.0 * Pixels27.y )));
				float2 uv021_g24 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult20_g24;
				float grayscale23_g24 = Luminance(tex2D( _Left, uv021_g24 ).rgb);
				float2 appendResult13_g24 = (float2(break12_g24.x , break12_g24.y));
				float2 uv06_g24 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult13_g24;
				float grayscale1_g24 = Luminance(tex2D( _Left, uv06_g24 ).rgb);
				float2 appendResult14_g24 = (float2(-break12_g24.x , 0.0));
				float2 uv07_g24 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult14_g24;
				float grayscale2_g24 = Luminance(tex2D( _Right, uv07_g24 ).rgb);
				float2 appendResult26_g24 = (float2(-break12_g24.x , ( Pixels27.y * -2.0 )));
				float2 uv027_g24 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult26_g24;
				float grayscale28_g24 = Luminance(tex2D( _Right, uv027_g24 ).rgb);
				float2 appendResult36_g24 = (float2(-break12_g24.x , ( Pixels27.y * 2.0 )));
				float2 uv038_g24 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult36_g24;
				float grayscale40_g24 = Luminance(tex2D( _Right, uv038_g24 ).rgb);
				float temp_output_41_0_g24 = max( max( max( max( max( max( max( grayscale60_g24 , grayscale53_g24 ) , grayscale43_g24 ) , grayscale23_g24 ) , grayscale1_g24 ) , grayscale2_g24 ) , grayscale28_g24 ) , grayscale40_g24 );
				float2 break12_g25 = ( float2( 6,0 ) * Pixels27 );
				float2 appendResult61_g25 = (float2(break12_g25.x , ( -3.0 * Pixels27.y )));
				float2 uv058_g25 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult61_g25;
				float grayscale60_g25 = Luminance(tex2D( _Left, uv058_g25 ).rgb);
				float2 appendResult55_g25 = (float2(break12_g25.x , ( 3.0 * Pixels27.y )));
				float2 uv052_g25 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult55_g25;
				float grayscale53_g25 = Luminance(tex2D( _Left, uv052_g25 ).rgb);
				float2 appendResult45_g25 = (float2(break12_g25.x , ( -2.0 * Pixels27.y )));
				float2 uv042_g25 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult45_g25;
				float grayscale43_g25 = Luminance(tex2D( _Left, uv042_g25 ).rgb);
				float2 appendResult20_g25 = (float2(break12_g25.x , ( 2.0 * Pixels27.y )));
				float2 uv021_g25 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult20_g25;
				float grayscale23_g25 = Luminance(tex2D( _Left, uv021_g25 ).rgb);
				float2 appendResult13_g25 = (float2(break12_g25.x , break12_g25.y));
				float2 uv06_g25 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult13_g25;
				float grayscale1_g25 = Luminance(tex2D( _Left, uv06_g25 ).rgb);
				float2 appendResult14_g25 = (float2(-break12_g25.x , 0.0));
				float2 uv07_g25 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult14_g25;
				float grayscale2_g25 = Luminance(tex2D( _Right, uv07_g25 ).rgb);
				float2 appendResult26_g25 = (float2(-break12_g25.x , ( Pixels27.y * -2.0 )));
				float2 uv027_g25 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult26_g25;
				float grayscale28_g25 = Luminance(tex2D( _Right, uv027_g25 ).rgb);
				float2 appendResult36_g25 = (float2(-break12_g25.x , ( Pixels27.y * 2.0 )));
				float2 uv038_g25 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult36_g25;
				float grayscale40_g25 = Luminance(tex2D( _Right, uv038_g25 ).rgb);
				float temp_output_41_0_g25 = max( max( max( max( max( max( max( grayscale60_g25 , grayscale53_g25 ) , grayscale43_g25 ) , grayscale23_g25 ) , grayscale1_g25 ) , grayscale2_g25 ) , grayscale28_g25 ) , grayscale40_g25 );
				float2 break12_g26 = ( float2( 7,0 ) * Pixels27 );
				float2 appendResult61_g26 = (float2(break12_g26.x , ( -3.0 * Pixels27.y )));
				float2 uv058_g26 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult61_g26;
				float grayscale60_g26 = Luminance(tex2D( _Left, uv058_g26 ).rgb);
				float2 appendResult55_g26 = (float2(break12_g26.x , ( 3.0 * Pixels27.y )));
				float2 uv052_g26 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult55_g26;
				float grayscale53_g26 = Luminance(tex2D( _Left, uv052_g26 ).rgb);
				float2 appendResult45_g26 = (float2(break12_g26.x , ( -2.0 * Pixels27.y )));
				float2 uv042_g26 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult45_g26;
				float grayscale43_g26 = Luminance(tex2D( _Left, uv042_g26 ).rgb);
				float2 appendResult20_g26 = (float2(break12_g26.x , ( 2.0 * Pixels27.y )));
				float2 uv021_g26 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult20_g26;
				float grayscale23_g26 = Luminance(tex2D( _Left, uv021_g26 ).rgb);
				float2 appendResult13_g26 = (float2(break12_g26.x , break12_g26.y));
				float2 uv06_g26 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult13_g26;
				float grayscale1_g26 = Luminance(tex2D( _Left, uv06_g26 ).rgb);
				float2 appendResult14_g26 = (float2(-break12_g26.x , 0.0));
				float2 uv07_g26 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult14_g26;
				float grayscale2_g26 = Luminance(tex2D( _Right, uv07_g26 ).rgb);
				float2 appendResult26_g26 = (float2(-break12_g26.x , ( Pixels27.y * -2.0 )));
				float2 uv027_g26 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult26_g26;
				float grayscale28_g26 = Luminance(tex2D( _Right, uv027_g26 ).rgb);
				float2 appendResult36_g26 = (float2(-break12_g26.x , ( Pixels27.y * 2.0 )));
				float2 uv038_g26 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult36_g26;
				float grayscale40_g26 = Luminance(tex2D( _Right, uv038_g26 ).rgb);
				float temp_output_41_0_g26 = max( max( max( max( max( max( max( grayscale60_g26 , grayscale53_g26 ) , grayscale43_g26 ) , grayscale23_g26 ) , grayscale1_g26 ) , grayscale2_g26 ) , grayscale28_g26 ) , grayscale40_g26 );
				float2 break12_g27 = ( float2( 8,0 ) * Pixels27 );
				float2 appendResult61_g27 = (float2(break12_g27.x , ( -3.0 * Pixels27.y )));
				float2 uv058_g27 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult61_g27;
				float grayscale60_g27 = Luminance(tex2D( _Left, uv058_g27 ).rgb);
				float2 appendResult55_g27 = (float2(break12_g27.x , ( 3.0 * Pixels27.y )));
				float2 uv052_g27 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult55_g27;
				float grayscale53_g27 = Luminance(tex2D( _Left, uv052_g27 ).rgb);
				float2 appendResult45_g27 = (float2(break12_g27.x , ( -2.0 * Pixels27.y )));
				float2 uv042_g27 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult45_g27;
				float grayscale43_g27 = Luminance(tex2D( _Left, uv042_g27 ).rgb);
				float2 appendResult20_g27 = (float2(break12_g27.x , ( 2.0 * Pixels27.y )));
				float2 uv021_g27 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult20_g27;
				float grayscale23_g27 = Luminance(tex2D( _Left, uv021_g27 ).rgb);
				float2 appendResult13_g27 = (float2(break12_g27.x , break12_g27.y));
				float2 uv06_g27 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult13_g27;
				float grayscale1_g27 = Luminance(tex2D( _Left, uv06_g27 ).rgb);
				float2 appendResult14_g27 = (float2(-break12_g27.x , 0.0));
				float2 uv07_g27 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult14_g27;
				float grayscale2_g27 = Luminance(tex2D( _Right, uv07_g27 ).rgb);
				float2 appendResult26_g27 = (float2(-break12_g27.x , ( Pixels27.y * -2.0 )));
				float2 uv027_g27 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult26_g27;
				float grayscale28_g27 = Luminance(tex2D( _Right, uv027_g27 ).rgb);
				float2 appendResult36_g27 = (float2(-break12_g27.x , ( Pixels27.y * 2.0 )));
				float2 uv038_g27 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult36_g27;
				float grayscale40_g27 = Luminance(tex2D( _Right, uv038_g27 ).rgb);
				float temp_output_41_0_g27 = max( max( max( max( max( max( max( grayscale60_g27 , grayscale53_g27 ) , grayscale43_g27 ) , grayscale23_g27 ) , grayscale1_g27 ) , grayscale2_g27 ) , grayscale28_g27 ) , grayscale40_g27 );
				float2 break12_g28 = ( float2( 9,0 ) * Pixels27 );
				float2 appendResult61_g28 = (float2(break12_g28.x , ( -3.0 * Pixels27.y )));
				float2 uv058_g28 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult61_g28;
				float grayscale60_g28 = Luminance(tex2D( _Left, uv058_g28 ).rgb);
				float2 appendResult55_g28 = (float2(break12_g28.x , ( 3.0 * Pixels27.y )));
				float2 uv052_g28 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult55_g28;
				float grayscale53_g28 = Luminance(tex2D( _Left, uv052_g28 ).rgb);
				float2 appendResult45_g28 = (float2(break12_g28.x , ( -2.0 * Pixels27.y )));
				float2 uv042_g28 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult45_g28;
				float grayscale43_g28 = Luminance(tex2D( _Left, uv042_g28 ).rgb);
				float2 appendResult20_g28 = (float2(break12_g28.x , ( 2.0 * Pixels27.y )));
				float2 uv021_g28 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult20_g28;
				float grayscale23_g28 = Luminance(tex2D( _Left, uv021_g28 ).rgb);
				float2 appendResult13_g28 = (float2(break12_g28.x , break12_g28.y));
				float2 uv06_g28 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult13_g28;
				float grayscale1_g28 = Luminance(tex2D( _Left, uv06_g28 ).rgb);
				float2 appendResult14_g28 = (float2(-break12_g28.x , 0.0));
				float2 uv07_g28 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult14_g28;
				float grayscale2_g28 = Luminance(tex2D( _Right, uv07_g28 ).rgb);
				float2 appendResult26_g28 = (float2(-break12_g28.x , ( Pixels27.y * -2.0 )));
				float2 uv027_g28 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult26_g28;
				float grayscale28_g28 = Luminance(tex2D( _Right, uv027_g28 ).rgb);
				float2 appendResult36_g28 = (float2(-break12_g28.x , ( Pixels27.y * 2.0 )));
				float2 uv038_g28 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult36_g28;
				float grayscale40_g28 = Luminance(tex2D( _Right, uv038_g28 ).rgb);
				float temp_output_41_0_g28 = max( max( max( max( max( max( max( grayscale60_g28 , grayscale53_g28 ) , grayscale43_g28 ) , grayscale23_g28 ) , grayscale1_g28 ) , grayscale2_g28 ) , grayscale28_g28 ) , grayscale40_g28 );
				float2 break12_g29 = ( float2( 10,0 ) * Pixels27 );
				float2 appendResult61_g29 = (float2(break12_g29.x , ( -3.0 * Pixels27.y )));
				float2 uv058_g29 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult61_g29;
				float grayscale60_g29 = Luminance(tex2D( _Left, uv058_g29 ).rgb);
				float2 appendResult55_g29 = (float2(break12_g29.x , ( 3.0 * Pixels27.y )));
				float2 uv052_g29 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult55_g29;
				float grayscale53_g29 = Luminance(tex2D( _Left, uv052_g29 ).rgb);
				float2 appendResult45_g29 = (float2(break12_g29.x , ( -2.0 * Pixels27.y )));
				float2 uv042_g29 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult45_g29;
				float grayscale43_g29 = Luminance(tex2D( _Left, uv042_g29 ).rgb);
				float2 appendResult20_g29 = (float2(break12_g29.x , ( 2.0 * Pixels27.y )));
				float2 uv021_g29 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult20_g29;
				float grayscale23_g29 = Luminance(tex2D( _Left, uv021_g29 ).rgb);
				float2 appendResult13_g29 = (float2(break12_g29.x , break12_g29.y));
				float2 uv06_g29 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult13_g29;
				float grayscale1_g29 = Luminance(tex2D( _Left, uv06_g29 ).rgb);
				float2 appendResult14_g29 = (float2(-break12_g29.x , 0.0));
				float2 uv07_g29 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult14_g29;
				float grayscale2_g29 = Luminance(tex2D( _Right, uv07_g29 ).rgb);
				float2 appendResult26_g29 = (float2(-break12_g29.x , ( Pixels27.y * -2.0 )));
				float2 uv027_g29 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult26_g29;
				float grayscale28_g29 = Luminance(tex2D( _Right, uv027_g29 ).rgb);
				float2 appendResult36_g29 = (float2(-break12_g29.x , ( Pixels27.y * 2.0 )));
				float2 uv038_g29 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult36_g29;
				float grayscale40_g29 = Luminance(tex2D( _Right, uv038_g29 ).rgb);
				float temp_output_41_0_g29 = max( max( max( max( max( max( max( grayscale60_g29 , grayscale53_g29 ) , grayscale43_g29 ) , grayscale23_g29 ) , grayscale1_g29 ) , grayscale2_g29 ) , grayscale28_g29 ) , grayscale40_g29 );
				float2 break12_g30 = ( float2( 11,0 ) * Pixels27 );
				float2 appendResult61_g30 = (float2(break12_g30.x , ( -3.0 * Pixels27.y )));
				float2 uv058_g30 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult61_g30;
				float grayscale60_g30 = Luminance(tex2D( _Left, uv058_g30 ).rgb);
				float2 appendResult55_g30 = (float2(break12_g30.x , ( 3.0 * Pixels27.y )));
				float2 uv052_g30 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult55_g30;
				float grayscale53_g30 = Luminance(tex2D( _Left, uv052_g30 ).rgb);
				float2 appendResult45_g30 = (float2(break12_g30.x , ( -2.0 * Pixels27.y )));
				float2 uv042_g30 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult45_g30;
				float grayscale43_g30 = Luminance(tex2D( _Left, uv042_g30 ).rgb);
				float2 appendResult20_g30 = (float2(break12_g30.x , ( 2.0 * Pixels27.y )));
				float2 uv021_g30 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult20_g30;
				float grayscale23_g30 = Luminance(tex2D( _Left, uv021_g30 ).rgb);
				float2 appendResult13_g30 = (float2(break12_g30.x , break12_g30.y));
				float2 uv06_g30 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult13_g30;
				float grayscale1_g30 = Luminance(tex2D( _Left, uv06_g30 ).rgb);
				float2 appendResult14_g30 = (float2(-break12_g30.x , 0.0));
				float2 uv07_g30 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult14_g30;
				float grayscale2_g30 = Luminance(tex2D( _Right, uv07_g30 ).rgb);
				float2 appendResult26_g30 = (float2(-break12_g30.x , ( Pixels27.y * -2.0 )));
				float2 uv027_g30 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult26_g30;
				float grayscale28_g30 = Luminance(tex2D( _Right, uv027_g30 ).rgb);
				float2 appendResult36_g30 = (float2(-break12_g30.x , ( Pixels27.y * 2.0 )));
				float2 uv038_g30 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult36_g30;
				float grayscale40_g30 = Luminance(tex2D( _Right, uv038_g30 ).rgb);
				float temp_output_41_0_g30 = max( max( max( max( max( max( max( grayscale60_g30 , grayscale53_g30 ) , grayscale43_g30 ) , grayscale23_g30 ) , grayscale1_g30 ) , grayscale2_g30 ) , grayscale28_g30 ) , grayscale40_g30 );
				float2 break12_g31 = ( float2( 12,0 ) * Pixels27 );
				float2 appendResult61_g31 = (float2(break12_g31.x , ( -3.0 * Pixels27.y )));
				float2 uv058_g31 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult61_g31;
				float grayscale60_g31 = Luminance(tex2D( _Left, uv058_g31 ).rgb);
				float2 appendResult55_g31 = (float2(break12_g31.x , ( 3.0 * Pixels27.y )));
				float2 uv052_g31 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult55_g31;
				float grayscale53_g31 = Luminance(tex2D( _Left, uv052_g31 ).rgb);
				float2 appendResult45_g31 = (float2(break12_g31.x , ( -2.0 * Pixels27.y )));
				float2 uv042_g31 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult45_g31;
				float grayscale43_g31 = Luminance(tex2D( _Left, uv042_g31 ).rgb);
				float2 appendResult20_g31 = (float2(break12_g31.x , ( 2.0 * Pixels27.y )));
				float2 uv021_g31 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult20_g31;
				float grayscale23_g31 = Luminance(tex2D( _Left, uv021_g31 ).rgb);
				float2 appendResult13_g31 = (float2(break12_g31.x , break12_g31.y));
				float2 uv06_g31 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult13_g31;
				float grayscale1_g31 = Luminance(tex2D( _Left, uv06_g31 ).rgb);
				float2 appendResult14_g31 = (float2(-break12_g31.x , 0.0));
				float2 uv07_g31 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult14_g31;
				float grayscale2_g31 = Luminance(tex2D( _Right, uv07_g31 ).rgb);
				float2 appendResult26_g31 = (float2(-break12_g31.x , ( Pixels27.y * -2.0 )));
				float2 uv027_g31 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult26_g31;
				float grayscale28_g31 = Luminance(tex2D( _Right, uv027_g31 ).rgb);
				float2 appendResult36_g31 = (float2(-break12_g31.x , ( Pixels27.y * 2.0 )));
				float2 uv038_g31 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult36_g31;
				float grayscale40_g31 = Luminance(tex2D( _Right, uv038_g31 ).rgb);
				float temp_output_41_0_g31 = max( max( max( max( max( max( max( grayscale60_g31 , grayscale53_g31 ) , grayscale43_g31 ) , grayscale23_g31 ) , grayscale1_g31 ) , grayscale2_g31 ) , grayscale28_g31 ) , grayscale40_g31 );
				float2 break12_g32 = ( float2( 13,0 ) * Pixels27 );
				float2 appendResult61_g32 = (float2(break12_g32.x , ( -3.0 * Pixels27.y )));
				float2 uv058_g32 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult61_g32;
				float grayscale60_g32 = Luminance(tex2D( _Left, uv058_g32 ).rgb);
				float2 appendResult55_g32 = (float2(break12_g32.x , ( 3.0 * Pixels27.y )));
				float2 uv052_g32 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult55_g32;
				float grayscale53_g32 = Luminance(tex2D( _Left, uv052_g32 ).rgb);
				float2 appendResult45_g32 = (float2(break12_g32.x , ( -2.0 * Pixels27.y )));
				float2 uv042_g32 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult45_g32;
				float grayscale43_g32 = Luminance(tex2D( _Left, uv042_g32 ).rgb);
				float2 appendResult20_g32 = (float2(break12_g32.x , ( 2.0 * Pixels27.y )));
				float2 uv021_g32 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult20_g32;
				float grayscale23_g32 = Luminance(tex2D( _Left, uv021_g32 ).rgb);
				float2 appendResult13_g32 = (float2(break12_g32.x , break12_g32.y));
				float2 uv06_g32 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult13_g32;
				float grayscale1_g32 = Luminance(tex2D( _Left, uv06_g32 ).rgb);
				float2 appendResult14_g32 = (float2(-break12_g32.x , 0.0));
				float2 uv07_g32 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult14_g32;
				float grayscale2_g32 = Luminance(tex2D( _Right, uv07_g32 ).rgb);
				float2 appendResult26_g32 = (float2(-break12_g32.x , ( Pixels27.y * -2.0 )));
				float2 uv027_g32 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult26_g32;
				float grayscale28_g32 = Luminance(tex2D( _Right, uv027_g32 ).rgb);
				float2 appendResult36_g32 = (float2(-break12_g32.x , ( Pixels27.y * 2.0 )));
				float2 uv038_g32 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult36_g32;
				float grayscale40_g32 = Luminance(tex2D( _Right, uv038_g32 ).rgb);
				float temp_output_41_0_g32 = max( max( max( max( max( max( max( grayscale60_g32 , grayscale53_g32 ) , grayscale43_g32 ) , grayscale23_g32 ) , grayscale1_g32 ) , grayscale2_g32 ) , grayscale28_g32 ) , grayscale40_g32 );
				float2 break12_g33 = ( float2( 14,0 ) * Pixels27 );
				float2 appendResult61_g33 = (float2(break12_g33.x , ( -3.0 * Pixels27.y )));
				float2 uv058_g33 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult61_g33;
				float grayscale60_g33 = Luminance(tex2D( _Left, uv058_g33 ).rgb);
				float2 appendResult55_g33 = (float2(break12_g33.x , ( 3.0 * Pixels27.y )));
				float2 uv052_g33 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult55_g33;
				float grayscale53_g33 = Luminance(tex2D( _Left, uv052_g33 ).rgb);
				float2 appendResult45_g33 = (float2(break12_g33.x , ( -2.0 * Pixels27.y )));
				float2 uv042_g33 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult45_g33;
				float grayscale43_g33 = Luminance(tex2D( _Left, uv042_g33 ).rgb);
				float2 appendResult20_g33 = (float2(break12_g33.x , ( 2.0 * Pixels27.y )));
				float2 uv021_g33 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult20_g33;
				float grayscale23_g33 = Luminance(tex2D( _Left, uv021_g33 ).rgb);
				float2 appendResult13_g33 = (float2(break12_g33.x , break12_g33.y));
				float2 uv06_g33 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult13_g33;
				float grayscale1_g33 = Luminance(tex2D( _Left, uv06_g33 ).rgb);
				float2 appendResult14_g33 = (float2(-break12_g33.x , 0.0));
				float2 uv07_g33 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult14_g33;
				float grayscale2_g33 = Luminance(tex2D( _Right, uv07_g33 ).rgb);
				float2 appendResult26_g33 = (float2(-break12_g33.x , ( Pixels27.y * -2.0 )));
				float2 uv027_g33 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult26_g33;
				float grayscale28_g33 = Luminance(tex2D( _Right, uv027_g33 ).rgb);
				float2 appendResult36_g33 = (float2(-break12_g33.x , ( Pixels27.y * 2.0 )));
				float2 uv038_g33 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult36_g33;
				float grayscale40_g33 = Luminance(tex2D( _Right, uv038_g33 ).rgb);
				float temp_output_41_0_g33 = max( max( max( max( max( max( max( grayscale60_g33 , grayscale53_g33 ) , grayscale43_g33 ) , grayscale23_g33 ) , grayscale1_g33 ) , grayscale2_g33 ) , grayscale28_g33 ) , grayscale40_g33 );
				float2 break12_g34 = ( float2( 15,0 ) * Pixels27 );
				float2 appendResult61_g34 = (float2(break12_g34.x , ( -3.0 * Pixels27.y )));
				float2 uv058_g34 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult61_g34;
				float grayscale60_g34 = Luminance(tex2D( _Left, uv058_g34 ).rgb);
				float2 appendResult55_g34 = (float2(break12_g34.x , ( 3.0 * Pixels27.y )));
				float2 uv052_g34 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult55_g34;
				float grayscale53_g34 = Luminance(tex2D( _Left, uv052_g34 ).rgb);
				float2 appendResult45_g34 = (float2(break12_g34.x , ( -2.0 * Pixels27.y )));
				float2 uv042_g34 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult45_g34;
				float grayscale43_g34 = Luminance(tex2D( _Left, uv042_g34 ).rgb);
				float2 appendResult20_g34 = (float2(break12_g34.x , ( 2.0 * Pixels27.y )));
				float2 uv021_g34 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult20_g34;
				float grayscale23_g34 = Luminance(tex2D( _Left, uv021_g34 ).rgb);
				float2 appendResult13_g34 = (float2(break12_g34.x , break12_g34.y));
				float2 uv06_g34 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult13_g34;
				float grayscale1_g34 = Luminance(tex2D( _Left, uv06_g34 ).rgb);
				float2 appendResult14_g34 = (float2(-break12_g34.x , 0.0));
				float2 uv07_g34 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult14_g34;
				float grayscale2_g34 = Luminance(tex2D( _Right, uv07_g34 ).rgb);
				float2 appendResult26_g34 = (float2(-break12_g34.x , ( Pixels27.y * -2.0 )));
				float2 uv027_g34 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult26_g34;
				float grayscale28_g34 = Luminance(tex2D( _Right, uv027_g34 ).rgb);
				float2 appendResult36_g34 = (float2(-break12_g34.x , ( Pixels27.y * 2.0 )));
				float2 uv038_g34 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult36_g34;
				float grayscale40_g34 = Luminance(tex2D( _Right, uv038_g34 ).rgb);
				float temp_output_41_0_g34 = max( max( max( max( max( max( max( grayscale60_g34 , grayscale53_g34 ) , grayscale43_g34 ) , grayscale23_g34 ) , grayscale1_g34 ) , grayscale2_g34 ) , grayscale28_g34 ) , grayscale40_g34 );
				float2 break12_g35 = ( float2( 16,0 ) * Pixels27 );
				float2 appendResult61_g35 = (float2(break12_g35.x , ( -3.0 * Pixels27.y )));
				float2 uv058_g35 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult61_g35;
				float grayscale60_g35 = Luminance(tex2D( _Left, uv058_g35 ).rgb);
				float2 appendResult55_g35 = (float2(break12_g35.x , ( 3.0 * Pixels27.y )));
				float2 uv052_g35 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult55_g35;
				float grayscale53_g35 = Luminance(tex2D( _Left, uv052_g35 ).rgb);
				float2 appendResult45_g35 = (float2(break12_g35.x , ( -2.0 * Pixels27.y )));
				float2 uv042_g35 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult45_g35;
				float grayscale43_g35 = Luminance(tex2D( _Left, uv042_g35 ).rgb);
				float2 appendResult20_g35 = (float2(break12_g35.x , ( 2.0 * Pixels27.y )));
				float2 uv021_g35 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult20_g35;
				float grayscale23_g35 = Luminance(tex2D( _Left, uv021_g35 ).rgb);
				float2 appendResult13_g35 = (float2(break12_g35.x , break12_g35.y));
				float2 uv06_g35 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult13_g35;
				float grayscale1_g35 = Luminance(tex2D( _Left, uv06_g35 ).rgb);
				float2 appendResult14_g35 = (float2(-break12_g35.x , 0.0));
				float2 uv07_g35 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult14_g35;
				float grayscale2_g35 = Luminance(tex2D( _Right, uv07_g35 ).rgb);
				float2 appendResult26_g35 = (float2(-break12_g35.x , ( Pixels27.y * -2.0 )));
				float2 uv027_g35 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult26_g35;
				float grayscale28_g35 = Luminance(tex2D( _Right, uv027_g35 ).rgb);
				float2 appendResult36_g35 = (float2(-break12_g35.x , ( Pixels27.y * 2.0 )));
				float2 uv038_g35 = IN.localTexcoord.xy * float2( 1,1 ) + appendResult36_g35;
				float grayscale40_g35 = Luminance(tex2D( _Right, uv038_g35 ).rgb);
				float temp_output_41_0_g35 = max( max( max( max( max( max( max( grayscale60_g35 , grayscale53_g35 ) , grayscale43_g35 ) , grayscale23_g35 ) , grayscale1_g35 ) , grayscale2_g35 ) , grayscale28_g35 ) , grayscale40_g35 );
				float temp_output_128_0 = max( max( max( max( max( max( max( max( max( max( max( max( max( max( max( max( temp_output_41_0_g20 , temp_output_41_0_g19 ) , temp_output_41_0_g21 ) , temp_output_41_0_g22 ) , temp_output_41_0_g23 ) , temp_output_41_0_g24 ) , temp_output_41_0_g25 ) , temp_output_41_0_g26 ) , temp_output_41_0_g27 ) , temp_output_41_0_g28 ) , temp_output_41_0_g29 ) , temp_output_41_0_g30 ) , temp_output_41_0_g31 ) , temp_output_41_0_g32 ) , temp_output_41_0_g33 ) , temp_output_41_0_g34 ) , temp_output_41_0_g35 );
				float4 appendResult820 = (float4(temp_output_128_0 , temp_output_128_0 , temp_output_128_0 , 1.0));
				
                finalColor = appendResult820;
				return finalColor;
            }
            ENDCG
		}
    }
	
	CustomEditor "ASEMaterialInspector"
	
}
/*ASEBEGIN
Version=16700
631;271;1683;939;1019.969;-3802.976;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;87;-2483.117,-717.134;Float;False;Property;_BridgeX;BridgeX;6;0;Create;True;0;0;True;0;0;0.342;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;88;-2176.647,-293.6722;Float;False;Property;_ExpandY;ExpandY;7;0;Create;True;0;0;True;0;0;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;16;-1914.2,-147.8;Float;False;2;0;FLOAT;0;False;1;FLOAT;2048;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;15;-2051.38,-510.893;Float;False;2;0;FLOAT;0;False;1;FLOAT;4096;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;36;-1759.083,-480.4841;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;27;-1589.583,-494.5457;Float;False;Pixels;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;35;-1263.534,-163.9498;Float;False;Constant;_Vector1;Vector 1;5;0;Create;True;0;0;False;0;1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;38;-1275.127,-15.86003;Float;False;27;Pixels;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;41;-1179.519,179.5922;Float;False;Constant;_Vector2;Vector 2;5;0;Create;True;0;0;False;0;2,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;42;-1182.122,318.6929;Float;False;27;Pixels;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-1008.15,-82.01749;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.VirtualTextureObject;22;-2617.685,1881.818;Float;True;Property;_Left;Left;4;0;Create;True;0;0;True;0;None;None;False;white;Auto;Unity5;0;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.VirtualTextureObject;23;-2667.917,2318.447;Float;True;Property;_Right;Right;5;0;Create;True;0;0;True;0;None;None;False;white;Auto;Unity5;0;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.Vector2Node;24;-1208.551,-368.7975;Float;False;Constant;_Vector0;Vector 0;5;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.FunctionNode;804;-737.8777,-178.8849;Float;False;VRPanoramaDepthMergerCore;1;;19;f187eff20ec9f3245917f2506e8568d0;0;4;33;FLOAT2;0,0;False;9;FLOAT2;0,0;False;10;SAMPLER2D;0,0;False;11;SAMPLER2D;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;46;-1163.919,633.2924;Float;False;27;Pixels;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;803;-848.9628,-428.7584;Float;False;VRPanoramaDepthMergerCore;1;;20;f187eff20ec9f3245917f2506e8568d0;0;4;33;FLOAT2;0,0;False;9;FLOAT2;0,0;False;10;SAMPLER2D;0,0;False;11;SAMPLER2D;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-981.9221,264.0929;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;45;-1161.317,494.1918;Float;False;Constant;_Vector3;Vector 3;5;0;Create;True;0;0;False;0;3,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.FunctionNode;805;-713.1096,111.8219;Float;False;VRPanoramaDepthMergerCore;1;;21;f187eff20ec9f3245917f2506e8568d0;0;4;33;FLOAT2;0,0;False;9;FLOAT2;0,0;False;10;SAMPLER2D;0,0;False;11;SAMPLER2D;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-963.7208,578.6924;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;50;-1150.918,919.2914;Float;False;27;Pixels;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;49;-1152.215,774.9908;Float;False;Constant;_Vector4;Vector 4;5;0;Create;True;0;0;False;0;4,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMaxOpNode;70;-389.186,-282.1498;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-950.7195,864.6915;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;53;-1150.917,1084.391;Float;False;Constant;_Vector5;Vector 5;5;0;Create;True;0;0;False;0;5,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;54;-1153.52,1223.492;Float;False;27;Pixels;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;71;-301.673,-23.11039;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;806;-703.7215,427.8904;Float;False;VRPanoramaDepthMergerCore;1;;22;f187eff20ec9f3245917f2506e8568d0;0;4;33;FLOAT2;0,0;False;9;FLOAT2;0,0;False;10;SAMPLER2D;0,0;False;11;SAMPLER2D;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;58;-1152.221,1544.593;Float;False;27;Pixels;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;-953.3198,1168.892;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;807;-690.7203,713.8893;Float;False;VRPanoramaDepthMergerCore;1;;23;f187eff20ec9f3245917f2506e8568d0;0;4;33;FLOAT2;0,0;False;9;FLOAT2;0,0;False;10;SAMPLER2D;0,0;False;11;SAMPLER2D;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;57;-1149.618,1405.492;Float;False;Constant;_Vector6;Vector 6;5;0;Create;True;0;0;False;0;6,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMaxOpNode;72;-280.6695,328.6931;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;62;-1167.821,1848.793;Float;False;27;Pixels;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;-952.0208,1489.993;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;73;-282.4222,654.2422;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;61;-1165.218,1709.692;Float;False;Constant;_Vector7;Vector 7;5;0;Create;True;0;0;False;0;7,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.FunctionNode;808;-634.2211,997.5333;Float;False;VRPanoramaDepthMergerCore;1;;24;f187eff20ec9f3245917f2506e8568d0;0;4;33;FLOAT2;0,0;False;9;FLOAT2;0,0;False;10;SAMPLER2D;0,0;False;11;SAMPLER2D;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;66;-1135.323,2151.693;Float;False;27;Pixels;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;-967.6213,1794.193;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;809;-692.0214,1339.19;Float;False;VRPanoramaDepthMergerCore;1;;25;f187eff20ec9f3245917f2506e8568d0;0;4;33;FLOAT2;0,0;False;9;FLOAT2;0,0;False;10;SAMPLER2D;0,0;False;11;SAMPLER2D;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;65;-1132.72,2012.592;Float;False;Constant;_Vector8;Vector 8;5;0;Create;True;0;0;False;0;8,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMaxOpNode;74;-226.4108,958.7892;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;90;-1144.311,2484.647;Float;False;27;Pixels;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;-935.123,2097.093;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;810;-707.6219,1643.391;Float;False;VRPanoramaDepthMergerCore;1;;26;f187eff20ec9f3245917f2506e8568d0;0;4;33;FLOAT2;0,0;False;9;FLOAT2;0,0;False;10;SAMPLER2D;0,0;False;11;SAMPLER2D;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;89;-1141.708,2345.547;Float;False;Constant;_Vector9;Vector 9;5;0;Create;True;0;0;False;0;9,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMaxOpNode;75;-250.9155,1275.587;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;91;-944.111,2430.047;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;811;-675.1237,1946.29;Float;False;VRPanoramaDepthMergerCore;1;;27;f187eff20ec9f3245917f2506e8568d0;0;4;33;FLOAT2;0,0;False;9;FLOAT2;0,0;False;10;SAMPLER2D;0,0;False;11;SAMPLER2D;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;76;-285.9208,1578.382;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;92;-1125.753,2637.722;Float;False;Constant;_Vector10;Vector 10;5;0;Create;True;0;0;False;0;10,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;93;-1128.356,2776.823;Float;False;27;Pixels;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;812;-684.1119,2279.246;Float;False;VRPanoramaDepthMergerCore;1;;28;f187eff20ec9f3245917f2506e8568d0;0;4;33;FLOAT2;0,0;False;9;FLOAT2;0,0;False;10;SAMPLER2D;0,0;False;11;SAMPLER2D;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;77;-280.9506,1842.346;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;94;-928.1561,2722.223;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;96;-1110.153,3091.422;Float;False;27;Pixels;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;95;-1107.551,2952.322;Float;False;Constant;_Vector11;Vector 11;5;0;Create;True;0;0;False;0;11,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.FunctionNode;813;-668.157,2571.421;Float;False;VRPanoramaDepthMergerCore;1;;29;f187eff20ec9f3245917f2506e8568d0;0;4;33;FLOAT2;0,0;False;9;FLOAT2;0,0;False;10;SAMPLER2D;0,0;False;11;SAMPLER2D;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;100;-909.9548,3036.823;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;101;-1098.449,3233.121;Float;False;Constant;_Vector12;Vector 12;5;0;Create;True;0;0;False;0;12,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMaxOpNode;102;-335.4201,2173.981;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;99;-1097.152,3377.421;Float;False;27;Pixels;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;106;-896.9535,3322.822;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;107;-1099.754,3681.622;Float;False;27;Pixels;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;105;-1097.151,3542.521;Float;False;Constant;_Vector13;Vector 13;5;0;Create;True;0;0;False;0;13,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMaxOpNode;104;-258.8323,2421.364;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;814;-649.9557,2886.021;Float;False;VRPanoramaDepthMergerCore;1;;30;f187eff20ec9f3245917f2506e8568d0;0;4;33;FLOAT2;0,0;False;9;FLOAT2;0,0;False;10;SAMPLER2D;0,0;False;11;SAMPLER2D;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;109;-1098.455,4002.723;Float;False;27;Pixels;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;112;-1095.852,3863.622;Float;False;Constant;_Vector14;Vector 14;5;0;Create;True;0;0;False;0;14,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.FunctionNode;815;-636.9545,3172.019;Float;False;VRPanoramaDepthMergerCore;1;;31;f187eff20ec9f3245917f2506e8568d0;0;4;33;FLOAT2;0,0;False;9;FLOAT2;0,0;False;10;SAMPLER2D;0,0;False;11;SAMPLER2D;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;111;-899.5538,3627.022;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;110;-226.9037,2786.823;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;113;-1111.452,4167.822;Float;False;Constant;_Vector15;Vector 15;5;0;Create;True;0;0;False;0;15,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMaxOpNode;117;-228.6564,3112.372;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;116;-898.2548,3948.124;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;115;-1114.055,4306.922;Float;False;27;Pixels;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;816;-580.4553,3455.663;Float;False;VRPanoramaDepthMergerCore;1;;32;f187eff20ec9f3245917f2506e8568d0;0;4;33;FLOAT2;0,0;False;9;FLOAT2;0,0;False;10;SAMPLER2D;0,0;False;11;SAMPLER2D;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;817;-638.2556,3797.32;Float;False;VRPanoramaDepthMergerCore;1;;33;f187eff20ec9f3245917f2506e8568d0;0;4;33;FLOAT2;0,0;False;9;FLOAT2;0,0;False;10;SAMPLER2D;0,0;False;11;SAMPLER2D;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;122;-913.8552,4252.323;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;119;-172.645,3416.919;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;121;-1081.557,4609.822;Float;False;27;Pixels;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;118;-1078.954,4470.721;Float;False;Constant;_Vector16;Vector 16;5;0;Create;True;0;0;False;0;16,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;124;-881.3569,4555.222;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;818;-653.8561,4101.521;Float;False;VRPanoramaDepthMergerCore;1;;34;f187eff20ec9f3245917f2506e8568d0;0;4;33;FLOAT2;0,0;False;9;FLOAT2;0,0;False;10;SAMPLER2D;0,0;False;11;SAMPLER2D;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;123;-197.1497,3733.717;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;819;-621.3579,4404.419;Float;False;VRPanoramaDepthMergerCore;1;;35;f187eff20ec9f3245917f2506e8568d0;0;4;33;FLOAT2;0,0;False;9;FLOAT2;0,0;False;10;SAMPLER2D;0,0;False;11;SAMPLER2D;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;126;-232.155,4036.512;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;821;-146.9692,4476.976;Float;False;Constant;_Float0;Float 0;22;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;128;-207.1196,4345.623;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;11;-2692.501,-464.6002;Float;False;Property;_TextureSize;TextureSize;0;0;Create;True;0;0;True;0;2048,1;4096,4096;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.DynamicAppendNode;820;62.03076,4346.976;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;138;320.953,4334.412;Float;False;True;2;Float;ASEMaterialInspector;0;3;VRPanoramaDepthMerger;32120270d1b3a8746af2aca8bc749736;True;Custom RT Update;0;0;Custom RT Update;1;True;0;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;True;False;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;0;True;2;0;False;False;False;False;False;False;False;False;False;False;True;2;0;;0;0;Standard;0;0;1;True;False;1;0;FLOAT4;0,0,0,0;False;0
WireConnection;16;0;88;0
WireConnection;15;0;87;0
WireConnection;36;0;15;0
WireConnection;36;1;16;0
WireConnection;27;0;36;0
WireConnection;37;0;35;0
WireConnection;37;1;38;0
WireConnection;804;33;38;0
WireConnection;804;9;37;0
WireConnection;804;10;22;0
WireConnection;804;11;23;0
WireConnection;803;33;38;0
WireConnection;803;9;24;0
WireConnection;803;10;22;0
WireConnection;803;11;23;0
WireConnection;43;0;41;0
WireConnection;43;1;42;0
WireConnection;805;33;42;0
WireConnection;805;9;43;0
WireConnection;805;10;22;0
WireConnection;805;11;23;0
WireConnection;47;0;45;0
WireConnection;47;1;46;0
WireConnection;70;0;803;0
WireConnection;70;1;804;0
WireConnection;51;0;49;0
WireConnection;51;1;50;0
WireConnection;71;0;70;0
WireConnection;71;1;805;0
WireConnection;806;33;46;0
WireConnection;806;9;47;0
WireConnection;806;10;22;0
WireConnection;806;11;23;0
WireConnection;55;0;53;0
WireConnection;55;1;54;0
WireConnection;807;33;50;0
WireConnection;807;9;51;0
WireConnection;807;10;22;0
WireConnection;807;11;23;0
WireConnection;72;0;71;0
WireConnection;72;1;806;0
WireConnection;59;0;57;0
WireConnection;59;1;58;0
WireConnection;73;0;72;0
WireConnection;73;1;807;0
WireConnection;808;33;54;0
WireConnection;808;9;55;0
WireConnection;808;10;22;0
WireConnection;808;11;23;0
WireConnection;63;0;61;0
WireConnection;63;1;62;0
WireConnection;809;33;58;0
WireConnection;809;9;59;0
WireConnection;809;10;22;0
WireConnection;809;11;23;0
WireConnection;74;0;73;0
WireConnection;74;1;808;0
WireConnection;67;0;65;0
WireConnection;67;1;66;0
WireConnection;810;33;62;0
WireConnection;810;9;63;0
WireConnection;810;10;22;0
WireConnection;810;11;23;0
WireConnection;75;0;74;0
WireConnection;75;1;809;0
WireConnection;91;0;89;0
WireConnection;91;1;90;0
WireConnection;811;33;66;0
WireConnection;811;9;67;0
WireConnection;811;10;22;0
WireConnection;811;11;23;0
WireConnection;76;0;75;0
WireConnection;76;1;810;0
WireConnection;812;33;90;0
WireConnection;812;9;91;0
WireConnection;812;10;22;0
WireConnection;812;11;23;0
WireConnection;77;0;76;0
WireConnection;77;1;811;0
WireConnection;94;0;92;0
WireConnection;94;1;93;0
WireConnection;813;33;93;0
WireConnection;813;9;94;0
WireConnection;813;10;22;0
WireConnection;813;11;23;0
WireConnection;100;0;95;0
WireConnection;100;1;96;0
WireConnection;102;0;77;0
WireConnection;102;1;812;0
WireConnection;106;0;101;0
WireConnection;106;1;99;0
WireConnection;104;0;102;0
WireConnection;104;1;813;0
WireConnection;814;33;96;0
WireConnection;814;9;100;0
WireConnection;814;10;22;0
WireConnection;814;11;23;0
WireConnection;815;33;99;0
WireConnection;815;9;106;0
WireConnection;815;10;22;0
WireConnection;815;11;23;0
WireConnection;111;0;105;0
WireConnection;111;1;107;0
WireConnection;110;0;104;0
WireConnection;110;1;814;0
WireConnection;117;0;110;0
WireConnection;117;1;815;0
WireConnection;116;0;112;0
WireConnection;116;1;109;0
WireConnection;816;33;107;0
WireConnection;816;9;111;0
WireConnection;816;10;22;0
WireConnection;816;11;23;0
WireConnection;817;33;109;0
WireConnection;817;9;116;0
WireConnection;817;10;22;0
WireConnection;817;11;23;0
WireConnection;122;0;113;0
WireConnection;122;1;115;0
WireConnection;119;0;117;0
WireConnection;119;1;816;0
WireConnection;124;0;118;0
WireConnection;124;1;121;0
WireConnection;818;33;115;0
WireConnection;818;9;122;0
WireConnection;818;10;22;0
WireConnection;818;11;23;0
WireConnection;123;0;119;0
WireConnection;123;1;817;0
WireConnection;819;33;121;0
WireConnection;819;9;124;0
WireConnection;819;10;22;0
WireConnection;819;11;23;0
WireConnection;126;0;123;0
WireConnection;126;1;818;0
WireConnection;128;0;126;0
WireConnection;128;1;819;0
WireConnection;820;0;128;0
WireConnection;820;1;128;0
WireConnection;820;2;128;0
WireConnection;820;3;821;0
WireConnection;138;0;820;0
ASEEND*/
//CHKSM=BD3E491ABD2C77147F6C5E0A343D94D543945A74