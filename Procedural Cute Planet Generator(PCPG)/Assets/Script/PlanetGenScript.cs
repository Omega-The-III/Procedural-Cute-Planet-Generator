using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGenScript : MonoBehaviour
{
    [SerializeField] private Vector3[,] verticeMap;

    private GameObject planetObject;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Mesh planetMesh;

    private Vector3 center;

    private void Start()
    {
        if (planetObject == null)
            planetObject = GameObject.Find("PlanetMesh");
        if (meshFilter == null)
            meshFilter = planetObject.GetComponent<MeshFilter>();
        if (meshRenderer == null)
            meshRenderer = planetObject.GetComponent<MeshRenderer>();
        if (planetMesh == null)
            planetMesh = planetObject.GetComponent<MeshFilter>().sharedMesh;

        verticeMap = new Vector3[10, 10];
        center = planetObject.transform.position;

        Vector3[] verticeCenterDirection = new Vector3[planetMesh.vertexCount];
        for (int y = 0; y < planetMesh.vertexCount; y++)
        {
            for (int x = 0; x < planetMesh.vertexCount; x++)
            {
                //verticeMap[i] = planetMesh.vertices[i];
                //verticeCenterDirection[i] = ((verticeMap[i] - center).normalized);
            }
        }
    }
}