using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] GameObject planetObject;
    private MeshRenderer planetRenderer;

    [SerializeField] private int textureSize;

    [Header("Heightmap variables")]
    [SerializeField] private float mountainThreshold;
    [SerializeField] private float waterThreshold;
    [SerializeField] private float heightNoiseScale;

    [Header("Heightmap variables")]
    [SerializeField] private int amountOfExtraBiomes = 3;
    [SerializeField] private float blendNoiseScale;
    [SerializeField] private float redThresholdMin;
    [SerializeField] private float blueThresholdMin;
    [SerializeField] private float purpleThresholdMin;
    private float extraGreenThreshold = 0.2f;
    enum MapType { heightMap, blendMap };

    [SerializeField] List<MapType> Maps = new List<MapType>();
    
    void Start()
    {
        planetRenderer = planetObject.GetComponent<MeshRenderer>();

        MapType currentMap;
        for (int i = 0; i < Maps.Count; i++)
        {
            currentMap = Maps[i];
            switch (currentMap)
            {
                case MapType.heightMap:
                    GenerateHeightMap();
                    break;
                case MapType.blendMap:
                    GenerateBlendmap();
                    break;
            }
        }
    }
    private void GenerateBlendmap()
    {
        Texture2D blendmapTexture = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, true);

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                blendmapTexture.SetPixel(x, y, Color.green);
            }
        }

        for (int i = 0; i < amountOfExtraBiomes + 1; i++) {
            int xOffset = Random.Range(-10000, 10000);
            int yOffset = Random.Range(-10000, 10000);
            float[,] NoiseMap = new float[textureSize, textureSize];

            for (int y = 0; y < textureSize; y++)
            {
                for (int x = 0; x < textureSize; x++)
                {
                    float noiseValue = Mathf.PerlinNoise(x * blendNoiseScale + xOffset, y * blendNoiseScale + yOffset);
                    NoiseMap[x, y] = noiseValue;

                    switch (i)
                    {
                        case 0:
                            if (NoiseMap[x, y] < redThresholdMin)
                                blendmapTexture.SetPixel(x, y, Color.red);
                            break;
                        case 1:
                            if (NoiseMap[x, y] < blueThresholdMin)
                                blendmapTexture.SetPixel(x, y, Color.blue);
                            break;
                        case 2:
                            if (NoiseMap[x, y] < purpleThresholdMin)
                                blendmapTexture.SetPixel(x, y, Color.black);
                            break;
                        case 3:
                            if (NoiseMap[x, y] < extraGreenThreshold)
                                blendmapTexture.SetPixel(x, y, Color.green);
                            break;
                    }
                    
                }
            }
        }

        blendmapTexture.Apply();

        string path = "./Assets/generatedtextures/texturemap.png";
        File.WriteAllBytes(path, blendmapTexture.EncodeToPNG());
        planetRenderer.material.SetTexture("_MaskTex", blendmapTexture);
        Debug.Log("Generated texture blendmap!");
    }
    private void GenerateHeightMap()
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
        noiseTexture.Apply();

        string path = "./Assets/generatedtextures/Noisemap.png";
        File.WriteAllBytes(path, noiseTexture.EncodeToPNG());
        planetRenderer.material.SetTexture("_HeightMap", noiseTexture);
        Debug.Log("Generated heightmap!");
    }
}
