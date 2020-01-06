Shader "Portals/RecursivePortalMask0Pass"
{
	Properties
	{
		_SecondTex("Second Texture", 2D) = "white" {}
		_StencilRef("Stencil Reference", Float) = 1
		
		_StencilComp(" Stencil Comparison", Float) = 8
        _Stencil(" Stencil ID", Float) = 0
        _StencilOp(" Stencil Operation", Float) = 0
        _StencilWriteMask(" Stencil Write Mask", Float) = 255
        _StencilReadMask(" Stencil Read Mask", Float) = 255
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
		// Cull both
		Lighting Off
		ZWrite On

		Pass
		{
			// Blend SrcAlpha OneMinusSrcAlpha
			Stencil
			{
				Ref[_Stencil]
				Comp[_StencilComp]
				Pass[_StencilOp]
				ReadMask[_StencilReadMask]
				WriteMask[_StencilWriteMask]
			}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			

			#include "UnityCG.cginc"

			uniform sampler2D _SecondTex;

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
				fixed4 col = tex2D(_SecondTex, uv);
				// col.a = 1;
				return col;
			}
			ENDCG

		}
	}
}
