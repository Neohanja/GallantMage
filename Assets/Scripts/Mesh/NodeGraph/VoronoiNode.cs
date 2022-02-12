using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class VoronoiNode : MeshNode
{
    public float scale;
    public int seed;
    public int zOffset;
    public bool inverse;
    public bool useCurve;

    public override void ProcessNode(bool addImage = false)
    {
        float[] vals = new float[MeshData.MeshSize * MeshData.MeshSize];
            //GetInputValue("entry", entry).CopyPoints();
        Color[] cols = new Color[vals.Length];

        for (int x = 0; x < MeshData.MeshSize; x++)
        {
            for (int y = 0; y < MeshData.MeshSize; y++)
            {
                int index = x + y * MeshData.MeshSize;
                float xSample = x / scale;
                float ySample = y / scale;
                vals[index] = Noise.VoronoiNoise(new Vector2(xSample, ySample), zOffset, seed);
                if (inverse) vals[index] = 1f - vals[index];
                if (addImage) cols[index] = Color.Lerp(Color.black, Color.white, vals[index]);
                if (useCurve) vals[index] = MathFun.Curve(vals[index]);
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
        return "VornoiNoiseTwo";
    }

    protected override void DoThisOrWeBreakStuff()
    {
        if (scale <= 0) scale = 0.0001f;
    }
}
