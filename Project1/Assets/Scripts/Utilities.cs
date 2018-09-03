using System;
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
        foreach (float value in values)
        { 
            if (value < minValue)
            {
                minValue = value;
            } 
            else if (value > maxValue)
            {
                maxValue = value;
            }
        }

        // Return the values with 0th and 1st indices being the min
        // and max values respectively.
        return new float[]{minValue, maxValue};
    }

    /** The seed input field is empty. Default to a "random" seed i.e.
    * the last 'charLength' digits from current time in ticks where 'charLength'
    * is the max number of digits for a seed.
    */
    public static int GetTimeBasedSeed(int charLength)
    {
        long ticks = DateTime.Now.Ticks;
        long modNum = (long)(Mathf.Pow(10, charLength - 1));
        return (int)(ticks % modNum);
    }
}
