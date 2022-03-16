using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon
{
    public List<int> m_Vertices;
    public List<Polygon> m_Neighbors;
    public Color32 m_Color;
    public bool m_SmoothNormals;
    public Polygon(int a, int b, int c)
    {
        m_Vertices = new List<int>() { a, b, c };
        m_Neighbors = new List<Polygon>();
        // This will determine whether a polygon's normals smoothly
        // blend into its neighbors, or if it should have sharp edges.
        m_SmoothNormals = true;
        // Hot Pink is an excellent default color because you'll 
        // notice instantly if you forget to set it to something else.
        m_Color = new Color32(255, 0, 255, 255);
    }
    public bool IsNeighborOf(Polygon other_poly)
    {
        int shared_vertices = 0;
        foreach (int vertex in m_Vertices)
        {
            if (other_poly.m_Vertices.Contains(vertex))
                shared_vertices++;
        }
        // A polygon and its neighbor will share exactly
        // two vertices. Ergo, if this poly shares two
        // vertices with the other, then they are neighbors.
        return shared_vertices == 2;
    }
    // Mr. Roger's voice: Please won't you replace my neighbor.
    public void ReplaceNeighbor(Polygon oldNeighbor,
                                Polygon newNeighbor)
    {
        for (int i = 0; i < m_Neighbors.Count; i++)
        {
            if (oldNeighbor == m_Neighbors[i])
            {
                m_Neighbors[i] = newNeighbor;
                return;
            }
        }
    }
}
