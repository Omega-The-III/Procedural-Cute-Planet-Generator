using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] GameObject planetObject;
    private MeshRenderer planetRenderer;

    [SerializeField] private int textureSize;
    [SerializeField] private float mountainThreshold, waterThreshold, heightMapNoiseScale;
    enum MapType { heightMap, textureMap };

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
                case MapType.textureMap:
                    //create texture map
                    break;
            }
        }
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
                float noiseValue = Mathf.PerlinNoise(x * heightMapNoiseScale + xOffset, y * heightMapNoiseScale + yOffset);
                noiseMap[x, y] = noiseValue;

                if (noiseMap[x, y] < waterThreshold)
                    noiseTexture.SetPixel(x, y, Color.white);
                else if (noiseMap[x, y] > mountainThreshold)
                    noiseTexture.SetPixel(x, y, Color.black);
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
