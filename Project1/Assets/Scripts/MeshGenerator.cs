using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator {

    public static Mesh GenerateSquareMesh(int numVerticesPerSide, float distBetweenVertices)
    {
        Mesh mesh = new Mesh();

        CreateVertices(mesh, numVerticesPerSide, distBetweenVertices);
        SetMeshTriangles(mesh, numVerticesPerSide);

        return mesh;
    }

    /**
     * Defines the triangles in the mesh according to the vertices.
     * This method takes care of referencing each vertex in the correct order
     * so as to create the correct triangles in the correct direction i.e. clockwise.
     */
    public static void SetMeshTriangles(Mesh mesh, int numVerticesPerSide) {
        int[] triangles = new int[6 * (numVerticesPerSide - 1) * (numVerticesPerSide - 1)];
        for (int triangleIndex = 0, vertexIndex = 0, z = 0; z < numVerticesPerSide - 1; z++, vertexIndex++) {
            for (int x = 0; x < numVerticesPerSide - 1; x++, triangleIndex += 6, vertexIndex++) {
                // Each iteration inside this inner loop defines a full square i.e. two triangles.
                triangles[triangleIndex] = vertexIndex;

                // The following two lines define the triangle vertices that are shared.
                triangles[triangleIndex + 3] = triangles[triangleIndex + 2] = vertexIndex + 1;
                triangles[triangleIndex + 4] = triangles[triangleIndex + 1] = vertexIndex + numVerticesPerSide;

                triangles[triangleIndex + 5] = vertexIndex + numVerticesPerSide + 1;
            }
        }

        mesh.triangles = triangles;
    }

    /**
     * Creates the vertices in the given mesh in row by row order to form a flat mesh.
     */
    private static void CreateVertices(Mesh mesh, int numVerticesPerSide, float distBetweenVertices)
    {
        Vector3[] vertices = new Vector3[numVerticesPerSide * numVerticesPerSide];

        for (int z = 0, v = 0; z < numVerticesPerSide; z++) {
            for (int x = 0; x < numVerticesPerSide; x++, v++) {
                vertices[v] = new Vector3(x * distBetweenVertices, 0, z * distBetweenVertices);
            }
        }

        mesh.vertices = vertices;
    }
}
