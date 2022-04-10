using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] GameObject planetObject;
    [SerializeField] MeshRenderer planetRenderer;

    [SerializeField] private int textureSize;

    [SerializeField] private bool UseTextures;
    [SerializeField] private List<Texture> BiomeTextures = new List<Texture>();

    [HideInInspector] public float mountainThreshold;
    [HideInInspector] public float waterThreshold;
    [HideInInspector] public float heightNoiseScale;

    [HideInInspector] public float blendNoiseScale;
    [HideInInspector] public float grassThreshold;
    [HideInInspector] public float desertThreshold;
    [HideInInspector] public float snowThreshold;
    [HideInInspector] public float volcanicThreshold;
    private void Start()
    {
        if (!Directory.Exists(Application.dataPath + "/generatedTextures"))
        {
            Directory.CreateDirectory(Application.dataPath + "/generatedTextures");
        }

        if (UseTextures)
        {
            planetRenderer.material.SetTexture("_GrassTex", BiomeTextures[0]);
            planetRenderer.material.SetTexture("_DesertTex", BiomeTextures[1]);
            planetRenderer.material.SetTexture("_SnowTex", BiomeTextures[2]);
            planetRenderer.material.SetTexture("_VolcanicTex", BiomeTextures[3]);
        } else {
            planetRenderer.material.SetTexture("_GrassTex", BiomeTextures[4]);
            planetRenderer.material.SetTexture("_DesertTex", BiomeTextures[5]);
            planetRenderer.material.SetTexture("_SnowTex", BiomeTextures[6]);
            planetRenderer.material.SetTexture("_VolcanicTex", BiomeTextures[7]);
        }
    }
    public void GenerateBiomeBlendmap(List<Biomes> biomePriorityList)
    {
        Texture2D blendmapTexOne = new Texture2D(textureSize, textureSize, TextureFormat.RGB48, true);
        Texture2D blendmapTexTwo = new Texture2D(textureSize, textureSize, TextureFormat.RGB48, true);

        //Base layer for maskOne is green
        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                blendmapTexOne.SetPixel(x, y, Color.green);
            }
        }

        //Base layer for maskOne is black
        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                blendmapTexTwo.SetPixel(x, y, Color.black);
            }
        }

        //For all listed biomes, add biomes.
        for (int i = 0; i < biomePriorityList.Count; i++) {
            int xOffset = Random.Range(-10000, 10000);
            int yOffset = Random.Range(-10000, 10000);
            float[,] NoiseMap = new float[textureSize, textureSize];

            for (int y = 0; y < textureSize; y++)
            {
                for (int x = 0; x < textureSize; x++)
                {
                    float noiseValue = Mathf.PerlinNoise(x * blendNoiseScale + xOffset, y * blendNoiseScale + yOffset);
                    NoiseMap[x, y] = noiseValue;

                    switch (biomePriorityList[i])
                    {
                        case Biomes.grass:
                            if (NoiseMap[x, y] < grassThreshold)
                                blendmapTexOne.SetPixel(x, y, Color.green);
                            break;
                        case Biomes.desert:
                            if (NoiseMap[x, y] < desertThreshold)
                                blendmapTexOne.SetPixel(x, y, Color.red);
                            break;
                        case Biomes.snow:
                            if (NoiseMap[x, y] < snowThreshold)
                                blendmapTexOne.SetPixel(x, y, Color.blue);
                            break;
                            //Start MaskTexTwo biomes
                        case Biomes.volcanic:
                            if (NoiseMap[x, y] < volcanicThreshold)
                                blendmapTexTwo.SetPixel(x, y, Color.red);
                            break;
                    }
                }
            }
        }

        string path;

        blendmapTexOne.Apply();
        path = Application.dataPath + "/generatedTextures/texturemapOne.png";
        File.WriteAllBytes(path, blendmapTexOne.EncodeToPNG());
        planetRenderer.material.SetTexture("_MaskTexOne", blendmapTexOne);

        blendmapTexTwo.Apply();
        path = Application.dataPath + "/generatedTextures/texturemapTwo.png";
        File.WriteAllBytes(path, blendmapTexTwo.EncodeToPNG());
        planetRenderer.material.SetTexture("_MaskTexTwo", blendmapTexTwo);
    }
    public void GenerateHeightMap()
    {
        Texture2D noiseTexture = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, true);
        float[,] noiseMap = new float[textureSize, textureSize];

        int xOffset = Random.Range(-10000, 10000);
        int yOffset = Random.Range(-10000, 10000);
        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                float noiseValue = Mathf.PerlinNoise(x * heightNoiseScale + xOffset, y * heightNoiseScale + yOffset);
                noiseMap[x, y] = noiseValue;

                if (noiseMap[x, y] < waterThreshold)
                    noiseTexture.SetPixel(x, y, Color.black);
                else if (noiseMap[x, y] > mountainThreshold)
                    noiseTexture.SetPixel(x, y, Color.white);
                else
                    noiseTexture.SetPixel(x, y, Color.gray);
            }
        }
        for (int y = 0; y < textureSize; y += textureSize - 1)
        {
            for (int x = 0; x < textureSize; x++)
            {
                noiseTexture.SetPixel(x, y, Color.gray);
            }
        }

        noiseTexture.Apply();
        string path = Application.dataPath + "/generatedTextures/heightmap.png";
        File.WriteAllBytes(path, noiseTexture.EncodeToPNG());
        planetRenderer.material.SetTexture("_HeightMap", noiseTexture);
    }
}
