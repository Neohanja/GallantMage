using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class BlendNode : MeshNode
{
    [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
    public ChunkData entryTwo;

    public MathFunction blendType;
    [Range(0f, 1f)]
    public float blend = 0.5f;    

    public override void ProcessNode(bool addImage = false)
    {
        float[] a = GetInputValue("entry", entry).CopyPoints();
        float[] b = GetInputValue("entryTwo", entryTwo).CopyPoints();
        float[] vals = new float[a.Length];
        Color[] cols = new Color[a.Length];

        for(int bl = 0; bl < a.Length; bl++)
        {
            switch(blendType)
            {
                case MathFunction.Blend:
                    vals[bl] = MathFun.Lerp(a[bl], b[bl], blend);
                    break;
                case MathFunction.Subtract:
                    vals[bl] = a[bl] - b[bl] * blend;
                    break;
                case MathFunction.Add:
                    vals[bl] = a[bl] + b[bl] * blend;
                    break;
                case MathFunction.Multiply:
                    vals[bl] = a[bl] * b[bl] * blend;
                    break;
                case MathFunction.Difference:
                    vals[bl] = MathFun.Abs(a[bl] - b[bl]);
                    break;
                case MathFunction.BlendDif:
                    vals[bl] = blend - MathFun.Abs(a[bl] - b[bl]);
                    break;
                case MathFunction.MinVal:
                    vals[bl] = a[bl] < b[bl] ? a[bl] : b[bl];
                    break;
                case MathFunction.MaxVal:
                    vals[bl] = a[bl] < b[bl] ? b[bl] : a[bl];
                    break;
            }

            if (addImage) cols[bl] = Color.Lerp(Color.black, Color.white, vals[bl]);
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

    public enum MathFunction
    {
        Blend,
        Add,
        Multiply,
        Subtract,
        Difference,
        BlendDif,
        MinVal,
        MaxVal
    }
}
