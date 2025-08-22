Shader "Hidden/MirrorView"
{
    Properties { }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            int _Horizontal;

            fixed4 frag(v2f_img i) : SV_Target
            {
                float2 uv = i.uv;
                if (_Horizontal == 1)
                    uv.x = 1 - uv.x;
                else
                    uv.y = 1 - uv.y;

                return tex2D(_MainTex, uv);
            }
            ENDCG
        }
    }
}
