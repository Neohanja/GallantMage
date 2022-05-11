using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshNode), true)]
public class MeshNodeImage : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MeshNode node = (MeshNode)target;

        node.ProcessNode();
        float[] data = node.points.GetPoints();

        int size = (int)Mathf.Sqrt(data.Length);
        Color[] colors = new Color[data.Length];

        for(int c = 0; c < data.Length; c++)
        {
            if (data[c] >= 0f)
            {
                colors[c] = Color.Lerp(Color.black, Color.white, data[c]);
            }
            else
            {
                colors[c] = Color.Lerp(Color.cyan, Color.blue, -data[c]);
            }
        }

        Texture2D result = new Texture2D(size, size);
        result.SetPixels(colors);
        result.filterMode = FilterMode.Point;
        result.wrapMode = TextureWrapMode.Clamp;
        result.Apply();

        GUILayout.Box(result);
    }
}
