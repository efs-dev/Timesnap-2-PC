Shader "Unlit/TextureColorAlpha"
{
	Properties
	{
		_Color("Color Tint", Color) = (1,1,1,1)
		_MainTex("Base (RGB) Alpha (A)", 2D) = "white"
	}

		Category
	{
		Lighting Off
		ZWrite Off
		//ZWrite On  // uncomment if you have problems like the sprite disappear in some rotations.
		Cull back
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaTest Greater 0.001  // uncomment if you have problems like the sprites or 3d text have white quads instead of alpha pixels.
		Tags{ Queue = Transparent }

        SubShader {Pass {
            GLSLPROGRAM
            varying mediump vec2 uv;
           
            #ifdef VERTEX
            uniform mediump vec4 _MainTex_ST;
            void main() {
                gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
                uv = gl_MultiTexCoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
            }
            #endif
           
            #ifdef FRAGMENT
            uniform lowp sampler2D _MainTex;
            uniform lowp vec4 _Color;
            void main() {
                gl_FragColor = texture2D(_MainTex, uv) * _Color;
            }
            #endif     
            ENDGLSL
        }}
       
        SubShader {Pass {
            SetTexture[_MainTex] {Combine texture * constant ConstantColor[_Color]}
        }}
    }
}