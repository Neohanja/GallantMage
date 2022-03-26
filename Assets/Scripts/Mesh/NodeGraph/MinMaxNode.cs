using UnityEngine;
using XNode;

public class MinMaxNode : MeshNode 
{
	public float min;
	public bool useMin;
	public float max;
	public bool useMax;

	public override void ProcessNode()
    {
        float[] vals = GetInputValue("entry", entry).CopyPoints();

        for (int x = 0; x < MeshData.MeshSize; x++)
		{
			for (int y = 0; y < MeshData.MeshSize; y++)
			{
				int index = x + y * MeshData.MeshSize;

				if (vals[index] < min && useMin) vals[index] = min;
				if (vals[index] > max && useMax) vals[index] = max;
			}
		}

		points.SetPoints(vals);
    }

    public override string GetString()
    {
		return "MinMaxNode";
	}
}