using UnityEngine;
using XNode;

/// <summary>
/// Creates a 2D Noise Map
/// </summary>
public class GradNoiseNode : MeshNode
{
	public int seed;
	public int zOffset;
	public float scale;
	public bool noise0To1;

	public override void ProcessNode()
	{
		float[] vals = GetInputValue("entry", entry).CopyPoints();

		for (int x = 0; x < MeshData.MeshSize; x++)
		{
			for (int y = 0; y < MeshData.MeshSize; y++)
			{
				float xSample = (x) / scale;
				float ySample = (y) / scale;

				float pNoise = Noise.Noise2D(xSample, ySample, zOffset, seed);
				if (noise0To1) pNoise = (pNoise + 1) * 0.5f;

				vals[x + y * MeshData.MeshSize] = pNoise;
			}
		}

		points.SetPoints(vals);
	}

    public override string GetString()
    {
		return "GradientNoise";
    }
}