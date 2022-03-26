using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class AnimeCurve : MeshNode
{
    public bool inverse;
    public AnimationCurve curve;

    public override void ProcessNode()
    {
        float[] vals = GetInputValue("entry", entry).CopyPoints();

        for (int x = 0; x < MeshData.MeshSize; x++)
        {
            for (int y = 0; y < MeshData.MeshSize; y++)
            {
                int index = x + y * MeshData.MeshSize;
                if (inverse) vals[index] = 1f - vals[index];
                vals[index] = curve.Evaluate(vals[index]);
            }
        }

        points.SetPoints(vals);
    }

    public override string GetString()
    {
        return "VornoiNoiseTwo";
    }
}
