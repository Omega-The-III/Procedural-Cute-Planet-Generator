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
    [SerializeField] private float blendNoiseScale;
    [SerializeField] private float redThresholdMin;
    [SerializeField] private float blueThresholdMax;
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
        float[,] redNoiseMap = new float[textureSize, textureSize];
        float[,] blueNoiseMap = new float[textureSize, textureSize];
        int xOffset = Random.Range(-10000, 10000);
        int yOffset = Random.Range(-10000, 10000);

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                float redNoiseValue = Mathf.PerlinNoise(x * blendNoiseScale + xOffset, y * blendNoiseScale + yOffset);
                float blueNoiseValue = Mathf.PerlinNoise(x * blendNoiseScale + yOffset, y * blendNoiseScale + xOffset);
                redNoiseMap[x, y] = redNoiseValue;
                blueNoiseMap[x, y] = blueNoiseValue;

                blendmapTexture.SetPixel(x, y, Color.green);

                if (redNoiseMap[x, y] < redThresholdMin)
                    blendmapTexture.SetPixel(x, y, Color.red);
                if(blueNoiseMap[x, y] > blueThresholdMax)
                    blendmapTexture.SetPixel(x, y, Color.blue);
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
