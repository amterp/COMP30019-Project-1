using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GenerateTerrain : MonoBehaviour {
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

    // This structure will contain the nodes/vertices for the terrain.
    // It is a 2D structure which can be queried as e.g. nodes[x, z].
    // z is used as the second index instead of y to reflect the fact
    // that the organization of these nodes are in order along the 
    // x-z axes. i.e. nodes[0, 0] is a node on the origin, and 
    // nodes[x, z] is the (x+1)th node over on the x axis, and (z+1)th
    // node over on the z axis.
    private Node[,] nodes;

    // Variables used as part of the actual diamond-step algorithm

    // Vector2Int will be used as a 2-pair of ints. Useful for storing
    // indexes to the 2D array 'nodes'.
    private Vector2Int[] verticesToInitialize;
    // A bool used to keep track of whether we've just performed a 
    // diamond or square step.
    private bool onDiamondStep;
    // A variable used to keep track of the number of nodes over to 
    // go when looking for neighbors in each step.
    private int matrixJumpSize;

    // Use this for initialization
    void Start() {
        // Grab references to components.
        meshFilter = GetComponent<MeshFilter>();

        // Calculate and set important values.

        Random.InitState(seed);
        // Comes from each side being 2^n + 1 nodes.
        numNodesPerSide = (int)Mathf.Pow(2, n) + 1;
        numNodes = numNodesPerSide * numNodesPerSide;

        // The actual in-Unity distance between nodes.
        distBetweenNodes = (float)sideLength / numNodesPerSide;
        Debug.Log(string.Format("numNodesPerSide: {0}, distBetweenNodes: {1}", numNodesPerSide, distBetweenNodes));

        // Create the terrain.
        GenerateNodes(); // Create the nodes/vertices for the terrain and place them.
        SetMeshVertices(meshFilter.mesh); // Define the vertices for the mesh.
        SetMeshTriangles(meshFilter.mesh); // Define the triangles for the mesh.

        // Prepare and perform diamond-square algorithm.
        onDiamondStep = true;
        matrixJumpSize = numNodesPerSide / 2;
        verticesToInitialize = new Vector2Int[1];
        verticesToInitialize[0] = new Vector2Int(matrixJumpSize, matrixJumpSize);
        PerformCompleteDS();

        // Update the mesh's vertices to reflect their new positions.
        SetMeshVertices(meshFilter.mesh);

        // Calculate the mesh's normals and tangents, to allow for proper lighting
        meshFilter.mesh.RecalculateNormals();
        meshFilter.mesh.RecalculateTangents();
    }

    /**
     * This creates the 2D data structure to contain the nodes for the terrain.
     * It does *not* initialize the corners.
     */
    private void GenerateNodes() {
        nodes = new Node[numNodesPerSide, numNodesPerSide];
        for (int z = 0; z < numNodesPerSide; z++) {
            for (int x = 0; x < numNodesPerSide; x++) {
                nodes[x, z] = new Node();
                nodes[x, z].pos = new Vector3(x * distBetweenNodes, 0, z * distBetweenNodes);

                // If the vertex is a corner, initiate its corner heights.
                if (IsCorner(x, z)) {
                    nodes[x, z].initialize(Random.Range(minCornerHeight, maxCornerHeight));
                }
            }
        }
    }

    /**
     * Pass the 2D 'nodes' structure as a 1D array into mesh.vertices, to define
     * the mesh's vertices. This is done in a simple, successive order (see the
     * loop), so the method that sets the triangles will be the complex one,
     * to properly pick the vertices in the right order.
     */
    private void SetMeshVertices(Mesh mesh) {
        Vector3[] flatVertices = new Vector3[numNodes];

        for (int z = 0, v = 0; z < numNodesPerSide; z++) {
            for (int x = 0; x < numNodesPerSide; x++, v++) {
                flatVertices[v] = nodes[x, z].pos;
            }
        }

        mesh.vertices = flatVertices;
    }

    /**
     * Defines the triangles in the mesh according to the vertices.
     * This method takes care of referencing each vertex in the correct order
     * so as to create the correct triangles in the correct direction i.e. clockwise.
     */
    private void SetMeshTriangles(Mesh mesh) {
        int[] triangles = new int[6 * (numNodesPerSide - 1) * (numNodesPerSide - 1)];
        for (int triangleIndex = 0, vertexIndex = 0, z = 0; z < numNodesPerSide - 1; z++, vertexIndex++) {
            for (int x = 0; x < numNodesPerSide - 1; x++, triangleIndex += 6, vertexIndex++) {
                // Each iteration inside this inner loop defines a full square i.e. two triangles.
                triangles[triangleIndex] = vertexIndex;

                // The following two lines define the triangle vertices that are shared.
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
    private void PerformCompleteDS() {
        // The last step will have a matrixJumpSize == 1. After that,
        // when matrixJumpSize is again divided by 2 at the end of the
        // square step, it will be made == 0. We can use this fact
        // to determine when the algorithm should stop.
        while (matrixJumpSize > 0) {
            PerformDSIteration();
        }
    }

    /**
     * Performs one full DS iteration i.e. both a diamond and a square step.
     */
    private void PerformDSIteration() {
        PerformDiamondStep();
        PerformSquareStep();
    }

    /**
     * Performs one diamond step of the algorithm.
     * Prepares properly for a diamond step.
     */
    private void PerformDiamondStep() {
        Debug.Log("Performing DIAMOND step...");
        if (!onDiamondStep) {
            // We're not on the diamond step i.e. we've performed one
            // already, and should be doing a square step.
            return;
        }

        // Get references to the vertices that will be processed *next* step
        // i.e. in the next square step. This is a set, as we don't want
        // 2+ references to the same node.
        HashSet<Vector2Int> verticesToInitializeNextStep = new HashSet<Vector2Int>();

        // Set the height of each vertex to be initialized this step, and grab
        // references to the vertices that need initiating *next* step.
        foreach (Vector2Int index in verticesToInitialize) {
            Debug.Log(string.Format("Initializing vertex: ({0}, {1})", index.x, index.y));

            // Get the neighbors that will be averaged to help find the height for *this* vertex.
            Vector2Int[] neighbors = GetDiamondJumpNeighbors(index.x, index.y, matrixJumpSize);
            Debug.Log("Num to average w/: " + neighbors.Length);

            float nextHeight = CalculateAverageHeight(neighbors) + Random.Range(minHeightAddition, maxHeightAddition);
            Debug.Log("Node height: " + nextHeight);
            nodes[index.x, index.y].initialize(nextHeight);

            // Add the square neighbors as vertices to be initialized next.
            neighbors = GetSquareJumpNeighbors(index.x, index.y, matrixJumpSize);
            foreach (Vector2Int neighbor in neighbors) {
                verticesToInitializeNextStep.Add(neighbor);
            }
        }

        // We've initialized all the nodes we're supposed to have this step.
        // Prepare for the next square step.

        verticesToInitialize = verticesToInitializeNextStep.ToArray();
        Debug.Log("Num next neighbors: " + verticesToInitialize.Length);
        minHeightAddition *= heightAdditionFactor;
        maxHeightAddition *= heightAdditionFactor;
        onDiamondStep = false;
    }

    /**
     * Performs one square step of the algorithm.
     * Prepares properly for a diamond step.
     */
    private void PerformSquareStep() {
        Debug.Log("Performing SQUARE step...");
        if (onDiamondStep) {
            // We're not on the square step i.e. we've performed one
            // already, and should be doing a diamond step.
            return;
        }

        // Get references to the vertices that will be processed *next* step
        // i.e. in the next diamond step. This is a set, as we don't want
        // 2+ references to the same node.
        HashSet<Vector2Int> verticesToInitializeNextStep = new HashSet<Vector2Int>();

        // Set the height of each vertex to be initialized this step, and grab
        // references to the vertices that need initiating *next* step.
        foreach (Vector2Int index in verticesToInitialize) {
            Debug.Log(string.Format("Initializing vertex: ({0}, {1})", index.x, index.y));

            // Get the neighbors that will be averaged to help find the height for *this* vertex.
            Vector2Int[] neighbors = GetSquareJumpNeighbors(index.x, index.y, matrixJumpSize);
            Debug.Log("Num to average w/: " + neighbors.Length);

            float nextHeight = CalculateAverageHeight(neighbors) + Random.Range(minHeightAddition, maxHeightAddition);
            Debug.Log("Node height: " + nextHeight);
            nodes[index.x, index.y].initialize(nextHeight);

            // Add the diamond neighbors as vertices to be initialized next. matrixJumpSize
            // is divided by two first as required by the algorithm.
            neighbors = GetDiamondJumpNeighbors(index.x, index.y, matrixJumpSize / 2);
            foreach (Vector2Int neighbor in neighbors) {
                verticesToInitializeNextStep.Add(neighbor);
            }
        }

        // We've initialized all the nodes we're supposed to have this step.
        // Prepare for the next diamond step.

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
    private float CalculateAverageHeight(Vector2Int[] indexes) {
        float sum = 0;
        foreach (Vector2Int index in indexes) {
            sum += nodes[index.x, index.y].pos.y;
        }

        return sum / indexes.Length;
    }

    /**
     * Gets the positions of nodes that are 'jumpSize' away from (x, z) in 'nodes'
     * in diagonal directions.
     */
    private Vector2Int[] GetDiamondJumpNeighbors(int x, int z, int jumpSize) {
        ArrayList neighbors = new ArrayList();

        // Try to add the top right neighbor.
        Vector2Int currentNeighbor = new Vector2Int(x + jumpSize, z + jumpSize);
        if (IsValidNode(currentNeighbor.x, currentNeighbor.y)) {
            neighbors.Add(new Vector2Int(currentNeighbor.x, currentNeighbor.y));
        }

        // Try to add the bottom right neighbor.
        currentNeighbor = new Vector2Int(x + jumpSize, z - jumpSize);
        if (IsValidNode(currentNeighbor.x, currentNeighbor.y)) {
            neighbors.Add(new Vector2Int(currentNeighbor.x, currentNeighbor.y));
        }

        // Try to add the bottom left neighbor.
        currentNeighbor = new Vector2Int(x - jumpSize, z - jumpSize);
        if (IsValidNode(currentNeighbor.x, currentNeighbor.y)) {
            neighbors.Add(new Vector2Int(currentNeighbor.x, currentNeighbor.y));
        }

        // Try to add the top left neighbor.
        currentNeighbor = new Vector2Int(x - jumpSize, z + jumpSize);
        if (IsValidNode(currentNeighbor.x, currentNeighbor.y)) {
            neighbors.Add(new Vector2Int(currentNeighbor.x, currentNeighbor.y));
        }

        return (Vector2Int[])neighbors.ToArray(typeof(Vector2Int));
    }

    /**
     * Gets the positions of nodes that are 'jumpSize' away from (x, z) in 'nodes'
     * in horizontal/vertical directions.
     */
    private Vector2Int[] GetSquareJumpNeighbors(int x, int z, int jumpSize) {
        ArrayList neighbors = new ArrayList();

        // Try to add the right neighbor.
        Vector2Int currentNeighbor = new Vector2Int(x + jumpSize, z);
        if (IsValidNode(currentNeighbor.x, currentNeighbor.y)) {
            neighbors.Add(new Vector2Int(currentNeighbor.x, currentNeighbor.y));
        }

        // Try to add the bottom neighbor.
        currentNeighbor = new Vector2Int(x, z - jumpSize);
        if (IsValidNode(currentNeighbor.x, currentNeighbor.y)) {
            neighbors.Add(new Vector2Int(currentNeighbor.x, currentNeighbor.y));
        }

        // Try to add the left neighbor.
        currentNeighbor = new Vector2Int(x - jumpSize, z);
        if (IsValidNode(currentNeighbor.x, currentNeighbor.y)) {
            neighbors.Add(new Vector2Int(currentNeighbor.x, currentNeighbor.y));
        }

        // Try to add the top neighbor.
        currentNeighbor = new Vector2Int(x, z + jumpSize);
        if (IsValidNode(currentNeighbor.x, currentNeighbor.y)) {
            neighbors.Add(new Vector2Int(currentNeighbor.x, currentNeighbor.y));
        }

        return (Vector2Int[])neighbors.ToArray(typeof(Vector2Int));
    }

    /**
     * Returns whether or the given indices correspond to a node in the terrain.
     * In other words, checks that 'nodes[x, z]' exists.
     */
    private bool IsValidNode(int x, int z) {
        return (x >= 0 && x < numNodesPerSide && z >= 0 && z < numNodesPerSide);
    }

    /**
     * Based on the supplied coordinates, determines if it would correspond to a
     * corner node in the 2D 'nodes' structure.
     */
    private bool IsCorner(int x, int z) {
        return (x == 0 && z == 0
                || x == 0 && z == numNodesPerSide - 1
                || x == numNodesPerSide - 1 && z == 0
                || x == numNodesPerSide - 1 && z == numNodesPerSide - 1);
    }

    /**
     * This can be used to draw vertices. It is set up to draw the nodes/vertices of
     * the map. Uncomment to see in Unity. Does lag a surprising amount though,
     * when enabled.
     */
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
