using UnityEngine;
using XNode;

public class CellAutoNode : MeshNode
{
	public int seed;
	[Range(0f, 1f)]
	public float startAlive;
	public int smoothing;
	public int birth;
	public int death;
	public bool inverse;

	public override void ProcessNode(bool addImage = false)
    {
		float[] vals = Noise.CellAuto(MeshData.MeshSize, seed, new Vector2(0, 0), startAlive, smoothing, birth, death);
			//GetInputValue("entry", entry).CopyPoints();
		Color[] cols = new Color[vals.Length];

        for (int x = 0; x < MeshData.MeshSize; x++)
		{
			for (int y = 0; y < MeshData.MeshSize; y++)
			{
				int index = x + y * MeshData.MeshSize;
				if (inverse) vals[index] = 1 - vals[index];

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
		return "Cellular Automata";
	}
}