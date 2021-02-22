Shader "Hidden/Glitch"
{
    Properties
    {
        //_MainTex ("Texture", 2D) = "white" {}
		_Strength("Strength", Range(0, 1)) = 1
		_FrequencyX("Frequency X", Range(0, 10)) = 1
		_FrequencyY("Frequency Y", Range(0, 10)) = 1
		_Color("Displacement", Vector) = (0, 0, 0, 0)
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

			//Initialize variables here (same ones as in "Properties" above).
			//A texture can contain a lot of data, not just color values for an image.
			//Types: half, float, fixed: No difference except for how it's stored.
			sampler2D _MainTex;
			float _Strength;
			float _FrequencyX;
			float _FrequencyY;

			//Defines which data we get from each vertex on the screen
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

			//Defines what information we are passing to the frag function
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

			//Looks at the position of each vertex. O will be sent to frag.
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

			//Returns a color as a float4. This is the final output.
            float4 frag (v2f i) : SV_Target
            {
				i.uv.x += sin(i.uv.x * _FrequencyX * _Time.y) * _Strength;
				i.uv.y += sin(i.uv.y * _FrequencyY * _Time.y) * _Strength;

				float4 result = tex2D(_MainTex, i.uv);
				return result;
            }
            ENDCG
        }
    }
}
