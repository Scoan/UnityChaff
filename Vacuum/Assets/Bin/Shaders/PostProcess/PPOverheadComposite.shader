Shader "Hidden/PPOverheadComposite"
{
	Properties
	{
		_MainTex("Stamp alpha", 2D) = "white" {}
		_OutTexture("Current Normal", 2D) = "white" {}
		_TgtValue("Incoming Normal", Color) = (0.5, 0.5, 1.0, 1.0)
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
			float3 _TgtValue;


			half4 encode_normals(half3 nrm)
			{
				return half4(nrm.xy-.5 * .5, 0.0, 0.0);
			}


			half3 decode_normals(half2 enc)
			{
				// Decodes incoming (0...1) XY normals to (-1...1) range, reconstructs Z.
				half3 n;
				n.xy = enc * 2 - 1;
				n.z = sqrt(1 - dot(n.xy, n.xy));
				return n;
			}



			fixed4 frag(v2f_img i) : SV_Target
			{
				// Blend between floor and cam textures
				half4 cur_color = tex2D(_OutTexture, i.uv);
				half4 mask_color = tex2D(_MainTex, i.uv);

				half4 tgt_color = half4(_TgtValue.xyz, 1.0f);		// TODO: Handle multiple tgt normals? Separate passes, or encode diff masks in mask rgb?

				// Bend current normal towards incoming normal, using the stamp as alpha
				half4 out_color = lerp(cur_color, tgt_color, mask_color.r);

				// Normalize results to ensure we're creating a valid normal map!
				//half3 normalized_color = normalize(half3(out_color.x-.5, out_color.y-.5, out_color.z));

				out_color = half4(out_color.xyz, 1.0f);

				return out_color;
			}
			ENDCG
		}
	}
}
