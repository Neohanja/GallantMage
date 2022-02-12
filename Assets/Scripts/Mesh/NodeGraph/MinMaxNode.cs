using UnityEngine;
using XNode;

public class MinMaxNode : MeshNode 
{
	public float min;
	public bool useMin;
	public float max;
	public bool useMax;

	public override void ProcessNode(bool addImage = false)
    {
        float[] vals = GetInputValue("entry", entry).CopyPoints();
		Color[] cols = new Color[vals.Length];

        for (int x = 0; x < MeshData.MeshSize; x++)
		{
			for (int y = 0; y < MeshData.MeshSize; y++)
			{
				int index = x + y * MeshData.MeshSize;

				if (vals[index] < min && useMin) vals[index] = min;
				if (vals[index] > max && useMax) vals[index] = max;

				if(addImage) cols[index] = Color.Lerp(Color.black, Color.white, vals[index]);
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

    public override string GetString()
    {
		return "MinMaxNode";
	}
}