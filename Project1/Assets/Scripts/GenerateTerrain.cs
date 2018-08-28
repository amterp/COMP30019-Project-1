using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GenerateTerrain : MonoBehaviour
{
    // Public input fields.

    // The number of nodes in the equation 2^n + 1 where that equation
    // is the terrain's width and height. Does NOT affect its actual width or
    // height. That's reflected by 'sideLength'. Cannot be > 15 due to the 
    // ~65000 vertice limit on a single mesh. 2^15 + 1 = 32769.
    public int n;

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

    public Transform waterTransform;

    // Min and max heights of the current terrain.
    [HideInInspector]
    public float minHeight;
    [HideInInspector]
    public float maxHeight;

    // Constants.

    private int numNodesPerSide;
    private float distBetweenNodes;
    private int numNodes;
    private MeshFilter meshFilter;
    private TerrainColor terrainColorizer;

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

    // Nodes to initialize on the next step of the algorithm.
    private Node[] verticesToInitialize;
    // A bool used to keep track of whether we've just performed a 
    // diamond or square step.
    private bool onDiamondStep;
    // A variable used to keep track of the number of nodes over to 
    // go when looking for neighbors in each step.
    private int matrixJumpSize;

    // Heights of our terrain.
    private float[,] heights;

    // Use this for initialization
    void Start() {
        Initialize();
        Generate(seed);
    }

    void Update() {
        // Update the terrain's colors for live editing.
        terrainColorizer.SetColorVertices(meshFilter.mesh);
    }

    /**
     * Must be called after Initialize() has been called at least one.
     * Utilizes the values set in Initialize() to generate a height map and then
     * creating a terrain mesh for it. Also properly positions the water.
     */
    public void Generate(int _seed)
    {
        // Calculate and set important values.

        // Comes from each side being 2^n + 1 nodes.
        numNodesPerSide = (int)Mathf.Pow(2, n) + 1;
        numNodes = numNodesPerSide * numNodesPerSide;

        // The actual in-Unity distance between nodes.
        // TODO: the -1 is required to make the size properly match 'sideLength'. That's a bit weird.
        distBetweenNodes = (float)sideLength / (numNodesPerSide - 1);
        Debug.Log(string.Format("numNodesPerSide: {0}, distBetweenNodes: {1}", numNodesPerSide, distBetweenNodes));

        // Calculate the terrain.
        heights = DiamondSquare.GetHeights(_seed, n, minCornerHeight, maxCornerHeight, minHeightAddition,
            maxHeightAddition,
            heightAdditionFactor);
        CreateVertices(); // Create the nodes/vertices for the terrain and place them.

        // Define the mesh.
        SetMeshVertices(meshFilter.mesh); // Define the vertices for the mesh.
        SetMeshTriangles(meshFilter.mesh); // Define the triangles for the mesh.

        // Calculate the mesh's normals and tangents, to allow for proper lighting
        meshFilter.mesh.RecalculateNormals();
        meshFilter.mesh.RecalculateTangents();
        meshFilter.mesh.RecalculateBounds();

        // Set the height of the water plane (in the middle of the highest and lowest point of the terrain)
        // Also, get our highest/lowest points of the terrain for later shading
        float[] minMaxHeights = SetPlaneTransform();

        // Generate a collision mesh, first destroying any existing one.
        Destroy(gameObject.GetComponent<MeshCollider>());
        gameObject.AddComponent<MeshCollider>();

        // Initialize terrain colors.
        terrainColorizer.Initialize(waterTransform, minMaxHeights);
    }

    /**
     * Initializes all the important values that will be used to generate the terrain.
     */
    private void Initialize()
    {
        Debug.Log("Initializing terrain.");
        // Grab references to components.
        meshFilter = GetComponent<MeshFilter>();
        terrainColorizer = GetComponent<TerrainColor>();
    }

    /**
     * This creates the 2D data structure to contain the nodes for the terrain.
     * It does *not* initialize the corners.
     */
    private void CreateVertices() {
        nodes = new Node[numNodesPerSide, numNodesPerSide];
        for (int z = 0; z < numNodesPerSide; z++) {
            for (int x = 0; x < numNodesPerSide; x++) {
                nodes[x, z] = new Node(new Vector3(x * distBetweenNodes, heights[x, z], z * distBetweenNodes));
            }
        }
    }

    /**
     * Set transform of a plane. This method sets it to be the center of the terrain in
     * all directions, spanning as far out as the terrain does. This should be used with the
     * water plane. Additionally, it also returns the lowest and highest point of the map as an
     * array of floats of size 2.
     */
    private float[] SetPlaneTransform()
    {
        // Get our highest and lowest points on the map
        float[] minMaxNodes = Utilities.GetMinMaxNodes(heights);
        minHeight = minMaxNodes[0];
        maxHeight = minMaxNodes[1];

        // Set the water position.
        float waterHeight = (minHeight + maxHeight) / 2;
        waterTransform.position = new Vector3(sideLength / 2, waterHeight, sideLength / 2);

        // Set the water scale.
        waterTransform.localScale = new Vector3(sideLength, 1, sideLength);

        return minMaxNodes;
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
}
