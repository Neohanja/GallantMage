using UnityEngine;
using XNode;

public class RaiseTerrainNode : MeshNode
{
	public float minHeight;
	public float heightGrowth;

    public override void ProcessNode()
    {
		float[] vals = GetInputValue("entry", entry).CopyPoints();

		for (int x = 0; x < MeshData.MeshSize; x++)
		{
			for (int y = 0; y < MeshData.MeshSize; y++)
			{
				int index = x + y * MeshData.MeshSize;
				vals[index] *= heightGrowth;
				vals[index] += minHeight;
			}
		}

		points.SetPoints(vals);
	}

    public override string GetString()
    {
		return "RaiseTerrain";
    }
}
