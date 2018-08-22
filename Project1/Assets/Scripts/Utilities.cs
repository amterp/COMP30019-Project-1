using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities
{
    /**
     * Gives a 2D array array of values, iterate through it and return a length 2 array
     * containing the min and max values in the 0th and 1st indices respectively.
     * Can be used to e.g. calculate the min and max height in an array of heights from
     * a terrain height map.
     */
    public static float[] GetMinMaxNodes(float[,] values)
    {
        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        // Iterate through all values and find the min and max values.
        // Complete this in one pass and return as an array to save time.
        foreach (float node in values)
        { 
            if (node < minValue)
            {
                minValue = node;
            } 
            else if (node > maxValue)
            {
                maxValue = node;
            }
        }

        // Return the values with 0th and 1st indices being the min
        // and max values respectively.
        return new float[]{minValue, maxValue};
    }
}
