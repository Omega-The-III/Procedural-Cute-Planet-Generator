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
    public bool hasUpdated = false;

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

        //todo make array into 2d array, somehow.
        //all x and y positions are in the 'same' position. a vertice on x = 1 and y = 11.567 is will also have a verticy on x = 2 and y = 11.567
    }
}