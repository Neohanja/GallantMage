using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapTexture
{
    public static Texture2D TextureByHeight(int size, float[] heightmap, Gradient colorKey)
    {
        
        Color[] cols = new Color[size * size];

        for (int h = 0; h < size; h++)
        {
            for (int w = 0; w < size; w++)
            {
                float height = heightmap[h * size + w];
                height = (height + 1) * 0.5f;
                cols[h * size + w] = colorKey.Evaluate(height);
            }
        }

        Texture2D texture = new Texture2D(size, size);
        texture.SetPixels(cols);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        return texture;
    }
}
