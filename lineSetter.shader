Shader "LineSettingShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
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
            float4 _MainTex_TexelSize;

            float xValue;
            float yValue;

            

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                if((i.uv.x < (.5 + xValue +.001)) && (i.uv.x > (.5 + xValue -.001)))
                    return fixed4(1, 0, 0, 1);
                    
                if((i.uv.x < (.5 - xValue +.001)) && (i.uv.x > (.5 - xValue -.001)))
                    return fixed4(1, 0, 0, 1);
                    
                if((i.uv.y < (.5 + yValue +.001)) && (i.uv.y > (.5 + yValue -.001)))
                    return fixed4(1, 0, 0, 1);
                    
                if((i.uv.y < (.5 - yValue +.001)) && (i.uv.y > (.5 - yValue -.001)))
                    return fixed4(1, 0, 0, 1);
                
                return col;
            }
            
            ENDCG
        }
    }
}
