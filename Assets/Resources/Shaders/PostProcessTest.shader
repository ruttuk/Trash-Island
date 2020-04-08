Shader "Hidden/PostProcessTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_DisplacementTex("Displacement Texture", 2D) = "white" {}
		_Magnitude("Magnitude", Range(0, 0.1)) = 1
		_PixelX("Pixels X", Range(128, 512)) = 256
		_PixelY("Pixels Y", Range(128, 512)) = 256
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

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
			sampler2D _DisplacementTex;
			float _Magnitude;
			float _PixelX;
			float _PixelY;

            fixed4 frag (v2f i) : SV_Target
            {
				float2 uv = i.uv;
				uv.x *= _PixelX;
				uv.y *= _PixelY;
				uv.x = round(uv.x);
				uv.y = round(uv.y);
				uv.x /= _PixelX;
				uv.y /= _PixelY;

				fixed4 col = tex2D(_MainTex, uv);
                return col;
            }
            ENDCG
        }
    }
}
