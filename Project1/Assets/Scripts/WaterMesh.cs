using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WaterMesh : MonoBehaviour
{
    // The number of nodes per Unity length. Acts as a sort of resolution.
    public int nodesPerUnit = 2;

    private const int maxVerticesPerMesh = 65534;
    private int maxVerticesPerSide = (int) Mathf.Sqrt(maxVerticesPerMesh);

    private MeshFilter meshFilter;

    /**
     * Set transform of a plane. This method sets it to be the center of the terrain in
     * all directions, spanning as far out as the terrain does. This should be used with the
     * water plane. Additionally, it also returns the lowest and highest point of the map as an
     * array of floats of size 2.
     */
    public void Initialize(float sideLength, float lowestHeight, float highestHeight)
    {
        int numVerticesPerSide = (int) Mathf.Min(sideLength * nodesPerUnit, maxVerticesPerSide);
        float distBetweenVertices = sideLength / (numVerticesPerSide - 1);

        // Ensure we have a reference to the mesh filter.
        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
        }


        meshFilter.mesh = MeshGenerator.GenerateSquareMesh(numVerticesPerSide, distBetweenVertices);

        gameObject.AddComponent<MeshCollider>();

        // Set the water position.
        float waterHeight = (lowestHeight + highestHeight) / 2;
        transform.position = new Vector3(0, waterHeight, 0);
    }
}
