using UnityEngine;
using XNode;

public class CellAutoNode : MeshNode
{
	[Range(0f, 1f)]
	public float startAlive;
	public int smoothing;
	public int birth;
	public int death;
	public bool inverse;

	public override void ProcessNode()
    {
		float[] vals = Noise.CellAuto(MeshData.MeshSize, GetGraph.seed, startAlive, smoothing, birth, death);

        for (int x = 0; x < MeshData.MeshSize; x++)
		{
			for (int y = 0; y < MeshData.MeshSize; y++)
			{
				int index = x + y * MeshData.MeshSize;
				if (inverse) vals[index] = 1 - vals[index];
			}
		}

		points.SetPoints(vals);
    }

    public override string GetString()
    {
		return "Cellular Automata";
	}
}