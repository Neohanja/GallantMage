using UnityEngine;
using XNode;

public class MeshFinalNode : MeshNode
{
	public bool normalize;
	public bool unclampValue;

    public override string GetString()
    {
        return "EndNode";
    }

    public override void ProcessNode()
    {
		float[] vals = GetInputValue("entry", entry).CopyPoints();

		if (normalize)
		{
			float min = float.MaxValue;
			float max = float.MinValue;

			for (int x = 0; x < MeshData.MeshSize; x++)
			{
				for (int y = 0; y < MeshData.MeshSize; y++)
				{
					int index = x + y * MeshData.MeshSize;
					if (vals[index] < min) min = vals[index];
					if (vals[index] > max) max = vals[index];
				}
			}

			for (int x = 0; x < MeshData.MeshSize; x++)
			{
				for (int y = 0; y < MeshData.MeshSize; y++)
				{
					int index = x + y * MeshData.MeshSize;
					vals[index] = Mathf.InverseLerp(min, max, vals[index]);
					if (unclampValue) vals[index] = vals[index] * 2 - 1f;
				}
			}
		}

		points.SetPoints(vals);
	}

    public override bool LastNode()
    {
        return true;
    }
}
