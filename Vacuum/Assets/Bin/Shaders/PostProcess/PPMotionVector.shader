Shader "PostProcess/PPMotionVector"
{
	Properties
	{
        // To be set by the object during movement.
        _MotionVector("Motion Vector", Vector) = (0.0, 0.0, 1.0, 1.0)
	}
	SubShader
	{

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

            struct vertexOut
            {
                float4 pos      : SV_POSITION;
                float3 normal   : NORMAL;
                half2  uv       : TEXCOORD0;
                float4 color    : COLOR0;
            };

            float4 _MotionVector;

            vertexOut vert(appdata_base v)
            {
                vertexOut o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = _MotionVector;
                // Motion vector passed in as (-1...1), rerange to (0...1)
                o.color.x = o.color.x + 1 * .5f;
                o.color.y = o.color.y + 1 * .5f;
                o.normal = UnityObjectToWorldNormal(v.normal);
                
                return o;
            }

			fixed4 frag(vertexOut IN) : SV_Target
			{
                // Fresnel Z out
                float normDot = dot(IN.normal, float3(0,1,0));
                normDot = saturate(normDot*normDot);

                float4 out_color = IN.color;
                // If we're standing still, reduce our mask (the z component)
                out_color.z = normDot * saturate(length(_MotionVector.xy));
				return out_color;

			}
			ENDCG
		}
	}
}
