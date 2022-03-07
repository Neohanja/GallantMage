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

    public override void ProcessNode(bool addImage = false)
    {
		float[] vals = GetInputValue("entry", entry).CopyPoints();
		Color[] cols = new Color[vals.Length];

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

		if (addImage)
		{
			for (int x = 0; x < MeshData.MeshSize; x++)
			{
				for (int y = 0; y < MeshData.MeshSize; y++)
				{
					int index = x + y * MeshData.MeshSize;
					if (addImage) cols[index] = Color.Lerp(Color.black, Color.white, vals[index]);
				}
			}

			image.SetPixels(cols);
			image.filterMode = FilterMode.Point;
			image.wrapMode = TextureWrapMode.Clamp;
			image.Apply();
		}

		points.SetPoints(vals);
	}

    public override bool LastNode()
    {
        return true;
    }
}
