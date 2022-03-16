Shader "Custom/PlanetShader"
{
    Properties
	{
		_MaskTex("Mask", 2D) = "white" {}
		_GrassTex("Grasslands", 2D) = "white" {}
		_SnowTex("SnowField", 2D) = "white" {}
		_DesertTex("Desertwaste", 2D) = "white" {}
		_OceanTex("Ocean", 2D) = "white" {}
	}
		SubShader
	{
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MaskTex;
			sampler2D _GrassTex;
			sampler2D _SnowTex;
			sampler2D _DesertTex;
			sampler2D _OceanTex;

			float4 _MaskTex_ST;

			v2f vert(appdata v)
			{
				v2f o;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MaskTex);

				//Height displacement using perlin noise goes here
				// \/\/\/

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// sample the textures
				fixed4 mask = tex2D(_MaskTex, i.uv);
				fixed4 grass = tex2D(_GrassTex, i.uv);
				fixed4 snow = tex2D(_SnowTex, i.uv);
				fixed4 sand = tex2D(_DesertTex, i.uv);
				fixed4 ocean = tex2D(_OceanTex, i.uv);

				// mask the different texture by the corrosponding rgb channels in the mask
				grass = grass * mask.r;
				snow = snow * mask.g;
				sand = sand * mask.b;

				// combine all masked textures as output
				return grass + snow + sand;
			}
			ENDCG
		}
	}
}
