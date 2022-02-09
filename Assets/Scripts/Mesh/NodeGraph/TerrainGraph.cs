using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu(fileName = "TerraGraph", menuName = "Terrain/Terrain Graph")]
public class TerrainGraph : NodeGraph 
{
	public MeshData ProcessNodes(Vector2 chunkCord)
    {
        HeightValues vals = new HeightValues();
        MeshNode endPoint = null;

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

        return new MeshData(vals.GetPoints());
    }
}