using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GenerateTerrain : MonoBehaviour
{

    // The number of nodes in the equation 2^n + 1 where that equation
    // is the terrain's width and height. Does NOT affect its actual width or
    // height. That's reflected by 'sideLength'. Cannot be > 15 due to the 
    // ~65000 vertice limit on a single mesh. 2^15 + 1 = 32769.
    public float n;

    // The Unity side length of the terrain.
    // High 'n', and low 'sideLength' -> High resolution, but small terrain.
    // Low 'n' and high 'sideLength' -> Low resolution, but large terrain.
    public float sideLength;
    public int seed;
    public float maxCornerHeight;
    public float minCornerHeight;

    // Diamond-square tweakable values.

    public float maxHeightAddition;
    public float minHeightAddition;

    // The % amount that the heightAddition fields should be multiplied
    // by every step to reduce the random value added to each node.
    public float heightAdditionFactor;

    // Constants.

    private int numNodesPerSide;
    private float distBetweenNodes;
    private int numNodes;
    private MeshFilter meshFilter;

    // Variables.

    private Node[,] nodes;

    // Variables used as part of the actual diamond-step algorithm

    // Vector2Int will be used as a 2-pair of ints. Useful for storing
    // indexes to the 2D array 'nodes'.
    private Vector2Int[] verticesToInitialize;
    private bool onDiamondStep;
    private int matrixJumpSize;

    // Use this for initialization
	void Start ()
	{
        // Grab references to components.
	    meshFilter = GetComponent<MeshFilter>();

        // Calculate and set important values.

	    Random.InitState(seed);
	    // Comes from each side being 2^n + 1 nodes.
	    numNodesPerSide = (int) Mathf.Pow(2, n) + 1;
	    numNodes = numNodesPerSide * numNodesPerSide;

        // The actual in-Unity distance between nodes.
	    distBetweenNodes = (float) sideLength / numNodesPerSide;
	    Debug.Log(string.Format("numNodesPerSide: {0}, distBetweenNodes: {1}", numNodesPerSide, distBetweenNodes));

	    // Create the terrain.
	    GenerateNodes(); // Create the nodes/vertices for the terrain and place them.
	    GenerateMesh(meshFilter.mesh); // Create the mesh for the terrain

        // Prepare and perform diamond-square algorithm.
	    onDiamondStep = true;
	    matrixJumpSize = numNodesPerSide / 2;
        verticesToInitialize = new Vector2Int[1];
        verticesToInitialize[0] = new Vector2Int(matrixJumpSize, matrixJumpSize);
        Debug.Log("matrixJumpSize: " + matrixJumpSize);
        PerformCompleteDS();

        // Update the mesh's vertices to reflect their new positions.
	    SetMeshVertices(meshFilter.mesh);

	    meshFilter.mesh.RecalculateNormals();
	    meshFilter.mesh.RecalculateTangents();
    }

    /**
     * This creates the 2D data structure to contain the nodes for the terrain.
     * It does *not* initialize the corners.
     */
    private void GenerateNodes()
    {
        nodes = new Node[numNodesPerSide, numNodesPerSide];
        for (int z = 0; z < numNodesPerSide; z++)
        {
            for (int x = 0; x < numNodesPerSide; x++)
            {
                nodes[x, z] = new Node();
                nodes[x, z].pos = new Vector3(x * distBetweenNodes, 0, z * distBetweenNodes);

                // If the vertex is a corner, initiate its corner heights.
                if (isCorner(x, z))
                {
                    nodes[x,z].initialize(Random.Range(minCornerHeight, maxCornerHeight));
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
     * Flatten the 2D nodes structure into 1D.
     * This will make it suppliable to the mesh.
     */
    private void SetMeshVertices(Mesh mesh)
    {
        Vector3[] flatVertices = new Vector3[numNodes];

        for (int z = 0, v = 0; z < numNodesPerSide; z++) {
            for (int x = 0; x < numNodesPerSide; x++, v++) {
                flatVertices[v] = nodes[x, z].pos;
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
     * Performs the diamond-square algorithm to completion i.e. to the
     * point where every node has been initialized.
     */
    private void PerformCompleteDS()
    {
        while (matrixJumpSize > 0)
        {
            PerformDSIteration();
        }
    }

    private void PerformDSIteration()
    {
        // Calculate step size to neighbors
        // Half step size every DS-iteration (i.e. after both diamond and square step)
        // nodes[x + halfStep, z + halfStep] during diamond step
        // e.g. nodes[x, z + halfStep] during square step
        // How to know which nodes to expand from?
        PerformDiamondStep();
        PerformSquareStep();
    }

    private void PerformDiamondStep()
    {
        Debug.Log("Performing DIAMOND step...");
        if (!onDiamondStep)
        {
            // We're not on the diamond step i.e. we've performed one
            // already, and should be doing a square step.
            return;
        }

        HashSet<Vector2Int> verticesToInitializeNextStep = new HashSet<Vector2Int>();

        foreach (Vector2Int index in verticesToInitialize)
        {
            Debug.Log(string.Format("Initializing vertex: ({0}, {1})", index.x, index.y));
            Vector2Int[] neighbors = GetDiagonalJumpNeighbors(index.x, index.y, matrixJumpSize);
            Debug.Log("Num to average w/: " + neighbors.Length);

            float nextHeight = CalculateAverageHeight(neighbors) + Random.Range(minHeightAddition, maxHeightAddition);
            Debug.Log("Node height: " + nextHeight);
            nodes[index.x, index.y].initialize(nextHeight);

            // Add the square neighbors as vertices to be initialized next.
            neighbors = GetHorizontalJumpNeighbors(index.x, index.y, matrixJumpSize);
            foreach (Vector2Int neighbor in neighbors)
            {
                verticesToInitializeNextStep.Add(neighbor);
            }
        }

        // We've initialized all the nodes we're supposed to have this step.
        // Prepare for the next step.

        verticesToInitialize = verticesToInitializeNextStep.ToArray();
        Debug.Log("Num next neighbors: " + verticesToInitialize.Length);
        minHeightAddition *= heightAdditionFactor;
        maxHeightAddition *= heightAdditionFactor;
        onDiamondStep = false;
    }

    private void PerformSquareStep()
    {
        Debug.Log("Performing SQUARE step...");
        if (onDiamondStep) {
            // We're not on the square step i.e. we've performed one
            // already, and should be doing a diamond step.
            return;
        }

        HashSet<Vector2Int> verticesToInitializeNextStep = new HashSet<Vector2Int>();

        foreach (Vector2Int index in verticesToInitialize) {
            Debug.Log(string.Format("Initializing vertex: ({0}, {1})", index.x, index.y));
            Vector2Int[] neighbors = GetHorizontalJumpNeighbors(index.x, index.y, matrixJumpSize);
            Debug.Log("Num to average w/: " + neighbors.Length);

            float nextHeight = CalculateAverageHeight(neighbors) + Random.Range(minHeightAddition, maxHeightAddition);
            Debug.Log("Node height: " + nextHeight);
            nodes[index.x, index.y].initialize(nextHeight);

            // Add the square neighbors as vertices to be initialized next.
            neighbors = GetDiagonalJumpNeighbors(index.x, index.y, matrixJumpSize/2);
            foreach (Vector2Int neighbor in neighbors) {
                verticesToInitializeNextStep.Add(neighbor);
            }
        }

        verticesToInitialize = verticesToInitializeNextStep.ToArray();
        Debug.Log("Num next neighbors: " + verticesToInitialize.Length);
        minHeightAddition *= heightAdditionFactor;
        maxHeightAddition *= heightAdditionFactor;
        matrixJumpSize /= 2;
        onDiamondStep = true;
    }

    /**
     * Calculates the average of the y components of each Node
     * referred to by index in the given list i.e. the average height.
     */
    private float CalculateAverageHeight(Vector2Int[] indexes)
    {
        float sum = 0;
        foreach (Vector2Int index in indexes)
        {
            sum += nodes[index.x, index.y].pos.y;
        }

        return sum / indexes.Length;
    }

    private Vector2Int[] GetDiagonalJumpNeighbors(int x, int z, int jumpSize)
    {
        ArrayList neighbors = new ArrayList();

        // Try to add the top right neighbor.
        Vector2Int currentNeighbor = new Vector2Int(x + jumpSize, z + jumpSize);
        if (IsValidNode(currentNeighbor.x, currentNeighbor.y))
        {
            neighbors.Add(new Vector2Int(currentNeighbor.x, currentNeighbor.y));
        }

        // Try to add the bottom right neighbor.
        currentNeighbor = new Vector2Int(x + jumpSize, z - jumpSize);
        if (IsValidNode(currentNeighbor.x, currentNeighbor.y))
        {
            neighbors.Add(new Vector2Int(currentNeighbor.x, currentNeighbor.y));
        }

        // Try to add the bottom left neighbor.
        currentNeighbor = new Vector2Int(x - jumpSize, z - jumpSize);
        if (IsValidNode(currentNeighbor.x, currentNeighbor.y))
        {
            neighbors.Add(new Vector2Int(currentNeighbor.x, currentNeighbor.y));
        }

        // Try to add the top left neighbor.
        currentNeighbor = new Vector2Int(x - jumpSize, z + jumpSize);
        if (IsValidNode(currentNeighbor.x, currentNeighbor.y))
        {
            neighbors.Add(new Vector2Int(currentNeighbor.x, currentNeighbor.y));
        }

        return (Vector2Int[]) neighbors.ToArray(typeof(Vector2Int));
    }

    private Vector2Int[] GetHorizontalJumpNeighbors(int x, int z, int jumpSize)
    {
        ArrayList neighbors = new ArrayList();

        // Try to add the right neighbor.
        Vector2Int currentNeighbor = new Vector2Int(x + jumpSize, z);
        if (IsValidNode(currentNeighbor.x, currentNeighbor.y))
        {
            neighbors.Add(new Vector2Int(currentNeighbor.x, currentNeighbor.y));
        }

        // Try to add the bottom neighbor.
        currentNeighbor = new Vector2Int(x, z - jumpSize);
        if (IsValidNode(currentNeighbor.x, currentNeighbor.y))
        {
            neighbors.Add(new Vector2Int(currentNeighbor.x, currentNeighbor.y));
        }

        // Try to add the left neighbor.
        currentNeighbor = new Vector2Int(x - jumpSize, z);
        if (IsValidNode(currentNeighbor.x, currentNeighbor.y))
        {
            neighbors.Add(new Vector2Int(currentNeighbor.x, currentNeighbor.y));
        }

        // Try to add the top neighbor.
        currentNeighbor = new Vector2Int(x, z + jumpSize);
        if (IsValidNode(currentNeighbor.x, currentNeighbor.y))
        {
            neighbors.Add(new Vector2Int(currentNeighbor.x, currentNeighbor.y));
        }

        return (Vector2Int[]) neighbors.ToArray(typeof(Vector2Int));
    }

    /**
     * Returns whether or the given indices correspond to a node in the terrain.
     * In other words, checks that 'nodes[x, z]' exists.
     */
    private bool IsValidNode(int x, int z)
    {
        return (x >= 0 && x < numNodesPerSide && z >= 0 && z < numNodesPerSide);
    }
    
    /**
     * Based on the supplied coordinates, determines if it would correspond to a
     * corner node in the 2D 'nodes' structure.
     */
    private bool isCorner(int x, int z)
    {
        return (x == 0 && z == 0
                || x == 0 && z == numNodesPerSide - 1
                || x == numNodesPerSide - 1 && z == 0
                || x == numNodesPerSide - 1 && z == numNodesPerSide - 1);
    }

//    private void OnDrawGizmos() {
//        if (nodes == null) {
//            return;
//        }
//
//        Gizmos.color = Color.black;
//        for (int z = 0; z < numNodesPerSide; z++) {
//            for (int x = 0; x < numNodesPerSide; x++) {
//                Gizmos.DrawSphere(nodes[x,z].pos, 0.1f);
//            }
//        }
//    }
}
