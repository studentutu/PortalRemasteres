Shader "Portals/RecursivePortalMask"
{
    Properties
    {
		_MainTex("Main Texture", 2D) = "white" {}
		[IntRange]_StencilRef("Stencil Reference", Range(0,255)) = 1
		
		// 0 disabled, 1 - Never, 2 - Less, 3 - equal,
		// 4- LessEqual
		// 5 - Greater , 6 - NotEqual, 7- GreaterEqual
		// 8 - always
		[IntRange]_StencilComp(" Stencil Comparison", Range(0,8)) = 8
		// 0 keep, 1 - Zero, 2 - Replace, 
		// 3 - IncrementSaturate ,4- DecrementSaturate
		// 5 - Invert , 6 - IncrementWrap,
		// 7- DecrementWrap
        [IntRange]_StencilOp(" Stencil Operation", Range(0,7)) = 0
        [IntRange]_StencilOpFail(" Stencil Operation On Fail", Range(0,7)) = 0
        [IntRange]_StencilOpZFail(" Stencil Operation On ZTestFail", Range(0,7)) = 0

        // [IntRange]_StencilWriteMask(" Stencil Write Mask", Range(0,255)) = 255
        // [IntRange]_StencilReadMask(" Stencil Read Mask", Range(0,255)) = 255

    }
    SubShader
    {
		Tags 
		{ 
			"RenderType" = "Opaque"  // Opaque  transparent tranparentcutout
			"Queue" = "Geometry+1"  // Geometry  
			"PreviewType" = "Plane"
            "DisableBatching" = "false"
            "ForceNoShadowCasting" = "true"
            "IgnoreProjector" = "true"
            "CanUseSpriteAtlas" = "false"
		}
		Lighting Off
		ZWrite On
		Offset -1, -1

        Pass
        {
			// Blend SrcAlpha OneMinusSrcAlpha
			Stencil
			{
				Ref[_StencilRef]
				Comp[_StencilComp]
				Pass[_StencilOp]
				// ReadMask[_StencilReadMask]
				// WriteMask[_StencilWriteMask]
				Fail[_StencilOpFail] // do not change stencil value if stencil test fails
            	ZFail[_StencilOpZFail] // do not change stencil value if stencil test passes but depth test fails
			}

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_instancing
            	#pragma fragmentoption ARB_precision_hint_fastest


				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					float4 screenPos : TEXCOORD0;
				};

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.screenPos = ComputeScreenPos(o.vertex);
					return o;
				}

				uniform sampler2D _MainTex;

				fixed4 frag(v2f i) : SV_Target
				{
					float2 uv = i.screenPos.xy / i.screenPos.w;
					fixed4 col = tex2D(_MainTex, uv);
					return col;
				}
			ENDCG
        }
    }
}
