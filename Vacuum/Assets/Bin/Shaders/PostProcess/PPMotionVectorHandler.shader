Shader "Hidden/PPMotionVectorHandler"
{
	Properties
	{
        // To be set by the object during movement.
        _MainTex("Flow Contributors", 2D) = "white" {}
        _OutTexture("Flow Map", 2D) = "white" {}
        _DeltaTime("Delta Time", Float) = 0
	}
	SubShader
	{

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _OutTexture;
            float _DeltaTime;

            half4 MoveTowards(half4 src, half4 dst, float step)
            {
                /// Blends a color towards another with a fixed step size.
                float difference = length(dst - src);
                // If we're there (or nearly there), return dst
                if (difference <= step)
                {
                    return dst;
                }
                // Otherwise, blend towards dst by step
                return lerp(src, dst, step / difference);
            }


			fixed4 frag(v2f_img IN) : SV_Target
			{
                half4 NEUTRAL_COLOR = half4(.5, .5, 0.0, 1.0);

                half4 cur_flow = tex2D(_OutTexture, IN.uv);
                half4 in_flow = tex2D(_MainTex, IN.uv);

                //TODO: If the incoming flow is less in magnitude than the current flow, we shouldn't lerp to it. We should stick to current flow.
                //      (slow-moving players shouldn't *slow down* super wavy grass as they walk through it)
                //      (But if a player goes NE through a SW flow, we shouldn't keep the flow SW... we should combine them somehow?)
                half4 out_flow = lerp(cur_flow, in_flow, in_flow.z);
                out_flow.z = 0;

                // Lerp towards neutral flow
                out_flow = MoveTowards(out_flow, NEUTRAL_COLOR, .3 * _DeltaTime);

                return out_flow;
                //return float4(1.0,0.5,1.0,1.0);
			}
			ENDCG
		}
	}
}
