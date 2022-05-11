using UnityEngine;
using XNode;

public class AmplifyNoiseNode : MeshNode 
{
	public MathFunction ampType;
	public float ampAmount;
	public float minEffected = -1f;
	public float maxEffected = 1f;
	public float clampMin = -1f;
	public float clampMax = 1f;

	public override void ProcessNode()
    {
        float[] vals = GetInputValue("entry", entry).CopyPoints();

        for (int x = 0; x < MeshData.MeshSize; x++)
		{
			for (int y = 0; y < MeshData.MeshSize; y++)
			{
				int index = x + y * MeshData.MeshSize;

				if (vals[index] >= minEffected && vals[index] <= maxEffected)
				{
					switch (ampType)
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

					vals[index] = MathFun.Clamp(clampMin, clampMax, vals[index]);
				}
			}
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