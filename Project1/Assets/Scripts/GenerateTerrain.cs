using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GenerateTerrain : MonoBehaviour
{

    // The number of nodes in the equation 2^n + 1 where that equation
    // is the terrain's width and height. Does NOT affect its actual width or
    // height. That's reflected by 'sideLength'.
    public float n;

    // The Unity side length of the terrain.
    // High 'n', and low 'sideLength' -> High resolution, but small terrain.
    // Low 'n' and high 'sideLength' -> Low resolution, but large terrain.
    public float sideLength;
    public int seed;
    public float maxCornerHeight;
    public float minCornerHeight;

    // Constants.
    private int numNodesPerSide;
    private float distBetweenNodes;
    private int numNodes;
    private MeshFilter meshFilter;

    // Variables.
    private Vector3[,] vertices;

    // Use this for initialization
	void Start ()
	{
        // Grab references to components.
	    meshFilter = GetComponent<MeshFilter>();

        // Calculate and set important values.

	    Random.InitState(seed);
	    // Comes from each side being 2^n + 1 nodes.
	    numNodesPerSide = (int) (n * n) + 1;
	    numNodes = numNodesPerSide * numNodesPerSide;

        // The actual in-Unity distance between nodes.
	    distBetweenNodes = (float) sideLength / numNodesPerSide;

        // Create the terrain.
	    GenerateVertices(); // Create the vertices for the terrain and place them.
        GenerateMesh(meshFilter.mesh); // Create the mesh for the terrain

        Debug.Log(string.Format("numNodesPerSide: {0}, distBetweenNodes: {1}", numNodesPerSide, distBetweenNodes));
	}

    /**
     * This creates the 2D data structure to contain the vertices for the terrain.
     * It does *not* initialize the corners.
     */
    private void GenerateVertices()
    {
        vertices = new Vector3[numNodesPerSide, numNodesPerSide];
        for (int z = 0; z < numNodesPerSide; z++)
        {
            for (int x = 0; x < numNodesPerSide; x++)
            {
                vertices[x,z] = new Vector3(x * distBetweenNodes, 0, z * distBetweenNodes);
                
                // If the vertex is a corner, initiate its corner heights.
                if (isCorner(x, z))
                {
                    vertices[x,z].y = Random.Range(minCornerHeight, maxCornerHeight);
                }
            }
        }
    }

    private void GenerateMesh(Mesh mesh)
    {
        SetMeshVertices(mesh);
        SetMeshTriangles(mesh);
    }

    /**
     * Flatten the 2D vertices structure into 1D.
     * This will make it suppliable to the mesh.
     */
    private void SetMeshVertices(Mesh mesh)
    {
        Vector3[] flatVertices = new Vector3[numNodes];

        for (int z = 0, v = 0; z < numNodesPerSide; z++) {
            for (int x = 0; x < numNodesPerSide; x++, v++) {
                flatVertices[v] = vertices[x, z];
            }
        }

        mesh.vertices = flatVertices;
    }

    private void SetMeshTriangles(Mesh mesh)
    {
        int[] triangles = new int[6 * (numNodesPerSide - 1) * (numNodesPerSide - 1)];
        for (int triangleIndex = 0, vertexIndex = 0, z = 0; z < numNodesPerSide - 1; z++, vertexIndex++) {
            for (int x = 0; x < numNodesPerSide - 1; x++, triangleIndex += 6, vertexIndex++) {
                triangles[triangleIndex] = vertexIndex;
                triangles[triangleIndex + 3] = triangles[triangleIndex + 2] = vertexIndex + 1;
                triangles[triangleIndex + 4] = triangles[triangleIndex + 1] = vertexIndex + numNodesPerSide;
                triangles[triangleIndex + 5] = vertexIndex + numNodesPerSide + 1;
            }
        }

        mesh.triangles = triangles;
    }

    /**
     * Takes the private 2D field 'vertices' and sets the mesh equal to them.
     * OR can we have the mesh point directly to the Vector3s inside 'vertices' so it's not required?
     */
    private void UpdateVertices()
    {
        return;
    }

    /**
     * Based on the supplied coordinates, determines if it would correspond to a
     * corner node in the 2D 'vertices' structure.
     */
    private bool isCorner(int x, int z)
    {
        return (x == 0 && z == 0
                || x == 0 && z == numNodesPerSide - 1
                || x == numNodesPerSide - 1 && z == 0
                || x == numNodesPerSide - 1 && z == numNodesPerSide - 1);
    }

    private void OnDrawGizmos() {
        if (vertices == null) {
            return;
        }

        Gizmos.color = Color.black;
        for (int z = 0; z < numNodesPerSide; z++) {
            for (int x = 0; x < numNodesPerSide; x++) {
                Gizmos.DrawSphere(vertices[x,z], 0.1f);
            }
        }
    }
}
