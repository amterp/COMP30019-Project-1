using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;

public class TerrainUtilities {

    // This acts as a static class which provides utilities for Terrain-related operations
    // e.g. getting the highest point on the map.

    public static float[] GetMinMaxNodes(float[,] map)
    {
        float maxNode = float.MinValue;
        float minNode = float.MaxValue;

        // Iterate through all nodes and find the min/max of the terrain
        // Complete this in one pass and return as an array to save time
        foreach (float node in map)
        { 
            if ((maxNode == null) || (node > maxNode))
            {
                maxNode = node;
            }
            if ((minNode == null) || (node < minNode))
            {
                minNode = node;
            }
        }

        float[] result = new float[2];
        result[0] = maxNode;
        result[1] = minNode;

        return result;

    }


}
