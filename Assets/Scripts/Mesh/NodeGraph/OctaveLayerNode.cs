using UnityEngine;
using XNode;

public class OctaveLayerNode : MeshNode 
{
	public float scale = 3.1415f;
	public int xOffset;
	public int octaves = 3;
	public float lacunarity = 0.5f;
	public float persistance = 2f;
	public bool noise0To1;


	public override void ProcessNode(bool addImage = false)
    {
        float[] vals = GetInputValue("entry", entry).CopyPoints();
		Color[] cols = new Color[vals.Length];

        for (int x = 0; x < MeshData.MeshSize; x++)
		{
			for (int y = 0; y < MeshData.MeshSize; y++)
			{
				int index = x + y * MeshData.MeshSize;

				float pNoise = 0f;
				float amp = 1f;
				float freq = 1f;

				for (int o = 0; o < octaves; o++)
                {
					float xSample = (GetGraph.offset.x + x) / scale * amp;
					float ySample = (GetGraph.offset.y + y) / scale * amp;
					float nVal = Noise.Noise2D(xSample, ySample, xOffset + o, GetGraph.seed);

					if (noise0To1) nVal = (nVal + 1) * 0.5f;

					pNoise += nVal * freq;
					freq *= lacunarity;
					amp *= persistance;
				}

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
		return "OctaveLayer";
	}

    protected override void DoThisOrWeBreakStuff()
    {
		if (scale <= 0f) scale = 0.00001f;
    }
}