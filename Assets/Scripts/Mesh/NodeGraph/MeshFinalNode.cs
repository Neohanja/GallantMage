using UnityEngine;
using XNode;

public class MeshFinalNode : MeshNode
{
    public float minHeight;
    public float heightGrowth;

    public override string GetString()
    {
        return "EndNode";
    }

    public override void ProcessNode(bool addImage = false)
    {
		float[] vals = GetInputValue("entry", entry).CopyPoints();
		Color[] cols = new Color[vals.Length];

		for (int x = 0; x < MeshData.MeshSize; x++)
		{
			for (int y = 0; y < MeshData.MeshSize; y++)
			{
				int index = x + y * MeshData.MeshSize;
				if (addImage) cols[index] = Color.Lerp(Color.black, Color.white, vals[index]);
				vals[index] *= heightGrowth;
				vals[index] += minHeight;
			}
		}
		if (addImage)
		{
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
