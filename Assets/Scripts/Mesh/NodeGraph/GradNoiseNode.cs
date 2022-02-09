using UnityEngine;
using XNode;

/// <summary>
/// Creates a 2D Noise Map
/// </summary>
public class GradNoiseNode : MeshNode
{
	[Input] public HeightValues entry;

	public int seed;
	public Vector2 offset;
	public float scale;
	public int octaves;
	public float lacunarity;
	public float persistance;
	public bool noise0To1;

	public override object GetValue(NodePort port)
	{
		float[] vals = entry.GetPoints();

		for(int x = 0; x < MeshData.MeshSize; x++)
        {
			for (int y = 0; y < MeshData.MeshSize; y++)
            {
				float pNoise = 0f;
				float amp = 1f;
				float freq = 1f;

				for (int o = 0; o < octaves; o++)
                {
					float xSample = (offset.x + x) / scale * freq;
					float ySample = (offset.y + y) / scale * freq;

					float noiseVal = Noise.Noise2D(xSample, ySample, o, seed);
					if (noise0To1) noiseVal = (noiseVal + 1) * 0.5f;
					pNoise += noiseVal * amp;

					amp *= persistance;
					freq *= lacunarity;
                }

				vals[x + y * MeshData.MeshSize] = pNoise;
            }

		}

		entry.SetPoints(vals);

		return entry;
	}

    public override string GetString()
    {
		return "GradientNoise";
    }
}