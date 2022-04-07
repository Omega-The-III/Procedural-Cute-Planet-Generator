Shader "Custom/PlanetShader"
{
	//HEIGHT MAP BULLSHIT
	//https://catlikecoding.com/unity/tutorials/rendering/part-6/

    Properties
	{
		_SizeScale("size scale", Float) = 20
		_DisplacementScale("displacement scale", Float) = 1

		_MaskTexOne("MaskOne", 2D) = "white" {}
		_MaskTexTwo("MaskTwo", 2D) = "white" {}

		_HeightMap("HeightMap", 2D) = "white" {}
		_OceanTex("_OceanTex", 2D) = "white" {}

		_GrassTex("_GrassTex", 2D) = "white" {}
		_DesertTex("_DesertTex", 2D) = "white" {}
		_SnowTex("_SnowTex", 2D) = "white" {}
		_VolcanicTex("_VolcanicTex", 2D) = "white" {}
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
				float4 color : COLOR;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MaskTexOne;
			sampler2D _MaskTexTwo;

			//Heightnoise map stuff
			sampler2D _HeightMap;
			sampler2D _OceanTex;

			//Mask one textures
			sampler2D _GrassTex;
			sampler2D _DesertTex;
			sampler2D _SnowTex;
			sampler2D _VolcanicTex;

			float _SizeScale;
			float _DisplacementScale;

			float4 _MaskTexOne_ST;

			v2f vert(appdata v)
			{
				v2f o;

				float displacement = tex2Dlod(_HeightMap, float4(v.uv.xy, 0, 0)).r;
				v.vertex.xyz += v.normal * _SizeScale;
				displacement = displacement * _DisplacementScale;
				v.vertex.xyz += v.normal * displacement;

				o.vertex = UnityObjectToClipPos(v.vertex);

				o.uv = TRANSFORM_TEX(v.uv, _MaskTexOne);
				UNITY_TRANSFER_FOG(o, o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				// sample the textures
				fixed4 maskOne = tex2D(_MaskTexOne, i.uv);
				fixed4 maskTwo = tex2D(_MaskTexTwo, i.uv);
				fixed4 maskCombination = tex2D(_MaskTexOne, i.uv);

				fixed4 height = tex2D(_HeightMap, i.uv);
				fixed4 heightocean = tex2D(_HeightMap, i.uv);
				fixed4 heightmountain = tex2D(_HeightMap, i.uv);
				fixed4 ocean = tex2D(_OceanTex, i.uv);

				fixed4 grass = tex2D(_GrassTex, i.uv);
				fixed4 desert = tex2D(_DesertTex, i.uv);
				fixed4 snow = tex2D(_SnowTex, i.uv);
				fixed4 volcanic = tex2D(_VolcanicTex, i.uv);
			
				//height mountain
				heightmountain = 1 - heightmountain;
				float weight = saturate(dot(heightmountain.rgb, float3(1, 1, 1)));
				heightmountain.rgb = lerp(heightmountain.rgb, float3(1, 1, 1), weight);
				heightmountain = 1 - heightmountain;
				
				//ocean depths
				weight = saturate(dot(heightocean.rgb, float3(1, 1, 1)));
				heightocean.rgb = lerp(heightocean.rgb, float3(1, 1, 1), weight);
				heightocean = 1 - heightocean;
				ocean *= heightocean.rgba;
				
				//Mask subtractions to prevent layering textures
				maskOne -= maskTwo.r;
				maskOne -= heightocean;
				maskOne = clamp(maskOne, 0, 1);
				
				maskTwo -= heightocean;
				maskTwo = clamp(maskTwo, 0, 1);
				
				//Make mountains darker
				maskOne += heightmountain * -0.1;

				// mask the different texture by the corrosponding rgb channels in the mask
				grass = grass * maskOne.g;
				snow = snow * maskOne.b;
				desert = desert * maskOne.r;
				volcanic = volcanic * maskTwo.r;
				
				// combine all masked textures as output
				fixed4 results;
				results = (grass + desert + snow + volcanic + ocean);
				return results;
			}
			ENDCG
		}
	}
}