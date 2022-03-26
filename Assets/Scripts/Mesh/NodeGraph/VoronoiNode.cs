using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class VoronoiNode : MeshNode
{
    public float scale;
    public int zOffset;
    public bool inverse;
    public bool useCurve;

    public override void ProcessNode()
    {
        float[] vals = new float[MeshData.MeshSize * MeshData.MeshSize];

        for (int x = 0; x < MeshData.MeshSize; x++)
        {
            for (int y = 0; y < MeshData.MeshSize; y++)
            {
                int index = x + y * MeshData.MeshSize;
                float xSample = (GetGraph.offset.x + x) / scale;
                float ySample = (GetGraph.offset.y + y) / scale;
                vals[index] = Noise.VoronoiNoise(new Vector2(xSample, ySample), zOffset, GetGraph.seed);
                if (inverse) vals[index] = 1f - vals[index];
                if (useCurve) vals[index] = MathFun.Curve(vals[index]);
            }
        }

        points.SetPoints(vals);
    }

    public override string GetString()
    {
        return "VornoiNoiseTwo";
    }

    void OnValidate()
    {
        if (scale <= 0) scale = 0.0001f;
    }
}
