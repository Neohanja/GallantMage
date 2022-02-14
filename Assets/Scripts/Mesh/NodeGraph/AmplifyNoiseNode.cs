using UnityEngine;
using XNode;

public class AmplifyNoiseNode : MeshNode 
{
	public MathFunction ampType;
	public float ampAmount;
	public bool clampNoise;
	public Vector2 minMax;

	public override void ProcessNode(bool addImage = false)
    {
        float[] vals = GetInputValue("entry", entry).CopyPoints();
		Color[] cols = new Color[vals.Length];

        for (int x = 0; x < MeshData.MeshSize; x++)
		{
			for (int y = 0; y < MeshData.MeshSize; y++)
			{
				int index = x + y * MeshData.MeshSize;

				switch(ampType)
                {
					case MathFunction.Add:
						vals[index] += ampAmount;
						break;
					case MathFunction.Subtract:
						vals[index] -= ampAmount;
						break;
					case MathFunction.Multiply:
						vals[index] *= ampAmount;
						break;
					case MathFunction.Divide:
						vals[index] /= ampAmount;
						break;
					case MathFunction.Absolute:
						vals[index] = MathFun.Abs(vals[index]);
						break;
                }

				if (clampNoise) vals[index] = MathFun.Clamp(minMax.x, minMax.y, vals[index]);

				if (addImage) cols[index] = Color.Lerp(Color.black, Color.white, vals[index]);
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
		return "ModNoiseNode";
	}

	public enum MathFunction
    {
		Add,
		Subtract,
		Multiply,
		Divide,
		Absolute
    }
}