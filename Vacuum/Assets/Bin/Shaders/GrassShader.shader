Shader "Custom/GrassShader" {
    Properties{
        _Color("Color", Color) = (1,1,1,1)
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Position("Position", Vector) = (0.0, 0.0, 0.0, 0.0)
        _FlowMap("Flow Map", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
        Cull Off

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 5.0

		sampler2D _MainTex;
        sampler2D _FlowMap;

		struct Input {
			float2 uv_MainTex   : TEXCOORD0;
            float3 uv_Flow;
            float3 worldPos     : POSITION;
            float4 color        : COLOR;
            UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
            UNITY_DEFINE_INSTANCED_PROP(float4, _Position)
		UNITY_INSTANCING_BUFFER_END(Props)

        float rand(float3 co)
        {
            return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
        }

        void vert(inout appdata_full v) {
            UNITY_SETUP_INSTANCE_ID(v);
            // Color foliage here!
            // Gradient towards ground
            v.color = _Color * lerp(.1, 1, v.vertex.y);
            // Per instance color variation based on position
            #ifdef UNITY_INSTANCING_ENABLED
                float4 instancePos = UNITY_ACCESS_INSTANCED_PROP(Props, _Position);
                float randVal = rand(instancePos.xyz);
                v.color *= lerp(.5, 1.3, randVal);
            #endif


            // TODO: Wiggle foliage here!
            #ifdef UNITY_INSTANCING_ENABLED
                //fit position into texture. Dependent on how big RT camera is, and where it is!
                instancePos /= 10;
                instancePos = instancePos + 1 * .5;

                // Sample flowmap
                float4 flowAtPivot = tex2Dlod(_FlowMap, float4(instancePos.x, instancePos.z, 1, 0));

                flowAtPivot = flowAtPivot * 2 - 1;

                flowAtPivot *= .3;

                //flowAtPivot.xyz = cross(flowAtPivot.xyz, float3(0, 1, 0));

                v.vertex.x += flowAtPivot.x * cos(_Time * 100) * v.vertex.y;
                v.vertex.z += flowAtPivot.y * cos(_Time * 100) * v.vertex.y;

            #endif

            //v.color = (float4)0;
            //v.color.w = 1;
            //v.color.xyz = fmod((float4)_Time, 1);
        }


		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			//fixed4 c = _Color;
			//o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			//o.Alpha = c.a;

            // Just color by vertex color
            o.Albedo = IN.color.xyz;
            o.Alpha = 1;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
