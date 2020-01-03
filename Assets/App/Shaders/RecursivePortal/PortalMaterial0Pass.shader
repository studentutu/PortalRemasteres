Shader "Portals/RecursivePortalMask0Pass"
{
	Properties
	{
		// _MainTex("Main Texture", 2D) = "white" {}
		_StencilRef(" Stencil Reference", Float) = 1

		// _StencilWrite("Stencil Write mask", Float) = 119
		// _StencilRead("Stencil Read Mask", Float) = 119
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

		Pass
		{
			// Blend SrcAlpha OneMinusSrcAlpha
			Stencil
			{
				// ReadMask [_StencilRead]
				// WriteMask [_StencilWrite]
				Ref [_StencilRef]

				Comp Always // GEqual Always
				Pass Replace
			}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			

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
				o.screenPos.w = 1/ o.screenPos.w;
				return o;
			}

			uniform sampler2D _MainTex;

			fixed4 frag(v2f i) : SV_Target
			{
				float2 uv = i.screenPos.xy * i.screenPos.w;
				fixed4 col = tex2D(_MainTex, uv);
				// col.a = 1;
				return col;
			}
			ENDCG

		}
	}
}
