Shader "Portals/MainCamRecursivePass"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		[IntRange]_StencilRef("Stencil Reference", Range(0,255)) = 1
		
		// 0 disabled, 1 - Never, 2 - Less, 3 - equal,
		// 4- LessEqual
		// 5 - Greater , 6 - NotEqual, 7- GreaterEqual
		// 8 - always
		// [IntRange]
		[Enum(UnityEngine.Rendering.CompareFunction)]_StencilComp(" Stencil Comparison", Int) = 8
		[Header(Stencil Stuff)]
		// 0 keep, 1 - Zero, 2 - Replace, 
		// 3 - IncrementSaturate ,4- DecrementSaturate
		// 5 - Invert , 6 - IncrementWrap,
		// 7- DecrementWrap
		// [IntRange]
		[Enum(UnityEngine.Rendering.StencilOp)]_StencilOp(" Stencil Operation", Int) = 0
		[Enum(UnityEngine.Rendering.StencilOp)]_StencilOpFail(" Stencil Operation On Fail", Int) = 0
		[Enum(UnityEngine.Rendering.StencilOp)]_StencilOpZFail(" Stencil Operation On ZTestFail", Int) = 0

		// [IntRange]_StencilWriteMask(" Stencil Write Mask", Range(0,255)) = 255
		// [IntRange]_StencilReadMask(" Stencil Read Mask", Range(0,255)) = 255

		// [Enum(UnityEngine.Rendering.ColorWriteMask)] _ColorWriteMask("Color Write Mask", Float) = 15 // "All"
        // [Enum(UnityEngine.Rendering.CullMode)] _CullMode("Cull Mode", Float) = 2  
        // [Enum(DepthWrite)] _ZWrite("Depth Write", Float) = 1                                         // "On"
        //
        // 
        // [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Source Blend", Float) = 1                 // "One"
        // [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Destination Blend", Float) = 0  

	}
	SubShader
	{
		Tags 
		{ 
			"LightMode" = "ForwardBase"
			"RenderType" = "Opaque"  // Opaque  transparent tranparentcutout
			"Queue" = "Geometry+100"  // Geometry  
			"PreviewType" = "Plane"
			"DisableBatching" = "false"
			"ForceNoShadowCasting" = "true"
			"IgnoreProjector" = "true"
			"CanUseSpriteAtlas" = "false"
		}
		Lighting Off
		// ZWrite Off
		// Offset -1, -1
		ZTest Always Cull Off ZWrite Off
    	Fog { Mode off }


		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			Stencil
			{
				Ref[_StencilRef]
				Comp[_StencilComp]
				Pass[_StencilOp]
				// ReadMask[_StencilReadMask]
				// WriteMask[_StencilWriteMask]
				// Fail[_StencilOpFail] // do not change stencil value if stencil test fails
				// ZFail[_StencilOpZFail] // do not change stencil value if stencil test passes but depth test fails
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
            #pragma fragmentoption ARB_precision_hint_fastest

			// make fog work
            // #pragma multi_compile_fog  

			// Ligthing
            // #pragma multi_compile_fwdbase 

			// Skip variants
            #pragma skip_variants POINT 
            #pragma skip_variants SPOT 
            #pragma skip_variants POINT_COOKIE 
            #pragma skip_variants VERTEXLIGHT_ON
            #pragma skip_variants LIGHTPROBE_SH 
            #pragma skip_variants DIRECTIONAL 
            #pragma skip_variants SHADOWS_SCREEN
            #pragma skip_variants UNITY_HDR_ON 
            #pragma skip_variants SHADOWS_CUBE 
            #pragma skip_variants SHADOWS_DEPTH 
            #pragma skip_variants FOG_LINEAR 
            #pragma skip_variants FOG_EXP 
            #pragma skip_variants FOG_EXP2 
            #pragma skip_variants DYNAMICLIGHTMAP_ON 
            #pragma skip_variants DIRECTIONAL_COOKIE
            #pragma skip_variants DIRLIGHTMAP_COMBINED
            #pragma skip_variants SHADOWS_SHADOWMASK 
            #pragma skip_variants LIGHTMAP_SHADOW_MIXING
            #pragma skip_variants SHADOWS_SOFT


			// fast if statement
            // #define IF(a, b, c) lerp(b, c, step((fixed) (a), 0.0)); 

			#include "UnityCG.cginc"
            // #include "AutoLight.cginc"


			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 screenPos : TEXCOORD0;
				// UNITY_FOG_COORDS(1)

			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
                // UNITY_TRANSFER_FOG(output,output.pos);

				return o;
			}

			uniform sampler2D _MainTex;

			fixed4 frag(v2f i) : SV_Target
			{
				float2 uv = i.screenPos.xy / i.screenPos.w;
				fixed4 col = tex2D(_MainTex, uv);

				// [unroll]
				// for (int proximityLightIndex = 0; proximityLightIndex < 4; ++proximityLightIndex)
				// {
				// 	int dataIndex = proximityLightIndex * PROXIMITY_LIGHT_DATA_SIZE;
				// 	fadeDistance = min(fadeDistance, NearLightDistance(_ProximityLightData[dataIndex], o.worldPosition));
				// }
                // UNITY_APPLY_FOG(i.fogCoord, col);

				return col;
			}
			ENDCG
		}
	}
}
