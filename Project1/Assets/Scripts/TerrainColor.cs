using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TerrainColor : MonoBehaviour {

    private MeshFilter meshFilter;

    // Values for Colour of the terrain.
    public Color sandColor;
    public Color snowColor;
    public Color defaultColor;
    public Color mountainColor;

    // Values for heights of different colours
    public float waterHeight = 10;
    public float mountainHeight = 12;
    public float snowHeight = 15;
    public float waterBufferZone = 0;
    public float mountainBufferZone = 0;
    public float snowBufferZone = 0;

    // Use Water Plane Height?
    public bool useWaterPlaneHeight = false;

    // Water Plane object height to determine values
    public GameObject waterPlane;

    // Use this for initialization
    public void Start () {

        // If we're using the water plane height, then calculate colours based on the water plane height
        if (useWaterPlaneHeight)
        {
            waterHeight = waterPlane.transform.position.y;
        }

        // Grab references to components.
        meshFilter = GetComponent<MeshFilter>();
        SetColorVertices(meshFilter.mesh);
    }

    void Update()
    {
        // Update our colours for live editing!
        SetColorVertices(meshFilter.mesh);
    }

    /**
     *  Takes the mesh and defines colours depending on the height value of
     *  the vertex.
     */
    public void SetColorVertices(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        Color[] colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            float h = 0;

            // Linearly interpolate colours from the defined colours when
            // above the water level
            // Also, apply a "buffer zone", which clamps values defined by the user which should allow
            // for more 'sharper' edges of each zone
            if (vertices[i].y < waterHeight)
            {
                // If underwater, just use the generic sand colour
                colors[i] = sandColor; 
            }
            else if (vertices[i].y < mountainHeight)
            {
                h = (mountainHeight - (vertices[i].y - waterBufferZone)) / (mountainHeight - waterHeight);
                colors[i] = Color.Lerp(defaultColor, sandColor, h);
            }
            else if (vertices[i].y < snowHeight)
            {
                 h = (snowHeight - (vertices[i].y - mountainBufferZone)) / (snowHeight - mountainHeight);
                colors[i] = Color.Lerp(mountainColor, defaultColor, h);
            }
            else
            {
                h = (snowHeight - (vertices[i].y - snowBufferZone)) / (snowHeight - mountainHeight);
                colors[i] = Color.Lerp(snowColor, mountainColor, h);
            }

        }

        mesh.colors = colors;
    }
}
