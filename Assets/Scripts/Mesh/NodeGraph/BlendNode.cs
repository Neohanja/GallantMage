using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class BlendNode : MeshNode
{
    [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
    public HeightValues entryTwo;

    [Range(0f, 1f)]
    public float blend = 0.5f;
    public MathFunction blendType;

    public override void ProcessNode()
    {
        float[] a = GetInputValue("entry", entry).CopyPoints();
        float[] b = GetInputValue("entryTwo", entryTwo).CopyPoints();
        float[] vals = new float[a.Length];

        for(int bl = 0; bl < a.Length; bl++)
        {
            switch(blendType)
            {
                case MathFunction.Blend:
                    vals[bl] = MathFun.Lerp(a[bl], b[bl], blend);
                    break;
                case MathFunction.Add:
                    vals[bl] = a[bl] + b[bl] * blend;
                    break;
                case MathFunction.Multiply:
                    vals[bl] = a[bl] * b[bl] * blend;
                    break;
            }
        }

        points.SetPoints(vals);
    }

    public enum MathFunction
    {
        Blend,
        Add,
        Multiply
    }
}
