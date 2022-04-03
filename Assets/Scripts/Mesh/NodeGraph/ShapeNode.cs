using UnityEngine;
using XNode;

public class ShapeNode : MeshNode 
{
	public ShapeType shape;
	public float size = 1;
	public float fallOff = 3;
	public bool scatter;
	public bool averageLand;

	public override void ProcessNode()
    {
        float[] vals = GetInputValue("entry", entry).CopyPoints();
		Vector2 half = new Vector2(MeshData.MeshSize / 2, MeshData.MeshSize / 2);

        for (int x = 0; x < MeshData.MeshSize; x++)
		{
			for (int y = 0; y < MeshData.MeshSize; y++)
			{
				int index = x + y * MeshData.MeshSize;

				switch (shape)
				{
					case ShapeType.Circle:
						// Get the distance from the center
						float c = Vector2.Distance(half, new Vector2(x, y));
						// Subtract the size, then "normalize" the distance
						c -= size;
						if (c < 0) c = 0;
						// Create Falloff
						c += fallOff;
						// Divide it by half, to get the float, creating upper bound
						c /= half.x;						
						if (c > 1) c = 1f;
						// Now, inverse the distance results
						vals[index] = 1f - c;
						break;
					case ShapeType.Square:
						if(x > half.x - size && x < half.x + size &&
							y > half.y - size && y < half.y + size)
						{
							float s = MathFun.Abs(y - half.y + 1);
							s += MathFun.Abs(x - half.x + 1);
							s *= 0.5f;
							vals[index] = 1f - (s / (half.x + size));
						}
						else
						{
							vals[index] = 0f;
						}
						break;
					default:
						vals[index] = 1f;
						break;
				}
			}
		}

		points.SetPoints(vals);
    }

    public override string GetString()
    {
		return "ShapeNode";
	}

	public enum ShapeType
	{
		Circle,
		Square,		
	}
}