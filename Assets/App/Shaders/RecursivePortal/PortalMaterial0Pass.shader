Shader "Portals/RecursivePortalMask0Pass"
{
	Properties
	{
		_SecondTex("Second Texture", 2D) = "white" {}
		_Color (" Color To Use", Color) = (1,1,1,1)
		[IntRange]_StencilRef("Stencil Reference", Range(0,255)) = 1
		
		[IntRange]_StencilComp(" Stencil Comparison", Range(0,8)) = 8
        // 0 keep, 1 - Zero, 2 - Replace, 
		// 3 - IncrementSaturate ,4- DecrementSaturate
		// 5 - Invert , 6 - IncrementWrap, 
		// 7- DecrementWrap
        
		[IntRange]_StencilOp(" Stencil Operation Main", Range(0,7)) = 0
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
			"Queue" = "Geometry+10"  // Geometry  
			"PreviewType" = "Plane"
			"DisableBatching" = "false"
			"ForceNoShadowCasting" = "true"
			"IgnoreProjector" = "true"
			"CanUseSpriteAtlas" = "false"
		}
		// Cull both
		Lighting Off
		ZWrite Off
		Offset -1, -1


		Pass
		{
			// Blend SrcAlpha OneMinusSrcAlpha
			Blend Zero One

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
			

			#include "UnityCG.cginc"

			uniform sampler2D _SecondTex;
			uniform fixed4 _Color;

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

			fixed4 frag(v2f i) : SV_Target
			{
				float2 uv = i.screenPos.xy / i.screenPos.w;
				fixed4 col = 
				_Color;
				// tex2D(_SecondTex, uv);
				// col.a = 1;
				return col;
			}
			ENDCG

		}
	}
}
