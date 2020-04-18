Shader "Fire"
{
	Properties
	{
		_NoiseTex("Noise Texture", 2D) = "white" {}
		_GradientTex("Gradient Texture", 2D) = "white" {}

		_BrighterCol("Brighter Color", Color) = (1,1,1,1)
		_MiddleCol("Middle Color", Color) = (.7,.7,.7,1)
		_DarkerCol("Darker Color", Color) = (.4,.4,.4,1)
		_ScrollSpeed("Scroll Speed", Range(1, 5)) = 2
	}

		SubShader
		{
			//The shader is transparent
			Tags
			{
				 "Queue" = "Transparent" "RenderType" = "Transparent"
			}

			Blend SrcAlpha OneMinusSrcAlpha

			Pass
			{

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				#include "UnityShaderVariables.cginc" //to use _Time


				sampler2D _NoiseTex;
				sampler2D _GradientTex;

				float4 _BrighterCol;
				float4 _MiddleCol;
				float4 _DarkerCol;
				float _ScrollSpeed;

				//Input for the vertex
				struct appdata {
					float4 vertex : POSITION;
					float4 texcoord : TEXCOORD0;
				};

				//Output for the fragment
				struct v2f {
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
				};

				v2f vert(appdata v) {
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = v.texcoord.xy;

					return o;
				}

				float4 frag(v2f IN) : SV_Target {

					float noiseValue = tex2D(_NoiseTex, IN.uv - float2(0, _Time.x * _ScrollSpeed)).x; //fire with scrolling
					float gradientValue = tex2D(_GradientTex, IN.uv).x;

					float step1 = step(noiseValue, gradientValue);
					float step2 = step(noiseValue, gradientValue - 0.2);
					float step3 = step(noiseValue, gradientValue - 0.4);

					//The entire fire color
					float4 c = float4
						(
							//Calculates where to place the darker color instead of the brighter one
							lerp
							(
								_BrighterCol.rgb,
								_DarkerCol.rgb,
								step1 - step2 //Corresponds to "L1" in my GIF
							),

						step1 //This is the alpha of our fire, which is the "outer" color, i.e. the step1
						);

					c.rgb = lerp //Calculates where to place the middle color
						(
							c.rgb,
							_MiddleCol.rgb,
							step2 - step3 //Corresponds to "L2" in my GIF
						);

					return c;
				}
				ENDCG
			}

		}
}
