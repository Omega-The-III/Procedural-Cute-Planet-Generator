Shader "Custom/PlanetShader"
{
	//HEIGHT MAP BULLSHIT
	//https://catlikecoding.com/unity/tutorials/rendering/part-6/

    Properties
	{
		_SizeScale("size scale", Float) = 20
		_DisplacementScale("displacement scale", Float) = 1
		_MaskTex("Mask", 2D) = "white" {}
		_HeightMap("HeightMap", 2D) = "white" {}
		_GrassTex("Grasslands", 2D) = "white" {}
		_SnowTex("SnowField", 2D) = "white" {}
		_DesertTex("Desertwaste", 2D) = "white" {}
		_OceanTex("Ocean", 2D) = "white" {}
		_VolcanicTex("Volcanic", 2D) = "white" {}
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
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MaskTex;
			sampler2D _HeightMap;
			sampler2D _MainTex;
			sampler2D _GrassTex;
			sampler2D _SnowTex;
			sampler2D _DesertTex;
			sampler2D _OceanTex;
			sampler2D _VolcanicTex;
			float _SizeScale;
			float _DisplacementScale;

			float4 _MaskTex_ST;

			v2f vert(appdata v)
			{
				v2f o;

				float displacement = tex2Dlod(_HeightMap, float4(v.uv.xy, 0, 0)).r;
				v.vertex.xyz += v.normal * _SizeScale;
				displacement = displacement * _DisplacementScale;
				v.vertex.xyz += v.normal * displacement;

				o.vertex = UnityObjectToClipPos(v.vertex);

				o.uv = TRANSFORM_TEX(v.uv, _MaskTex);
				UNITY_TRANSFER_FOG(o, o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// sample the textures
				fixed4 mask = tex2D(_MaskTex, i.uv);
				fixed4 height = tex2D(_HeightMap, i.uv);
				fixed4 grass = tex2D(_GrassTex, i.uv);
				fixed4 snow = tex2D(_SnowTex, i.uv);
				fixed4 desert = tex2D(_DesertTex, i.uv);
				fixed4 ocean = tex2D(_OceanTex, i.uv);
				fixed4 volcanic = tex2D(_VolcanicTex, i.uv);

				// mask the different texture by the corrosponding rgb channels in the mask
				grass = grass * mask.g;
				snow = snow * mask.b;
				desert = desert * mask.r;
				volcanic = volcanic * mask.a;

				fixed4 results = grass + desert + snow;
				// combine all masked textures as output
				return results;
			}
			ENDCG
		}
	}
}