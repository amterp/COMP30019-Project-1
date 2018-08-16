using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DiamondSquare
{
    private const int NumNeighbors = 4;

    private static int numVerticesPerSide;
    private static float minCornerHeight;
    private static float maxCornerHeight;
    private static float minHeightAddition;
    private static float maxHeightAddition;
    private static float heightAdditionFactor;
    private static float[,] heights;

    /**
     * Given all of the listed values, calculates an (2^n + 1) by (2^n + 1) float array
     * containing heights, as calculated by the diamond-square algorithm.
     */
    public static float[,] GetHeights(
        int seed,
        int n,
        float _minCornerHeight,
        float _maxCornerHeight,
        float _minHeightAddition,
        float _maxHeightAddition,
        float _heightAdditionFactor
    )
    {
        // Set utility fields.
        minCornerHeight = _minCornerHeight;
        maxCornerHeight = _maxCornerHeight;
        minHeightAddition = _minHeightAddition;
        maxHeightAddition = _maxHeightAddition;
        heightAdditionFactor = _heightAdditionFactor;

        // Calculate and set utility values.
        Random.InitState(seed);
        numVerticesPerSide = (int) Mathf.Pow(2, n) + 1;
        heights = new float[numVerticesPerSide, numVerticesPerSide];

        // Calculate the heights.
        InitializeCorners();
        PerformDiamondSquare();

        return heights;
    }

    /**
     * Initiates the 4 initial corners with random values.
     */
    private static void InitializeCorners() {
        // Initiate the corner heights.

        // Bottom left corner
        heights[0, 0] = Random.Range(minCornerHeight, maxCornerHeight);
        // Top left corner
        heights[0, numVerticesPerSide - 1] = Random.Range(minCornerHeight, maxCornerHeight);
        // Top right corner
        heights[numVerticesPerSide - 1, numVerticesPerSide - 1] = Random.Range(minCornerHeight, maxCornerHeight);
        // Bottom right corner
        heights[numVerticesPerSide - 1, 0] = Random.Range(minCornerHeight, maxCornerHeight);
    }

    /**
     * After the corner values have been initialized, this method can be called to calculate all the heights
     * in the 'heights' array, utilizing the diamond-square algorithm in an iterative implementation.
     */
    private static void PerformDiamondSquare()
    {
        // The last step will have a gridSize == 2. After that,
        // when gridSize is again divided by 2 at the end of the
        // square step, it will be made == 1, which corresponds
        // to just a point. We can use this fact to determine when
        // the algorithm should stop.

        // Iterate for as long as there are uninitialized heights.
        for (int gridSize = numVerticesPerSide - 1; gridSize > 1; gridSize /= 2)
        {
            // Calculate the distance between nodes to consider when
            // calculating heights.
            int matrixJumpSize = gridSize / 2;

            // Perform the diamond step.

            // Iterate over each (x, z) index that corresponds to an uninitialized height
            // that will be initialized in *this* diamond step.
            for (int zIndex = matrixJumpSize; zIndex < numVerticesPerSide - 1; zIndex += gridSize)
            {
                for (int xIndex = matrixJumpSize; xIndex < numVerticesPerSide - 1; xIndex += gridSize)
                {
                    // Grab a reference to the height of each corner of the grid from the current
                    // uninitialized height.
                    float bottomLeft = heights[xIndex - matrixJumpSize, zIndex - matrixJumpSize];
                    float topLeft = heights[xIndex - matrixJumpSize, zIndex + matrixJumpSize];
                    float topRight = heights[xIndex + matrixJumpSize, zIndex + matrixJumpSize];
                    float bottomRight = heights[xIndex + matrixJumpSize, zIndex - matrixJumpSize];

                    // Calculate the height of this point by averaging the 4 corners and
                    // adding a random amount to it as noise.
                    heights[xIndex, zIndex] = ((bottomLeft + topLeft + bottomRight + topRight) / NumNeighbors)
                                              + Random.Range(minHeightAddition, maxHeightAddition);
                }
            }

            // The diamond step has been completed, so we can now move onto the square step.

            // Iterate over each (x, z) index that corresponds to an uninitialized height
            // that will be initialized in *this* square step.          
            for (int zIndex = 0; zIndex < numVerticesPerSide; zIndex += matrixJumpSize)
            {
                // Determine the x coordinate of the first uninitialized node that is part
                // of this square step for the given z-row.
                int xIndexStart = zIndex % gridSize == 0 ? matrixJumpSize : 0;

                for (int xIndex = xIndexStart; xIndex < numVerticesPerSide; xIndex += gridSize)
                {
                    // Calculate the height for this point from the square neighbors.
                    heights[xIndex, zIndex] = CalculateAverageHeight(
                        xIndex + matrixJumpSize, zIndex,
                        xIndex - matrixJumpSize, zIndex,
                        xIndex, zIndex + matrixJumpSize,
                        xIndex, zIndex - matrixJumpSize
                    ) + Random.Range(minHeightAddition, maxHeightAddition);
                }
            }

            // Both diamond and square steps have been completed for this iteration.

            // Update the noise addition values.
            minHeightAddition *= heightAdditionFactor;
            maxHeightAddition *= heightAdditionFactor;
        }
    }

    /**
     * Given 4 pairs of coordinates, averages their height values,
     * taking into account whether or not they refer to a valid height
     * i.e. they do not index out of bounds.
     */
    private static float CalculateAverageHeight(
        int x1, int z1,
        int x2, int z2,
        int x3, int z3,
        int x4, int z4
    )
    {
        float sum = 0f;
        int count = 0;

        if (IsValidNode(x1, z1))
        {
            sum += heights[x1, z1];
            count++;
        }

        if (IsValidNode(x2, z2))
        {
            sum += heights[x2, z2];
            count++;
        }

        if (IsValidNode(x3, z3))
        {
            sum += heights[x3, z3];
            count++;
        }

        if (IsValidNode(x4, z4))
        {
            sum += heights[x4, z4];
            count++;
        }

        return sum / count;
    }

    /**
     * Returns whether or the given indices correspond to a node in the terrain.
     * In other words, checks that 'nodes[x, z]' exists.
     */
    private static bool IsValidNode(int x, int z) {
        return (x >= 0 && x < numVerticesPerSide && z >= 0 && z < numVerticesPerSide);
    }
}
