using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://www.youtube.com/watch?v=7E1EqekXnwQ

public class Chunk : MonoBehaviour
{
    [Header("Node Builder")]
    public TerrainGraph terrainGraph;

    [Header("Materials")]
    public MeshRenderer meshRender;
    public MeshFilter meshFilter;
    MeshData chunkMesh;

    [Header("Game Settings")]
    public Vector2 chunkCoord;

    [Header("Editor Settings")]
    public bool autoUpdate;

    void Start()
    {
        if (meshRender == null) meshRender = gameObject.AddComponent<MeshRenderer>();
        if (meshFilter == null) meshFilter = gameObject.AddComponent<MeshFilter>();
        CompileNodeGraph();
    }

    public void CompileNodeGraph()
    {
        foreach(MeshNode m in terrainGraph.nodes)
        {
            if(m.GetString() == "BaseNode")
            {
                terrainGraph.current = m;
                break;
            }
        }

        //Add code to move through the node paths here. <--WIP-->

        chunkMesh = terrainGraph.current.GetMesh();

        meshFilter.mesh = chunkMesh.GetMesh();
    }
}
