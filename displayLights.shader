Shader "DisplayLightsShader"
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
            
            float aspectRatio; 
            
            int debugMode; 

            fixed4 frag (v2f i) : SV_Target
            {
                float pixelSize = .005; 
               
                fixed4 col = tex2D(_MainTex, i.uv);
                
                 if(debugMode == 1)
                 {
                    if((i.uv.x < (xValue +.001)) && (i.uv.x > (xValue -.001)))
                        return fixed4(1, 0, 0, 1);
                        
                    if((i.uv.y < (yValue +.001)) && (i.uv.y > (yValue -.001)))
                        return fixed4(1, 0, 0, 1);
                 }
                 else
                 {
                    if((i.uv.x < (xValue +(pixelSize/aspectRatio))) && (i.uv.x > (xValue -(pixelSize/aspectRatio))) && (i.uv.y < (yValue +pixelSize)) && (i.uv.y > (yValue -pixelSize))
                        || (i.uv.x < (xValue +(pixelSize/1.5/aspectRatio))) && (i.uv.x > (xValue -(pixelSize/1.5/aspectRatio))) && (i.uv.y < (yValue +1.5*pixelSize)) && (i.uv.y > (yValue - 1.5*pixelSize))
                        || (i.uv.x < (xValue +(1.5*pixelSize/aspectRatio))) && (i.uv.x > (xValue -(1.5*pixelSize/aspectRatio))) && (i.uv.y < (yValue +pixelSize/1.5)) && (i.uv.y > (yValue - pixelSize/1.5)))
                            return fixed4(1, 1, 1, 1);
                        
                    if((i.uv.x < (.5 + .05/aspectRatio)) && (i.uv.x > (.5 - .05/aspectRatio)) && (i.uv.y < (.5 + .005)) && (i.uv.y > (.5 - .005))
                        || (i.uv.x < (.5 + .005/aspectRatio)) && (i.uv.x > (.5 - .005/aspectRatio)) && (i.uv.y < (.5 + .05)) && (i.uv.y > (.5 - .05)))
                            return fixed4(0, 0, 0, 1);
                 }
                 
                return col;
            }
            
            ENDCG
        }
    }
}
