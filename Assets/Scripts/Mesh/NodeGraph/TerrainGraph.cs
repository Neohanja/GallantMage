using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu(fileName = "TerraGraph", menuName = "Gallant Mage/Terrain Graph")]
public class TerrainGraph : NodeGraph 
{
    public Vector2 offset;
    public int seed;

	public ChunkData ProcessNodes(Vector2 chunkCord, int seed)
    {
        ChunkData vals = new ChunkData();
        MeshNode endPoint = null;

        offset = chunkCord;
        this.seed = seed;

        foreach(MeshNode m in nodes)
        {
            if (m.LastNode())
            {
                endPoint = m;
                break;
            }
        }

        if (endPoint != null)
            vals = endPoint.GetMesh();

        return vals;
    }
}