Shader "Custom/OverlayShader"
{
    Properties
    {
        _OverlayTex("Overlay Texture", 2D) = "white" {}
        _Scale("Scale", Float) = 0.006944444
        _BlendAmount("Blend Amount", Range(0.0, 1.0)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
        
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _OverlayTex;
            float _Scale; // should be pixels per unit divided by texture size for pixel perfect look (64 pixels per unit here)
            float _BlendAmount;
            bool _Solid;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color : COLOR; // Tilemap color
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 worldPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                // Calculate the world position (in 2D space, so xy)
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldPos = worldPos.xy;
                
                o.uv = v.texcoord;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Sample the main texture
                fixed4 color = tex2D(_MainTex, i.uv);
                color = color * i.color; // Applies the Tilemap's color

                // Sample the overlay texture using the scaled world position
                float2 overlayUV = i.worldPos * _Scale;
                fixed4 overlayColor = tex2D(_OverlayTex, overlayUV);
                if (color.a == 0.0) { // Ignores anything outside the texture
                    return color;
                }

                return lerp(color, clamp(color + overlayColor, 0.0, 1.0), _BlendAmount);
            }
            ENDCG
        }
    }
}
