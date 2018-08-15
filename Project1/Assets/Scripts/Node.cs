using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Node
{

    public Vector3 pos;
    // private Color color;
    private bool initialized;

    public Node()
    {
    }

    public void initialize(float y)
    {
        if (this.initialized)
        {
            Debug.LogError("Node already initialized! Pos: " + pos.ToString());
        }
        this.pos.y = y;
        this.initialized = true;
    }

    public void uninitialize()
    {
        this.pos.y = 0;
        this.initialized = false;
    }
}
