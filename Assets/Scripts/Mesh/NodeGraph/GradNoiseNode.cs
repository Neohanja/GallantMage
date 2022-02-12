using UnityEngine;
using XNode;

/// <summary>
/// Creates a 2D Noise Map
/// </summary>
public class GradNoiseNode : MeshNode
{
	public int zOffset;
	public float scale;
	public bool noise0To1;

	public override void ProcessNode(bool addImage = false)
	{
		float[] vals = new float[MeshData.MeshSize * MeshData.MeshSize];

		Color[] cols = new Color[vals.Length];

		for (int x = 0; x < MeshData.MeshSize; x++)
		{
			for (int y = 0; y < MeshData.MeshSize; y++)
			{
				int index = x + y * MeshData.MeshSize;

				float xSample = (GetGraph.offset.x + x) / scale;
				float ySample = (GetGraph.offset.y + y) / scale;

				float pNoise = Noise.Noise2D(xSample, ySample, zOffset, GetGraph.seed);
				if (noise0To1) pNoise = (pNoise + 1) * 0.5f;

				vals[index] = pNoise;

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
		return "GradientNoise";
	}

	protected override void DoThisOrWeBreakStuff()
	{
		if (scale <= 0) scale = 0.0001f;
	}
}