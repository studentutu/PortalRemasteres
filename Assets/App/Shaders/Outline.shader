Shader "Portals/Outline"
{
    Properties
    {
		_OutlineColour("Outline Colour", Color) = (1, 1, 1, 1)
		_StencilRef(" Stencil Reference", Int) = 0

    }
    SubShader
    {
        Tags 
		{ 
			"RenderType" = "transparent"  // Opaque  transparent tranparentcutout
			"Queue" = "Geometry+2"  // Geometry  
			"PreviewType" = "Plane"
            "DisableBatching" = "false"
            "ForceNoShadowCasting" = "false"
            "IgnoreProjector" = "false"
            "CanUseSpriteAtlas" = "false"
		}
        
		Stencil
		{
			Ref [_StencilRef]
			Comp Equal  // GEqual  LEqual
		}
        Lighting Off
		ZWrite Off
		Offset -1, -1


        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile_instancing


            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

			uniform float4 _OutlineColour;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _OutlineColour;
            }
            ENDCG
        }
    }
}
